using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;

namespace EquipCheck.Controllers
{
    public class ActionLogController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly ActionLogService _actionLogService;

        public ActionLogController(CommonService commonService, ActionLogService actionLogService)
        {
            _CommonService = commonService;
            _actionLogService = actionLogService;
        }

        public async Task<IActionResult> List()
        {
            VM_ActionLog model = new VM_ActionLog();

            var result = await _actionLogService.GetActionLogList();

            if (result.Success)
            {
                model.ActionlogList = result.Data;
            }
            else
            {
                model.ActionlogList = new List<ActionlogListModel>();
            }

            return View(model);
        }
    }
}
