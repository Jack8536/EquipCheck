using Microsoft.AspNetCore.Mvc.Rendering;
using static EquipCheck.Models.ViewModels.VM_PersonalForm;

namespace EquipCheck.Models.ViewModels
{
    public class VM_Form
    {
        // 表單id
        public Guid FormUID { get; set; }

        // 表單名稱
        public string FormName { get; set; }

        // 排序
        public int Sort { get; set; }

        // 年份
        public string Year { get; set; }

        // 填寫時間(起)
        public DateTime StartDate { get; set; }

        // 填寫時間(迄)
        public DateTime EndDate { get; set; }

        // 狀態
        public bool Status { get; set; }

        // 發起人
        public Guid Sponsor { get; set; }
        public List<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();

        public List<FormListModel> FormList { get; set; } = new List<FormListModel>();

        public List<SubmissionFormModel> SubmissionFormList { get; set; } = new List<SubmissionFormModel>();
        
        public FullForm FullForm { get; set; } = new FullForm();
        // 下拉選單
        public List<SelectListItem> SponsorDDL { get; set; } = new List<SelectListItem>();
    }


    public class FormListModel
    {
        // 表單id
        public Guid FormUID { get; set; }
        // 年份
        public string Year { get; set; }

        // 填寫時間(起)
        public string StartDate { get; set; }

        // 填寫時間(迄)
        public string EndDate { get; set; }

        // 表單名稱
        public string FormName { get; set; }

        // 發起人名稱
        public string SponsorName { get; set; }

        // 發布狀態
        public string StatusName { get; set; }
    }


    public class ChecklistItem
    {
        // 檢查項目id
        public Guid ChecklistItemUid { get; set; }

        // 項目名稱
        public string ItemName { get; set; }

        // 項目排序
        public int Sort { get; set; }

        // 是否啟用
        public bool Status { get; set; }
    }

    public class SubmissionFormModel
    {
        //提交表單id
        public Guid SubmissionFormUID { get; set; }
        //表單id
        public Guid FormUID { get; set; }

        // 提交日期
        public string SubmissionDate { get; set; }

        // 填寫人id
        public Guid EmployeeUID { get; set; }
        // 填寫人
        public string EmployeeName { get; set; }

        // 部門
        public string DepartmentName { get; set; }

        // 資產編號
        public string AssetTag { get; set; }

        // 狀態
        public string Status { get; set; }

        // 提交日期
        public string CheckDate { get; set; }
    }

    public class FullForm
    {
        //

        // 員工id
        public Guid EmployeeId { get; set; }
        // 員工姓名
        public string EmployeeName { get; set; }

        // 部門id
        public Guid DepartmentId { get; set; }

        // 部門名稱
        public string DepartmentName { get; set; }

        // 分機
        public string Tel { get; set; }

        // 資產編號
        public string AssetCode { get; set; }

        // 檢查日期
        public DateTime CheckDate { get; set; }

        // 檢查項目
        public List<FormItem> Items { get; set; }

        // 表單ID
        public Guid FormUID { get; set; }
    }

    public class FormItem
    {
        // 表單名稱
        public string ItemName { get; set; }

        // 符不符合
        public int IsChecked { get; set; }

        // 查核紀錄
        public string Remark { get; set; }
    }
}
