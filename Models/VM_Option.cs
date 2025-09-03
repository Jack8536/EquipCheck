using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class VM_Option
    {
        //選項類別id

        public Guid Omuid { get; set; }

        // 選項類別
        public int OptionCategory { get; set; }

        // 選項名稱
        public string OptionName { get; set; }

        // 狀態
        public int Status { get; set; }

        // 備註
        public string Remark { get; set; }

        public List<OptionListModel> OptionList { get; set; } = new List<OptionListModel>();

        // 下拉
        public List<SelectListItem> OptionDDL { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusDDL { get; set; } = new List<SelectListItem>();
        
    }

    public class OptionListModel
    {
        // 選項名稱
        public string OptionName { get; set; }

        // 狀態
        public string OptionStatus { get; set; }

        // 備註
        public string Remark { get; set; }

        // 排序
        public int Sort { get; set; }
    }
}
