using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class FormSubmissions
{
    public Guid SubmissionUid { get; set; }

    public Guid? FormUid { get; set; }

    public Guid UserUid { get; set; }

    public Guid DepartmentUid { get; set; }

    public string Tel { get; set; } = null!;

    public Guid AssestUid { get; set; }

    public DateTime CheckDate { get; set; }

    public int Status { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }

    public virtual ICollection<FormSubmissionItems> FormSubmissionItems { get; set; } = new List<FormSubmissionItems>();

    public virtual ICollection<FormSubmissionLogs> FormSubmissionLogs { get; set; } = new List<FormSubmissionLogs>();
}
