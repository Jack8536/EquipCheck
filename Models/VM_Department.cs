using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class VM_Department
    {
        // 部門id
        public Guid DepartmentUID { get; set; }
        // 部門名稱
        public string DepartmentName { get; set; }

        // 部門狀態
        public string DepartmentStatus { get; set; }

        // 部門成員
        public List<string> Members { get; set; } = new List<string>();

        // 新增用      
        public Guid Manager { get; set; }

        public List<Guid> AddMembers { get; set; } = new List<Guid>();

        // 部門清單
        public List<DeptListModel> DeptLists { get; set; } = new List<DeptListModel>();

        // 下拉選單
        public List<SelectListItem> ManagerDDL { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MemberDDL { get; set; } = new List<SelectListItem>();

        // 搜尋
        public string keyword { get; set; }
    }

    public class DeptListModel
    {
        // 部門id
        public Guid DepartmentUID { get; set; }
        // 部門名稱
        public string DepartmentName { get; set; }

        // 部門狀態
        public string DepartmentStatus { get; set; }

        // 部門主管
        public string Manager { get; set; }

        // 部門成員們
        public List<string> Members { get; set; } = new List<string>();
    }
}