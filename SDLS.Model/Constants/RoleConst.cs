using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.Constants
{
    public static class RoleConst
    {
        public static readonly Guid GUEST_ROLE_ID =
        Guid.Parse("77a25d78-1b9b-47d2-88a0-f58615a694c4");

        public static readonly Guid USER_ROLE_ID = Guid.Parse("a764b8eb-b030-4387-a27f-a0242b6dd0b0");

        public static readonly Guid ADMIN_ROLE_ID = Guid.Parse("d6b974b2-f2e1-435e-8a5a-08fa0ca24fe5");
    }
}
