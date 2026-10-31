using System;
using System.Collections.Generic;

namespace Store.DtoLayer.IdentityDtos.RoleDtos
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}
