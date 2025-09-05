using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class FormsManagements
{
    public Guid FormUid { get; set; }

    public string Year { get; set; } = null!;

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public string FormName { get; set; } = null!;

    public Guid Sponsor { get; set; }

    public int Status { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }

    public virtual ICollection<FormChecklistItems> FormChecklistItems { get; set; } = new List<FormChecklistItems>();
}
