using AiryBotCode.Application.Interfaces.Service;

namespace AiryBotCode.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private DateTime _installationPermissionExpiry = DateTime.MinValue;

        public bool IsInstallationAllowed()
        {
            return DateTime.UtcNow < _installationPermissionExpiry;
        }

        public void GrantInstallationPermission(TimeSpan duration)
        {
            _installationPermissionExpiry = DateTime.UtcNow.Add(duration);
        }
    }
}
