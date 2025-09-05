using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EquipCheck.Models.ViewModels.VM_PersonalForm;

namespace EquipCheck.Controllers
{
    public class PersonalFormController :Controller
    {
        private readonly CommonService _CommonService;
        private readonly PersonalFormService _personalFormService;
        

        public PersonalFormController(CommonService commonService, PersonalFormService personalFormService)
        {            
            _CommonService = commonService;
            _personalFormService = personalFormService;
        }

        public async Task<IActionResult> List()
        {
            VM_PersonalForm model = new VM_PersonalForm();

            var result = await _personalFormService.GetEnableFormList(model);

            if (result.Success)
            {
                model.FormList = result.Data;
            }
            else
            {
                model.FormList = new List<EnableFormListModel>();
            }

            return View(model);            
        }

        [HttpPost]
        public async Task<IActionResult> List(VM_PersonalForm model)
        {
            VM_PersonalForm data = new VM_PersonalForm();

            var result = await _personalFormService.GetEnableFormList(model);

            if (result.Success)
            {
                data.FormList = result.Data;
            }
            else
            {
                data.FormList = new List<EnableFormListModel>();
            }

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            VM_PersonalForm model = new VM_PersonalForm();
            var result = await _personalFormService.GetFullForm(id);

            var AllAsset = _CommonService.GetAssetList();
            
            model.AssetsDDL = AllAsset.Select(x => new SelectListItem
            {
                Text = x.AssetCode,
                Value = x.AssetUid.ToString()
            }).ToList();

            if (result.Success)
            {
                model.FullForm = result.Data;
            }
            else
            {
                model.FullForm = new Form();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VM_PersonalForm model)
        {
            // 遞交表單作答
            var result = await _personalFormService.SaveForm(model);
            return Json(result);            
        }
    }
}
