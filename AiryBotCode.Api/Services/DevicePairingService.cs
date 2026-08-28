using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace AiryBotCode.Api.Services
{
    public enum PairingState { Pending, Approved, Denied }

    public sealed class PairingRequest
    {
        public string Code { get; init; } = "";
        public string DeviceId { get; init; } = "";
        public string DeviceSecret { get; init; } = "";
        public string DeviceName { get; init; } = "";
        public string Os { get; init; } = "";
        public string AppVersion { get; init; } = "";
        public string RequestedFromIp { get; init; } = "";
        public DateTimeOffset RequestedAt { get; init; }
        public DateTimeOffset ExpiresAt { get; init; }

        public PairingState State { get; set; } = PairingState.Pending;
        public string? Token { get; set; }
        public string? ApprovedBy { get; set; }
    }

    /// <summary>
    /// Short-lived device pairings, held in memory. A desktop app with no credential
    /// starts one, shows the code and its own name so the human can see WHICH machine
    /// is asking, and polls. Someone already signed in approves that code in the
    /// browser, and the poll then returns a token.
    ///
    /// In memory on purpose: a pairing lives ten minutes and is worthless afterwards,
    /// so it is not worth a table or a migration. A restart cancels anything in flight,
    /// which is the correct outcome anyway.
    /// </summary>
    public sealed class DevicePairingService
    {
        // No I/O/0/1 — these get read off one screen and typed into another.
        private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);

        private readonly ConcurrentDictionary<string, PairingRequest> _byCode = new(StringComparer.OrdinalIgnoreCase);

        public PairingRequest Start(string deviceName, string os, string appVersion, string ip)
        {
            Sweep();

            var now = DateTimeOffset.UtcNow;
            var request = new PairingRequest
            {
                Code = NewCode(),
                DeviceId = Guid.NewGuid().ToString("N"),
                DeviceSecret = RandomToken(32),
                DeviceName = Trim(deviceName, 64, "unnamed device"),
                Os = Trim(os, 32, "unknown"),
                AppVersion = Trim(appVersion, 32, "unknown"),
                RequestedFromIp = Trim(ip, 45, "unknown"),
                RequestedAt = now,
                ExpiresAt = now.Add(Lifetime),
            };

            _byCode[request.Code] = request;
            return request;
        }

        public PairingRequest? Find(string code)
        {
            Sweep();
            if (string.IsNullOrWhiteSpace(code)) return null;
            return _byCode.TryGetValue(code.Trim(), out var r) && !Expired(r) ? r : null;
        }

        public IReadOnlyList<PairingRequest> Pending()
        {
            Sweep();
            return _byCode.Values
                .Where(r => r.State == PairingState.Pending && !Expired(r))
                .OrderByDescending(r => r.RequestedAt)
                .ToList();
        }

        public bool Approve(string code, string token, string approvedBy)
        {
            var request = Find(code);
            if (request is null || request.State != PairingState.Pending) return false;
            request.Token = token;
            request.ApprovedBy = approvedBy;
            request.State = PairingState.Approved;
            return true;
        }

        public bool Deny(string code)
        {
            var request = Find(code);
            if (request is null || request.State != PairingState.Pending) return false;
            request.State = PairingState.Denied;
            return true;
        }

        /// <summary>
        /// The device polls with the secret it was given at start, so knowing a code —
        /// which is short and shown on a screen — is not enough to collect the token.
        /// An approved pairing is consumed on the first successful poll.
        /// </summary>
        public (PairingState State, string? Token)? Poll(string deviceId, string deviceSecret)
        {
            Sweep();
            var request = _byCode.Values.FirstOrDefault(r => r.DeviceId == deviceId);
            if (request is null || Expired(request)) return null;

            if (!FixedTimeEquals(request.DeviceSecret, deviceSecret)) return null;

            if (request.State != PairingState.Approved) return (request.State, null);

            _byCode.TryRemove(request.Code, out _);
            return (PairingState.Approved, request.Token);
        }

        private void Sweep()
        {
            foreach (var pair in _byCode)
                if (Expired(pair.Value))
                    _byCode.TryRemove(pair.Key, out _);
        }

        private static bool Expired(PairingRequest r) => DateTimeOffset.UtcNow >= r.ExpiresAt;

        private static string Trim(string? value, int max, string fallback)
        {
            var v = (value ?? "").Trim();
            if (v.Length == 0) return fallback;
            return v.Length <= max ? v : v[..max];
        }

        private static string NewCode()
        {
            Span<char> chars = stackalloc char[9];
            for (var i = 0; i < chars.Length; i++)
                chars[i] = i == 4 ? '-' : CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
            return new string(chars);
        }

        private static string RandomToken(int bytes) =>
            Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes)).ToLowerInvariant();

        private static bool FixedTimeEquals(string a, string b)
        {
            var x = System.Text.Encoding.UTF8.GetBytes(a);
            var y = System.Text.Encoding.UTF8.GetBytes(b ?? "");
            return x.Length == y.Length && CryptographicOperations.FixedTimeEquals(x, y);
        }
    }
}
