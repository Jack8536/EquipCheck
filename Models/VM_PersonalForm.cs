using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class VM_PersonalForm
    {
        public Guid AssetUID { get; set; }
        public List<EnableFormListModel> FormList { get; set; } = new List<EnableFormListModel>();
        public Form FullForm { get; set; }

        // 檢查日期
        public DateTime CheckDate { get; set; }

        // 接收表單提交結果
        public List<FormResult> Results { get; set; }

        // 新增資產下拉選單
        public List<SelectListItem> AssetsDDL { get; set; }

        // 查詢
        public string keyword { get; set; }

        public string year { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

    }
    public class EnableFormListModel
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

        // 是否已填寫
        public int IsFinished { get; set; }
    }

    public class Form
    {
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
        public List<FormSubmissionItem> Items { get; set; }

        // 表單ID
        public Guid FormUID { get; set; }
    }

    // 檢查項目
    public class FormSubmissionItem
    {
        // 項目id
        public Guid ItemUID { get; set; }
        // 項目名稱
        public string ItemName { get; set; }

        // 排序
        public int Sort { get; set; }
    }

    public class FormResult
    {
        // 檢查項目ID
        public Guid ItemUID { get; set; }

        // 檢查結果 (1:符合, 0:不符合)
        public int IsChecked { get; set; }

        // 查核紀錄
        public string Remark { get; set; }
    }
}
