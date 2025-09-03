using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;

namespace EquipCheck.Controllers
{
    public class FormManagementController : Controller
    {
        private readonly CommonService _CommonService;

        public FormManagementController(CommonService commonService)
        {
            _CommonService = commonService;
        }

        public async Task<IActionResult> List()
        {
            return View();
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }
    }
}
