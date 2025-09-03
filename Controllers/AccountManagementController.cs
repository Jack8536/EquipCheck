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
            AccountViewModel model = new AccountViewModel();
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
        public async Task<IActionResult> List(AccountViewModel model)
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
            var model = new AccountViewModel();

            var AllDepartment = _CommonService.GetDepartmentList();

            model.DepartmentDDL = AllDepartment.Select(x => new SelectListItem
            {
                Text = x.DepartmentName,
                Value = x.DepartmentUid.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] AccountViewModel model)
        {
            var result = await _accountService.CreateAccount(model);
            return Json(result);
        }
    }
}
