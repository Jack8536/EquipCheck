using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class ActionLogs
{
    public long Pid { get; set; }

    public int Action { get; set; }

    public bool IsSuccess { get; set; }

    public string Ip { get; set; } = null!;

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? Event { get; set; }
}
