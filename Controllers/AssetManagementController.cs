using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace EquipCheck.Controllers
{
    public class AssetManagementController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly AssetManagementService _assetManagementService;        

        public AssetManagementController(CommonService commonService, AssetManagementService assetManagementService)
        {
            _CommonService = commonService;
            _assetManagementService = assetManagementService;
        }

        public async Task<IActionResult> List()
        {
            VM_Asset model = new VM_Asset();

            // 取得資產列表
            var assetList = await _assetManagementService.GetAssetList();
            if (assetList.Success)
            {
                model.AssetList = assetList.Data;
            }
            else
            {
                model.AssetList = new List<AssetListModel>();
            }

            // 下拉選單
            var AllOMDetail = _CommonService.GetOMDetail(1);
            model.CategoryDDL = AllOMDetail.Select(x => new SelectListItem
            {
                Text = x.DetailName,
                Value = x.Omduid.ToString()
            }).ToList();

            var AllUser = _CommonService.GetUserInfoList();
            model.BuyerDDL = AllUser.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.UserUid.ToString()
            }).ToList();

            model.Status = 1; // 預設啟用
            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VM_Asset model)
        {
            var result = await _assetManagementService.AddAsset(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            VM_Asset model = new VM_Asset();
            // 取得資產
            var asset = await _assetManagementService.GetAsset(id);

            if (asset.Success)
            {
                model.Asset = asset.Data;
            }
            else
            {
                model.AssetList = new List<AssetListModel>();
            }

            // 下拉選單
            var AllOMDetail = _CommonService.GetOMDetail(1);
            model.CategoryDDL = AllOMDetail.Select(x => new SelectListItem
            {
                Text = x.DetailName,
                Value = x.Omduid.ToString()
            }).ToList();

            var AllUser = _CommonService.GetUserInfoList();
            model.BuyerDDL = AllUser.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.UserUid.ToString()
            }).ToList();

            model.StatusDDL = new List<SelectListItem>
            {
                new SelectListItem { Text = "停用", Value = "0" },
                new SelectListItem { Text = "啟用", Value = "1" }
            };
            return View(model);
        }
    }
}
