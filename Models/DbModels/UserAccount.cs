using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManagingAccessService.Models.DbModels;

public partial class UserAccount
{
    [Key]
    public int AccountId { get; set; }

    public int? EmployeeId { get; set; }
    [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты.")]
    public string? Email { get; set; }
    [Required(ErrorMessage ="Не указан логин.")]
    public string? Login { get; set; }

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public DateOnly? LastLogin { get; set; }

    public DateOnly? LastPasswordChange { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();

    public virtual Employee? Employee { get; set; }
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
        = new Role()
        {
            RoleId = 2,
            Name = "Пользователь",
        };
}
