using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Controllers
{
    public class OptionManagementController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly OptionManagementService _optionManagementService;

        public OptionManagementController(CommonService commonService, OptionManagementService optionManagementService)
        {
            _CommonService = commonService;
            _optionManagementService = optionManagementService;
        }

        public async Task<IActionResult> List()
        {
            VM_Option model = new VM_Option();

            var result = await _optionManagementService.GetOptionList();
            if (result.Success)
            {
                model.OptionList = result.Data;
            }
            else
            {
                model.OptionList = new List<OptionListModel>();
            }

            // 下拉
            var AllOption = _CommonService.GetOption();
            model.OptionDDL = AllOption.Select(x => new SelectListItem
            {
                Text = x.OptionName,
                Value = x.Omuid.ToString()
            }).ToList();

            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" }
            };

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }
    }
}
