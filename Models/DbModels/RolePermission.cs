using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class RolePermission
{
    public int PermissionId { get; set; }

    public int? RoleId { get; set; }

    public string? Name { get; set; }

    public virtual Role? Role { get; set; }
}
