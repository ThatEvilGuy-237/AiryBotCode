using System;
using System.Collections.Generic;
using System.Linq;

namespace AiryBotCode.Events.ButtonPress
{
    // encription: c:command|a:action|u:userId|al:allowedRoleId|t:targetId|ts:timestamp
    // example: c:edit|a:update|u:123456789|al:987654321|t:111111111|t:111111112|ts:12454548775757

    public class ButtonEncoder
    {
        public string Command; // c:
        public string Action; // a:
        public List<ulong> UsersId; // u: // can be more than 1, the user wants it too.
        public List<ulong> AllowedRoleId; // al: // can be more than 1, the user wants it too.
        public List<ulong> TargetId; // t: // can be more than 1, the user wants it too.
        public DateTime Timestamp; // ts:

        public ButtonEncoder()
        {
            UsersId = new List<ulong>();
            AllowedRoleId = new List<ulong>();
            TargetId = new List<ulong>();
        }
        public ButtonEncoder(string command, string action, List<ulong> usersId, List<ulong> allowedRoleId, List<ulong> targetId, DateTime timestamp)
        {
            Command = command;
            Action = action;
            UsersId = usersId;
            AllowedRoleId = allowedRoleId;
            TargetId = targetId;
            Timestamp = timestamp;
        }

        // Encryption
        public string Encript()
        {
            // Command and Action: REQUIRED
            string result = $"c:{Command}|a:{Action}";

            // Users
            if (UsersId.Count > 0)
            {
                foreach (var userId in UsersId)
                {
                    result += $"|u:{userId}";
                }
            }

            // Roles
            if (AllowedRoleId.Count > 0)
            {
                foreach (var roleId in AllowedRoleId)
                {
                    result += $"|al:{roleId}";
                }
            }

            // Targets
            if (TargetId.Count > 0)
            {
                foreach (var target in TargetId)
                {
                    result += $"|t:{target}";
                }
            }

            // Timestamp
            if (Timestamp > DateTime.MinValue && Timestamp < DateTime.MaxValue)
            {
                result += $"|ts:{new DateTimeOffset(Timestamp).ToUnixTimeMilliseconds()}";
            }

            return result;
        }

        // IsValid: debug method. Best not to use in practice.
        public bool IsValid(string code, ref string error)
        {
            if (string.IsNullOrWhiteSpace(Command) || string.IsNullOrWhiteSpace(Action))
            {
                error = $"Command:'{Command}' and Action:'{Action}' must be set.";
                return false;
            }
            if (code.Count() > 100)
            {
                error = $"Value must be shorter. Currently it's {code.Count()} chars.";
                return false;
            }
            return true;
        }

        // Decryption
        public ButtonEncoder Decript(string code)
        {

            var parts = code.Split('|');

            foreach (var part in parts)
            {
                var keyValue = part.Split(':');
                if (keyValue.Length != 2) continue;

                var key = keyValue[0];
                var value = keyValue[1];

                switch (key)
                {
                    case "c": Command = value; break;
                    case "a": Action = value; break;
                    case "u": UsersId.Add(ulong.Parse(value)); break;
                    case "al": AllowedRoleId.Add(ulong.Parse(value)); break;
                    case "t": TargetId.Add(ulong.Parse(value)); break;
                    case "ts": Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value)).DateTime; break;
                }
            }
     
            return this;
        }
    }


}
