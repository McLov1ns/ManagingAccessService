using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class Position
{
    public int PositionId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
}
