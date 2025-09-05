using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class VM_Asset
    {
        // 資產編號
        public string AssetTag { get; set; }

        // 資產名稱/型別
        public string AssetName { get; set; }

        public Guid CategoryId { get; set; }

        // 採購人員
        public Guid BuyerId { get; set; }

        // 購買日期

        public DateTime PurchaseDate { get; set; }

        public AssetListModel Asset { get; set; }

        // 狀態
        public int Status { get; set; }
        public List<AssetListModel> AssetList { get; set; } = new List<AssetListModel>();

        // 下拉
        public List<SelectListItem> CategoryDDL { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> BuyerDDL { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> StatusDDL { get; set; } = new List<SelectListItem>();
    }

    public class AssetListModel
    {
        // 資產id
        public Guid AssetId { get; set; }

        // 資產編號
        public string AssetTag { get; set; }
        // 資產名稱/型別
        public string AssetName { get; set; }

        // 資產類別id
        public Guid CategoryId { get; set; }

        // 資產類別
        public string Category { get; set; }

        // 購買日期
        public DateTime _PurchaseDate { get; set; }

        // 購買日期
        public string PurchaseDate { get; set; }

        // 狀態
        public int _Status { get; set; }

        // 狀態
        public string Status { get; set; }

        // 採購人員
        public Guid UserId { get; set; }
        // 採購人員
        public string UserName { get; set; }
    }
}
