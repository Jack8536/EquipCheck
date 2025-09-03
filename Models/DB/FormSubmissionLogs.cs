using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class FormSubmissionLogs
{
    public Guid SubmissionLogUid { get; set; }

    public Guid SubmissionUid { get; set; }

    public int Status { get; set; }

    public Guid UserUid { get; set; }

    public DateTime ModifyDate { get; set; }

    public virtual FormSubmissions SubmissionU { get; set; } = null!;
}
