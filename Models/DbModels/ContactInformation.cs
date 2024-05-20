using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class ContactInformation
{
    public int ContactId { get; set; }

    public int? EmployeeId { get; set; }

    public string? Phone { get; set; }

    public string? EmailAddress { get; set; }

    public virtual Employee? Employee { get; set; }
}
