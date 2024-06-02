using System;
using System.Collections.Generic;

namespace ManagingAccessService.Models.DbModels;

public partial class WorkSchedule
{
    internal string DayofWeek;

    public int ScheduleId { get; set; }

    public int? EmployeeId { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public virtual Employee? Employee { get; set; }
}
