using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDLS.Model.Constants
{
    public static class RoleConst
    {
        public static readonly Guid USER_ROLE_ID = Guid.Parse("66efa899-db8f-4866-bfe9-6fe9d98eca7e");

        public static readonly Guid GUEST_ROLE_ID = Guid.Parse("67b62c82-e459-4b1a-b912-e1758a5c87c4");
        
        public static readonly Guid INSTRUCTOR_ROLE_ID = Guid.Parse("179599cd-c7c9-4b3b-b0c7-f832991dc7a4");

        public static readonly Guid ADMIN_ROLE_ID = Guid.Parse("90292570-fb35-445c-9f1f-a37dcdd48e5e"); 
    }
}
