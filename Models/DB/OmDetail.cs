using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class OmDetail
{
    public Guid Omduid { get; set; }

    public Guid Omuid { get; set; }

    public string? DetailName { get; set; }

    public int Status { get; set; }

    public int Sort { get; set; }

    public string? Remark { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifydDate { get; set; }

    public virtual OptionManagements Omu { get; set; } = null!;
}
