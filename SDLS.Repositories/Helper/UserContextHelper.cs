using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Repositories.Helper
{
    public static class UserContextHelper
    {
        public static bool IsAuthenticated(IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public static string? GetUserId(IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetRole(IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Role)?.Value;
        }

        public static Guid? GetCurrentUserId(IHttpContextAccessor accessor)
        {
            var rawUserId = GetUserId(accessor);
            return Guid.TryParse(rawUserId, out var userId) ? userId : null;
        }

        public static Guid GetRequiredCurrentUserId(IHttpContextAccessor accessor)
        {
            var currentUserId = GetCurrentUserId(accessor);
            if (!currentUserId.HasValue || currentUserId.Value == Guid.Empty)
                throw new UnauthorizedAccessException("Không xác định được UserId từ JWT.");

            return currentUserId.Value;
        }
    }
}
