using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace EquipCheck.Services
{
    public class AssetManagementService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public AssetManagementService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }


        /// <summary>
        /// 取得資產列表
        /// </summary>
        /// <returns>回傳資產列表</returns>
        public async Task<BaseModel<List<AssetListModel>>> GetAssetList()
        {
            var result = new BaseModel<List<AssetListModel>>();

            try
            {
                var Assets = (from asset in _dbcontext.AssetsManagements.Where(x => x.Status == 1 && x.IsDeleted != true)
                              join user in _dbcontext.Users on asset.UserUid equals user.UserUid into user0
                              from user in user0.DefaultIfEmpty()
                              join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid into dept0
                              from dept in dept0.DefaultIfEmpty()
                              join om in _dbcontext.OmDetail on asset.Omduid equals om.Omduid
                              select new AssetListModel
                              {
                                  AssetId = asset.AssetUid,
                                  AssetName = asset.AssetName,
                                  AssetTag = asset.AssetCode,
                                  Category = om.DetailName,
                                  PurchaseDate = asset.PurchaseDate.ToString("yyyy/MM/dd"),
                                  UserName = user.UserName,
                                  Status = _enumService.GetDisplayNameByValue<Status>(asset.Status),
                              }).ToList();

                result.Success = true;
                result.Data = Assets;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }


        /// <summary>
        /// 新增資產
        /// </summary>
        /// <param name="model">資產資料</param>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> AddAsset(VM_Asset model)
        {
            var result = new BaseModel<string>();
            var CurrentUser = _CommonService.GetCurrentUser();
            try
            {
                #region 檢查必填欄位
                if (string.IsNullOrEmpty(model.AssetName) ||
                    model.CategoryId == Guid.Empty ||
                    model.BuyerId == Guid.Empty ||
                    model.PurchaseDate == default(DateTime)
                )
                {
                    result.Success = false;
                    result.Message = "有必填欄位沒填寫";
                    return result;
                }

                // 檢查資產編號是否重複
                //var IsExist = await _dbcontext.AssetsManagements
                //    .Where(x => x.AssetCode == model.AssetCode)
                //    .FirstOrDefaultAsync();
                //if (IsExist != null)
                //{
                //    result.Success = false;
                //    result.Message = "資產編號重複";
                //    return result;
                //}
                #endregion

                var maxCode = _dbcontext.AssetsManagements.Where(a => a.AssetCode.StartsWith("TL"))
                                                            .OrderByDescending(a => a.AssetCode)
                                                            .Select(a => a.AssetCode)
                                                            .FirstOrDefault();

                // 如果資料庫裡還沒有，就從 0 開始
                int nextNumber = 1;
                if (!string.IsNullOrEmpty(maxCode))
                {
                    // 取後 6 碼轉成 int
                    var numberPart = maxCode.Substring(2); // "000001"
                    if (int.TryParse(numberPart, out int num))
                    {
                        nextNumber = num + 1;
                    }
                }
                
                // 建立新資產資料
                var newAsset = new AssetsManagements
                {
                    AssetUid = Guid.NewGuid(),
                    AssetCode = $"TL{nextNumber:D6}",
                    AssetName = model.AssetName,
                    Omduid = model.CategoryId,  // 資產類別
                    PurchaseDate = model.PurchaseDate,
                    UserUid = model.BuyerId,   // 採購人
                    Status = model.Status,       // 狀態
                    CreateUser = CurrentUser,
                    CreatedDate = DateTime.Now,
                    ModifyUser = null,
                    ModifyDate = null
                };

                // 交易
                using var transaction = await _dbcontext.Database.BeginTransactionAsync();
                await _dbcontext.AssetsManagements.AddAsync(newAsset);
                await _dbcontext.SaveChangesAsync();
                await transaction.CommitAsync();

                await _CommonService.WriteActionLog(3, true, CurrentUser, "資產管理"); // 3:新增

                result.Success = true;
                result.Message = "新增成功";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 取得資產
        /// </summary>
        /// <returns>回傳資產</returns>
        public async Task<BaseModel<AssetListModel>> GetAsset(Guid id)
        {
            var result = new BaseModel<AssetListModel>();
            try
            {
                var asset = await (from a in _dbcontext.AssetsManagements
                                   join u in _dbcontext.Users on a.UserUid equals u.UserUid
                                   join om in _dbcontext.OmDetail on a.Omduid equals om.Omduid
                                   where a.AssetUid == id
                                   select new AssetListModel
                                   {
                                       AssetTag = a.AssetCode,
                                       AssetName = a.AssetName,
                                       CategoryId = a.Omduid,
                                       Category = om.DetailName,
                                       UserId = a.UserUid,
                                       _PurchaseDate = a.PurchaseDate,
                                       _Status = a.Status,
                                   }).FirstOrDefaultAsync();

                result.Success = true;
                result.Data = asset;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }
    }
}
