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
        /// 取得選項
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
    }
}
