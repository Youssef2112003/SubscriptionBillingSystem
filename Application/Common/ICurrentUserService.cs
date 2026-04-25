namespace SPS.Application.Common;

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();
    string GetCurrentUserName();
    string GetCurrentUserRole();
    IReadOnlyList<string> GetCurrentUserRoles();
    bool IsAuthenticated();
}