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
            VM_Department model = new VM_Department();

            var result = await _departmentService.GetDeptList(model);
            if (result.Success)
            {
                model.DeptLists = result.Data;
            }
            else
            {
                model.DeptLists = new List<DeptListModel>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(VM_Department model)
        {
            VM_Department data = new VM_Department();

            var result = await _departmentService.GetDeptList(model);
            if (result.Success)
            {
                data.DeptLists = result.Data;
            }
            else
            {
                data.DeptLists = new List<DeptListModel>();
            }

            return View(data);
        }

        public async Task<IActionResult> Add()
        {
            var model = new VM_Department();

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
        public async Task<IActionResult> Add(VM_Department model)
        {
            var result = await _departmentService.CreateDept(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            var Dept = _CommonService.GetDepartment(id);
            var model = new VM_Department();
            var AllMembers = _CommonService.GetUserInfoList();
            model.DepartmentUID = id;
            model.DepartmentName = Dept?.DepartmentName;
            model.Manager = Dept?.ManagerUid ?? Guid.Empty;
            // 主管下拉
            model.ManagerDDL = AllMembers.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.UserUid.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VM_Department model)
        {
            var result = await _departmentService.EditDept(model);
            return Json(result);
        }
    }
}
