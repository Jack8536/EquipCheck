using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using EquipCheck.Services;

namespace EquipCheck.Controllers
{
    public class PersonalFormController :Controller
    {
        private readonly CommonService _CommonService;        

        public PersonalFormController(CommonService commonService)
        {            
            _CommonService = commonService;
        }

        public async Task<IActionResult> List()
        {
            return View();
        }
    }
}
