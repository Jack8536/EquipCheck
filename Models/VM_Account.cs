using EquipCheck.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class AccountViewModel
    {        
        // 員工名稱
        public string MemberName { get; set; }
  
        // 員工帳號           
        public string MemberAccount { get; set; }

        // 部門id
        public Guid DepartmentUID { get; set; }

        // 部門名稱
        public string DepartmentName { get; set; }

        // 身分
        public int Role { get; set; }

        // 身分名稱
        public string RoleName { get; set; }

        // 分機
        public string Tel { get; set; }

        // 備份
        public string Remark { get; set; }

        // 狀態
        public int Status { get; set; }    
                
        // 成員清單
        public List<AccountListModel> UserList { get; set; } = new List<AccountListModel>();

        // 下拉選單
        public List<SelectListItem> DepartmentDDL { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusDDL { get; set; } = new List<SelectListItem>();

        // 查詢
        public string keyword { get; set; }

        public string SearchDept  { get; set; }

        public string SearchStatus { get; set; }
    }

    public class AccountListModel
    {
        // 員工名稱
        public string MemberName { get; set; }

        // 員工帳號           
        public string MemberAccount { get; set; }
        // 部門名稱
        public string DepartmentName { get; set; }
        // 身分名稱
        public string RoleName { get; set; }
        // 狀態
        public string Status { get; set; }
    }
}