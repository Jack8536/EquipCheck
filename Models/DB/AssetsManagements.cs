using System;
using System.Collections.Generic;

namespace EquipCheck.Models.DB;

public partial class AssetsManagements
{
    public Guid AssetUid { get; set; }

    public string AssetCode { get; set; } = null!;

    public string AssetName { get; set; } = null!;

    public Guid Omduid { get; set; }

    public Guid UserUid { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int Status { get; set; }

    public bool? IsDeleted { get; set; }

    public Guid CreateUser { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifyUser { get; set; }

    public DateTime? ModifyDate { get; set; }
}
