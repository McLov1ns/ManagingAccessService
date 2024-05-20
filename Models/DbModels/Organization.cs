using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class Organization
{
    public int OrganizationId { get; set; }

    public string? Name { get; set; }

    public string? Inn { get; set; }

    public string? Ogrn { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}
