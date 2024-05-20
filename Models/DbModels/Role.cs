using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class Role
{
    public int RoleId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
    public Role()
    {
        UserAccounts = new List<UserAccount>();
    }
}
