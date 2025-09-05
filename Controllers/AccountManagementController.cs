using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly AccountService _accountService;

        public AccountManagementController(CommonService commonService, AccountService accountService)
        {
            _CommonService = commonService;
            _accountService = accountService;
        }

        public async Task<IActionResult> List()
        {
            VM_Account model = new VM_Account();
            var UserList = await _accountService.GetUserList(model);
            if (UserList.Success)
            {
                model.UserList = UserList.Data;
            }
            else
            {
                model.UserList = new List<AccountListModel>();
            }

            var AllDepartment = _CommonService.GetDepartmentList();

            model.DepartmentDDL = AllDepartment.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentUid.ToString()
            }).ToList();

            model.Status = -1; // 為了下拉選單初始化，不然預設值都是0，會選擇"停用"
            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> List(VM_Account model)
        {
            var UserList = await _accountService.GetUserList(model);
            if (UserList.Success)
            {
                model.UserList = UserList.Data;
            }
            else
            {
                model.UserList = new List<AccountListModel>();
            }
            var AllDepartment = _CommonService.GetDepartmentList();
            model.DepartmentDDL = AllDepartment.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentUid.ToString()
            }).ToList();

            model.Status = -1; // 為了下拉選單初始化，不然預設值都是0，會選擇"停用"
            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" }
            };
            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var model = new VM_Account();

            var AllDepartment = _CommonService.GetDepartmentList();

            // 下拉選單
            model.DepartmentDDL = AllDepartment.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentUid.ToString()
            }).ToList();

            model.Role = -1;// 為了下拉選單初始化，不然預設值都是0，會選擇"一般使用者"
            model.RoleDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "一般使用者", Value = "0" },
                new SelectListItem { Text = "管理者", Value = "1" }

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] VM_Account model)
        {
            var result = await _accountService.CreateAccount(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = new VM_Account();

            var User = await _accountService.GetUser(id);
            if (User.Success)
            {
                model = User.Data;
            }
            else
            {
                model = new VM_Account();
            }

            // 部門下拉選單資料
            var AllDepartment = _CommonService.GetDepartmentList();
            model.DepartmentDDL = AllDepartment.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentUid.ToString(),
                Selected = x.DepartmentUid == model.DepartmentUID
            }).ToList();

            // 角色下拉選單
            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" },
            };

            // 身分下拉選單
            model.RoleDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "一般使用者", Value = "0" },
                new SelectListItem { Text = "管理者", Value = "1" }

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VM_Account model)
        {
            var result = await _accountService.ModifyUser(model);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DisableAccount(Guid MemberUID)
        {
            var result = await _accountService.DisableAccount(MemberUID);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(Guid MemberUID)
        {
            var result = await _accountService.DeleteAccount(MemberUID);
            return Json(result);
        }
    }
}
