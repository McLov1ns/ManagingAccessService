using System;
using System.Collections.Generic;
using ManagingAccessService.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace ManagingAccessService.Models.DBContext;

public partial class ManagingAccessServiceContext : DbContext
{
    public ManagingAccessServiceContext(DbContextOptions<ManagingAccessServiceContext> options)
        : base(options)
    {
        Database.EnsureCreated();
        DbInitializer.Initialize(this);
    }

    public ManagingAccessServiceContext()
    {
    }

    public virtual DbSet<AccessLog> AccessLogs { get; set; }

    public virtual DbSet<AccessRestriction> AccessRestrictions { get; set; }

    public virtual DbSet<ContactInformation> ContactInformations { get; set; }

    public virtual DbSet<DutiesNoncompliance> DutiesNoncompliances { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<EmploymentHistory> EmploymentHistories { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<WorkSchedule> WorkSchedules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ManagingAccessService;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        } 
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<AccessLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Access_L__2D26E7AE3FD807B7");

            entity.ToTable("Access_Log");

            entity.Property(e => e.LogId)
                .ValueGeneratedNever()
                .HasColumnName("Log_ID");
            entity.Property(e => e.AccessTime).HasColumnName("Access_Time");
            entity.Property(e => e.AccountId).HasColumnName("Account_ID");
            entity.Property(e => e.ActionPerformed)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Action_Performed");

            entity.HasOne(d => d.Account).WithMany(p => p.AccessLogs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Access_Lo__Accou__38996AB5");
        });

        modelBuilder.Entity<AccessRestriction>(entity =>
        {
            entity.HasKey(e => e.RestrictionId).HasName("PK__Access_R__1D4F7CD6F6019110");

            entity.ToTable("Access_Restrictions");

            entity.Property(e => e.RestrictionId)
                .ValueGeneratedNever()
                .HasColumnName("Restriction_ID");
            entity.Property(e => e.AccessFrom).HasColumnName("Access_From");
            entity.Property(e => e.AccessTo).HasColumnName("Access_To");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("IP");

            entity.HasOne(d => d.Employee).WithMany(p => p.AccessRestrictions)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Access_Re__Emplo__267ABA7A");
        });

        modelBuilder.Entity<ContactInformation>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contact___82ACC1CD23B85382");

            entity.ToTable("Contact_Information");

            entity.Property(e => e.ContactId)
                .ValueGeneratedNever()
                .HasColumnName("Contact_ID");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Email_Address");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.ContactInformations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Contact_I__Emplo__3E52440B");
        });

        modelBuilder.Entity<DutiesNoncompliance>(entity =>
        {
            entity.HasKey(e => e.NoncomplianceId).HasName("PK__Duties_N__1EE28D0977ABF65D");

            entity.ToTable("Duties_Noncompliance");

            entity.Property(e => e.NoncomplianceId)
                .ValueGeneratedNever()
                .HasColumnName("Noncompliance_ID");
            entity.Property(e => e.EmploymentId).HasColumnName("Employment_ID");
            entity.Property(e => e.EndDate).HasColumnName("End_Date");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Employment).WithMany(p => p.DutiesNoncompliances)
                .HasForeignKey(d => d.EmploymentId)
                .HasConstraintName("FK__Duties_No__Emplo__47DBAE45");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__78113481A2F5682C");

            entity.Property(e => e.EmployeeId)
                .HasColumnName("Employee_ID");
            entity.Property(e => e.ContactInformation)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Contact_Information");
            entity.Property(e => e.DateOfBirth).HasColumnName("Date_of_Birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Full_Name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Identifier)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmployeeRoleId).HasName("PK__Employee__4AC09085C75E5DFE");

            entity.ToTable("Employee_Roles");

            entity.Property(e => e.EmployeeRoleId)
                .ValueGeneratedNever()
                .HasColumnName("Employee_Role_ID");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.EndDate).HasColumnName("End_Date");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Employee___Emplo__2B3F6F97");

            entity.HasOne(d => d.Role).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Employee___Role___2C3393D0");
        });

        modelBuilder.Entity<EmploymentHistory>(entity =>
        {
            entity.HasKey(e => e.EmploymentId).HasName("PK__Employme__56060C95D1AAB61F");

            entity.ToTable("Employment_History");

            entity.Property(e => e.EmploymentId)
                .ValueGeneratedNever()
                .HasColumnName("Employment_ID");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.EndDate).HasColumnName("End_Date");
            entity.Property(e => e.OrganizationId).HasColumnName("Organization_ID");
            entity.Property(e => e.PositionId).HasColumnName("Position_ID");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmploymentHistoryEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Employmen__Emplo__412EB0B6");

            entity.HasOne(d => d.Organization).WithMany(p => p.EmploymentHistories)
                .HasForeignKey(d => d.OrganizationId)
                .HasConstraintName("FK__Employmen__Organ__4316F928");

            entity.HasOne(d => d.Position).WithMany(p => p.EmploymentHistories)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employmen__Posit__4222D4EF");

            entity.HasOne(d => d.SupervisorNavigation).WithMany(p => p.EmploymentHistorySupervisorNavigations)
                .HasForeignKey(d => d.Supervisor)
                .HasConstraintName("FK__Employmen__Super__440B1D61");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__A6FA250696BD1872");

            entity.Property(e => e.OrganizationId)
            .HasColumnName("Organization_ID")
            .ValueGeneratedOnAdd();
            entity.Property(e => e.Inn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("INN");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Ogrn)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("OGRN");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__3C3EAFE6F2EC8586");

            entity.Property(e => e.PositionId)
                .ValueGeneratedNever()
                .HasColumnName("Position_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__D80AB49B23297BF4");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("Role_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Role_Per__89B744E583F451AB");

            entity.ToTable("Role_Permissions");

            entity.Property(e => e.PermissionId)
                .ValueGeneratedNever()
                .HasColumnName("Permission_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Role_Perm__Role___2F10007B");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__User_Acc__B19E45C941A4E13F");

            entity.ToTable("User_Accounts");

            entity.Property(e => e.AccountId).HasColumnName("Account_ID");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");
            entity.Property(e => e.LastLogin).HasColumnName("Last_Login");
            entity.Property(e => e.LastPasswordChange).HasColumnName("Last_Password_Change");
            entity.Property(e => e.Email).HasColumnName("Email");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__User_Acco__Emplo__31EC6D26");
            entity.HasOne(d => d.Role).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User_Acco___Role___2C3393D0");
        });

        modelBuilder.Entity<WorkSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Work_Sch__8C4D3BBBA4A0D430");

            entity.ToTable("Work_Schedules");

            entity.Property(e => e.ScheduleId)
                .ValueGeneratedNever()
                .HasColumnName("Schedule_ID");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Day_of_Week");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_ID");
            entity.Property(e => e.EndTime).HasColumnName("End_Time");
            entity.Property(e => e.StartTime).HasColumnName("Start_Time");

            entity.HasOne(d => d.Employee).WithMany(p => p.WorkSchedules)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Work_Sche__Emplo__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }
    public static class DbInitializer
    {
        public static void Initialize(ManagingAccessServiceContext context)
        {
            if (context.Employees.Any())
            {
                return;   // Данные уже инициализированы
            }

            // Добавление данных в таблицу Employees
            var employees = new[]
            {
            new Employee {FullName = "Иванов Иван Иванович", Gender = "Мужской", DateOfBirth = DateOnly.Parse("1990-05-15"), Identifier = "12345678", ContactInformation = "телефон: 123-456, email: ivanov@example.com", Status = "Активный" },
            new Employee {FullName = "Петров Петр Петрович", Gender = "Мужской", DateOfBirth = DateOnly.Parse("1985-08-25"), Identifier = "87654321", ContactInformation = "телефон: 987-654, email: petrov@example.com", Status = "Активный" }
        };
            context.Employees.AddRange(employees);

            // Добавление данных в таблицу Roles
            var roles = new[]
            {
            new Role { RoleId = 1, Name = "Администратор" },
            new Role { RoleId = 2, Name = "Пользователь" }
        };
            context.Roles.AddRange(roles);

            // Добавление данных в таблицу Organizations
            var organizations = new[]
            {
            new Organization {Name = "ООО \"Рога и копыта\"", Inn = "1234567890", Ogrn = "0987654321", Status = "Активная" },
            new Organization {Name = "ООО \"Птичка\"", Inn = "9876543210", Ogrn = "0123456789", Status = "Активная" }
        };
            context.Organizations.AddRange(organizations);

            // Добавление данных в таблицу User_Accounts
        //    var userAccounts = new[]
        //    {
        //    new UserAccount {RoleId = 1, Login = "ivanov", Password = "ivanov_password", LastLogin = null, LastPasswordChange = DateOnly.Parse("2024-03-20"), Status = "Активный" },
        //    new UserAccount {RoleId = 2, Login = "petrov", Password = "petrov_password", LastLogin = null, LastPasswordChange = DateOnly.Parse("2024-03-21"), Status = "Активный" }
        //};
        //    context.UserAccounts.AddRange(userAccounts);
            context.SaveChanges();
        }
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
