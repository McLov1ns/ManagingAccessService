using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class DutiesNoncompliance
{
    public int NoncomplianceId { get; set; }

    public int? EmploymentId { get; set; }

    public string? Status { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual EmploymentHistory? Employment { get; set; }
}
