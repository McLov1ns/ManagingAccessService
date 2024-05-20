using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class EmploymentHistory
{
    public int EmploymentId { get; set; }

    public int? EmployeeId { get; set; }

    public int? OrganizationId { get; set; }

    public string? Department { get; set; }

    public int? PositionId { get; set; }

    public int? Supervisor { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<DutiesNoncompliance> DutiesNoncompliances { get; set; } = new List<DutiesNoncompliance>();

    public virtual Employee? Employee { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual Position? Position { get; set; }

    public virtual Employee? SupervisorNavigation { get; set; }
}
