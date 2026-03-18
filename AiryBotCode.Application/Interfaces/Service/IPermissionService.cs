namespace AiryBotCode.Application.Interfaces.Service
{
    public interface IPermissionService
    {
        bool IsInstallationAllowed();
        void GrantInstallationPermission(TimeSpan duration);
    }
}
