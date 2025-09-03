using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class Departments
{
    public Guid DepartmentUid { get; set; }

    public string DepartmentName { get; set; } = null!;

    public Guid ManagerUid { get; set; }

    public int Status { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }
}
