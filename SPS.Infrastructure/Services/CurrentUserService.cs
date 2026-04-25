using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SPS.Application.Common;
using System.Security.Claims;

namespace SPS.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Guid? GetCurrentUserId()
        {
            var sub = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var guid) ? guid : null;
        }

        public string GetCurrentUserName()
            => _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown";

        public string GetCurrentUserRole()
            => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";

        public IReadOnlyList<string> GetCurrentUserRoles()
            => _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList() ?? new List<string>();

        public bool IsAuthenticated()
            => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
