using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using EquipCheck.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Controllers
{
    public class DepartmentManagementController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly DepartmentService _departmentService;

        public DepartmentManagementController(CommonService commonService, DepartmentService departmentService)
        {
            _CommonService = commonService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> List()
        {
            DepartmentViewModel model = new DepartmentViewModel();

            var DeptLists = await _departmentService.GetDeptList();
            if (DeptLists.Success)
            {
                model.DeptLists = DeptLists.Data;
            }
            else
            {
                model.DeptLists = new List<DeptListModel>();
            }

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var model = new DepartmentViewModel();

            var AllMembers = _CommonService.GetUserInfoList();

            // 主管下拉
            model.ManagerDDL = AllMembers.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.UserUid.ToString()
            }).ToList();            

            // 員工下拉-過濾已經有部門
            //var NoDeptmembers = AllMembers.Where(x => x.DepartmentUid == null).ToList();
            //model.MemberDDL = NoDeptmembers.Select(x => new SelectListItem
            //{
            //    Text = x.UserName,
            //    Value = x.UserUid.ToString()
            //}).ToList();            

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DepartmentViewModel model)
        {
            var result = await _departmentService.CreateDept(model);
            return Json(result);
        }
    }
}
