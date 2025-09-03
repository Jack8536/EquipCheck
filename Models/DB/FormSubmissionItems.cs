using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class FormSubmissionItems
{
    public Guid SubmissionItemUid { get; set; }

    public Guid SubmissionUid { get; set; }

    public Guid ChecklistItemUid { get; set; }

    public int IsChecked { get; set; }

    public string? Remark { get; set; }

    public virtual FormSubmissions SubmissionU { get; set; } = null!;
}
