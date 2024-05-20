using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class AccessRestriction
{
    public int RestrictionId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly? AccessFrom { get; set; }

    public DateOnly? AccessTo { get; set; }

    public string? Ip { get; set; }

    public virtual Employee? Employee { get; set; }
}
