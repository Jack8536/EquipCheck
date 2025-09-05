using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquipCheck.Services
{
    public class OptionManagementService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public OptionManagementService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得選項清單
        /// </summary>
        /// <returns>回傳選項</returns>
        public async Task<BaseModel<List<OptionListModel>>> GetOptionList()
        {
            var result = new BaseModel<List<OptionListModel>>();

            try
            {
                // 選項類別，預設為1 -> 資產類別
                var OptionList = (from om in _dbcontext.OptionManagements.Where(x => x.Category == 1)
                                  join omd in _dbcontext.OmDetail on om.Omuid equals omd.Omuid
                                  select new OptionListModel
                                  {
                                      OptionName = omd.DetailName,
                                      OptionStatus = omd.Status == 1 ? "啟用" : "停用",
                                      Remark = omd.Remark,
                                      Sort = omd.Sort
                                  }).OrderByDescending(x => x.Sort).ToList();

                result.Success = true;
                result.Data = OptionList;
            }
            catch (Exception ex)
            {
                result.Success = true;
                result.Message = ex.ToString();
            }

            return result;
        }


        /// <summary>
        /// 新增選項
        /// </summary>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> CreateDetail(VM_Option model)
        {
            var result = new BaseModel<string>();
            var currentusaer = _CommonService.GetCurrentUser();

            try
            {
                // 1. 驗證必填欄位
                if (model.Omuid == Guid.Empty || string.IsNullOrEmpty(model.OptionName))
                {
                    result.Success = false;
                    result.Message = "請填寫必填欄位";
                    return result;
                }

                // 2. 檢查選項名稱是否重複
                var isDuplicate = _dbcontext.OmDetail.Any(x => x.Omuid == model.Omuid && x.DetailName == model.OptionName);

                if (isDuplicate)
                {
                    result.Success = false;
                    result.Message = "選項名稱重複";
                    return result;
                }

                // 最大排序(todo)
                var maxSort = _dbcontext.OmDetail.Where(x => x.Omuid == model.Omuid).Select(x => x.Sort).Max();

                // 建立新的選項明細
                var newDetail = new OmDetail
                {
                    Omduid = Guid.NewGuid(),
                    Omuid = model.Omuid,
                    DetailName = model.OptionName,
                    Status = model.Status,
                    Sort = maxSort + 1,
                    Remark = model.Remark,
                    CreateUser = currentusaer,
                    CreatedDate = DateTime.Now
                };

                // 5. 新增資料到資料庫
                _dbcontext.OmDetail.Add(newDetail);
                _dbcontext.SaveChangesAsync();

                // 6. 寫入操作日誌
                _CommonService.WriteActionLog(3, true, currentusaer, "選項管理");

                result.Success = true;
                result.Message = "新增成功";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.ToString();
            }

            return result;
        }
    }
}
