using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class FormChecklistItems
{
    public Guid ChecklistItemUid { get; set; }

    public Guid FormUid { get; set; }

    public string ItemName { get; set; } = null!;

    public int Sort { get; set; }

    public int Status { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }

    public virtual FormsManagements FormU { get; set; } = null!;
}
