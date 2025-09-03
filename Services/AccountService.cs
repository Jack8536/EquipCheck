using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EquipCheck.Services
{
    public class AccountService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public AccountService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得帳號清單
        /// </summary>
        /// <returns>回傳帳號清單</returns>
        public async Task<BaseModel<List<AccountListModel>>> GetUserList(AccountViewModel model)
        {
            var result = new BaseModel<List<AccountListModel>>();

            try
            {
                var userList = (from user in _dbcontext.Users
                                join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                                select new AccountListModel
                                {
                                    MemberName = user.UserName,
                                    MemberAccount = user.UserAccount,
                                    DepartmentName = dept.DepartmentName,
                                    RoleName = _enumService.GetDisplayNameByValue<Auth>(user.Role),
                                    Status = _enumService.GetDisplayNameByValue<Status>(user.Status)
                                }).OrderBy(x => x.MemberName).ToList();

                // 查詢
                if (!string.IsNullOrEmpty(model.keyword))
                {
                    userList = userList.Where(x => x.MemberName.Contains(model.keyword)).ToList();
                }
                if (model.SearchDept != null)
                {
                    var dept = _dbcontext.Departments.Where(x => x.DepartmentUid == model.DepartmentUID).FirstOrDefault();
                    if (dept != null)
                    {
                        userList = userList.Where(x => x.DepartmentName == dept.DepartmentName).ToList();
                    }                        
                }
                if (!string.IsNullOrEmpty(model.SearchStatus))
                {
                    userList = userList.Where(x => x.Status == (model.SearchStatus == "1" ? "啟用" : "停用")).ToList();
                }
                

                result.Success = true;
                result.Message = "查詢成功";
                result.Data = userList;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();                
            }

            return result;
        }

        /// <summary>
        /// 新增員工
        /// </summary>
        /// <param name="model">員工資料</param>
        /// <param name="userName">建立者帳號</param>
        /// <returns>建立結果</returns>
        public async Task<BaseModel<string>> CreateAccount(AccountViewModel model)
        {
            var result = new BaseModel<string>();
            var CurrentUser = _CommonService.GetCurrentUser();
            try
            {
                #region 檢查必填寫
                if (string.IsNullOrEmpty(model.MemberName) || string.IsNullOrEmpty(model.MemberAccount)
                    || string.IsNullOrEmpty(model.DepartmentUID.ToString()) || string.IsNullOrEmpty(model.Role.ToString())
                    || string.IsNullOrEmpty(model.Tel))
                {
                    result.Success = false;
                    result.Message = "有必填欄位沒填寫";
                    return result;
                }
                // 員工帳號不能重複
                var IsExist = _dbcontext.Users.Where(x => x.UserAccount == model.MemberAccount).FirstOrDefault();
                if (IsExist != null)
                {
                    result.Success = false;
                    result.Message = "帳號重複";
                    return result;
                }
                #endregion

                // 建立新員工資料
                var newUser = new Users
                {
                    UserUid = Guid.NewGuid(),
                    UserName = model.MemberName,
                    UserAccount = model.MemberAccount,
                    Password = _CommonService.HashPassword("ISCOM" + model.MemberAccount), // 預設密碼=ISCOM+帳號
                    DepartmentUid = model.DepartmentUID,
                    Tel = model.Tel,
                    Role = model.Role,
                    Remark = model.Remark,
                    Status = 1, // 預設啟用
                    CreateUser = CurrentUser,
                    CreateDate = DateTime.Now,
                    ModifyUser = null,
                    ModifyDate = null
                };

                // 交易
                using var transaction = await _dbcontext.Database.BeginTransactionAsync();
                _dbcontext.Users.Add(newUser);
                _dbcontext.SaveChanges();
                await transaction.CommitAsync();
                _CommonService.WriteActionLog(3, true, CurrentUser); // 3:新增
                result.Success = true;
                result.Message = "新增成功";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
                return result;
            }
        }
    }
}
