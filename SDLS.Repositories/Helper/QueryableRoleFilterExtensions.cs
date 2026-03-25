using Microsoft.EntityFrameworkCore;

namespace SDLS.Repositories.Helper
{
    public static class QueryableRoleFilterExtensions
    {
        public static IQueryable<T> ApplyRoleFilter<T>(this IQueryable<T> query, string? role) where T : class
        {
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Instructor", StringComparison.OrdinalIgnoreCase))
            {
                return query;
            }

            return query.Where(x => EF.Property<int?>(x, "Status") != 0);
        }

        public static bool IsPrivilegedRole(string? role)
        {
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(role, "Instructor", StringComparison.OrdinalIgnoreCase);
        }
    }
}