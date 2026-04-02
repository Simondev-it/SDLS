using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.Constants
{
    public static class RoleConst
    {
        public static readonly Guid USER_ROLE_ID = Guid.Parse("b944491c-af9f-485c-aa88-3671314dcc04");

        public static readonly Guid GUEST_ROLE_ID = Guid.Parse("b944491c-af9f-485c-aa88-3671314dcc04");
        
        public static readonly Guid INSTRUCTOR_ROLE_ID = Guid.Parse("77c846e8-c1dd-43e5-9165-d7967d550543");

        public static readonly Guid ADMIN_ROLE_ID = Guid.Parse("622655ad-39f0-4c48-ae85-c4a6dfb2ef83"); 
    }
}
