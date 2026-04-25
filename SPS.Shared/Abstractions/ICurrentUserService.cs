namespace SPS.Shared.Abstractions
{
    public interface ICurrentUserService
    {
        Guid? GetCurrentUserId();
        string GetCurrentUserName();
        string GetCurrentUserRole();
        IReadOnlyList<string> GetCurrentUserRoles();  
        string GetCurrentUserType();
        bool IsAuthenticated();
    }
}