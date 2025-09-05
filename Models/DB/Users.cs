using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class Users
{
    public Guid UserUid { get; set; }

    public string UserName { get; set; } = null!;

    public string UserAccount { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid DepartmentUid { get; set; }

    public int Role { get; set; }

    public string Tel { get; set; } = null!;

    public string? SignaturePath { get; set; }

    public int Status { get; set; }

    public string? Remark { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }
}
