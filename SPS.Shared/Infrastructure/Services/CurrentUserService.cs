using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SPS.Shared.Abstractions;
using System.Security.Claims;

namespace SPS.Shared.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        private const string SubClaimType = "sub";
        private const string UserIdClaimType = "user_id";
        private const string IdClaimType = "id";
        private const string NameClaimType = "name";
        private const string UserNameClaimType = "username";
        private const string RoleClaimType = "role";



        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //public Guid? GetCurrentUserId()
        //{
        //    var subClaim = _httpContextAccessor.HttpContext?.User
        //        ?.FindFirst(ClaimTypes.NameIdentifier);

        //    if (subClaim?.Value is not null && Guid.TryParse(subClaim.Value, out var userId))
        //        return userId;

        //    _logger.LogWarning("JWT 'sub' claim missing or invalid. Claims: {Claims}",
        //        string.Join(", ", _httpContextAccessor.HttpContext?.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? []));
        //    return null;
        //}

        public Guid? GetCurrentUserId()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }


                var userIdString =
                    httpContext.User.FindFirst(SubClaimType)?.Value ??
                    httpContext.User.FindFirst(UserIdClaimType)?.Value ??
                    httpContext.User.FindFirst(IdClaimType)?.Value ??
                    httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? httpContext.User.FindFirst("sub")?.Value
                   ?? httpContext.User.FindFirst("user_id")?.Value;

                if (Guid.TryParse(userIdString, out var userIdGuid))
                {
                    return userIdGuid;
                }

                _logger.LogWarning("Authenticated user found, but no valid GUID could be parsed from claims for User ID. Claim value: {UserIdString}", userIdString);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user ID due to an unexpected error.");
                return null;
            }
        }
        public string GetCurrentUserName()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return "Unkown User";
                }

                var userName =
                    httpContext.User.FindFirst(NameClaimType)?.Value ??
                    httpContext.User.FindFirst(UserNameClaimType)?.Value ??
                    httpContext.User.FindFirst(ClaimTypes.Name)?.Value ??
                    httpContext.User.Identity.Name;

                return !string.IsNullOrEmpty(userName) ? userName : "Unkown User";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user name due to an unexpected error.");
                return "Unkown User";
            }
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public string GetCurrentUserRole()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                {
                    return "Not Authorized";
                }

                var role =
                    httpContext.User.FindFirst(RoleClaimType)?.Value ??
                    httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

                return !string.IsNullOrEmpty(role) ? role : "Unkown Role";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user role due to an unexpected error.");
                return "Unkown Role";
            }
        }

        public string GetCurrentUserType()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("UserType")?.Value ?? "Unknown";
        }

        public IReadOnlyList<string> GetCurrentUserRoles()
        {
            return _httpContextAccessor.HttpContext?.User?
       .FindAll(ClaimTypes.Role)
       .Select(c => c.Value)
       .ToList() ?? new List<string>();
        }

        //public IReadOnlyList<string> GetCurrentUserRoles()
        //{
        //    return _httpContextAccessor.HttpContext?.User?
        //        .FindAll(ClaimTypes.Role)
        //        .Select(c => c.Value)
        //        .ToList() ?? [];
        //}


    }
}
