using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class OptionManagements
{
    public Guid Omuid { get; set; }

    public int Category { get; set; }

    public string OptionName { get; set; } = null!;

    public int Status { get; set; }

    public int? Sort { get; set; }

    public string? Remark { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifydDate { get; set; }

    public virtual ICollection<OmDetail> OmDetail { get; set; } = new List<OmDetail>();
}
