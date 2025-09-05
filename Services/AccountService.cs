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
        /// 取得使用者
        /// </summary>
        /// <returns>回傳使用者</returns>
        public async Task<BaseModel<VM_Account>> GetUser(Guid MemberUID)
        {
            BaseModel<VM_Account> result = new BaseModel<VM_Account>();

            try
            {
                var User = (from user in _dbcontext.Users.Where(x => x.UserUid == MemberUID)
                            join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                            select new VM_Account
                            {
                                MemberUID = user.UserUid,
                                MemberName = user.UserName,
                                MemberAccount = user.UserAccount,
                                DepartmentName = dept.DepartmentName,
                                DepartmentUID = dept.DepartmentUid,
                                RoleName = _enumService.GetDisplayNameByValue<Auth>(user.Role),
                                Status = user.Status,
                                StatusName = _enumService.GetDisplayNameByValue<Status>(user.Status),
                                Tel = user.Tel,
                                Role = user.Role,
                                SignaturePath = user.SignaturePath
                            }).FirstOrDefault();

                result.Success = true;
                result.Data = User;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 取得帳號清單
        /// </summary>
        /// <param name="model"></param>
        /// <returns>回傳帳號清單</returns>
        public async Task<BaseModel<List<AccountListModel>>> GetUserList(VM_Account model)
        {
            var result = new BaseModel<List<AccountListModel>>();

            try
            {
                var userList = (from user in _dbcontext.Users
                                join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                                select new AccountListModel
                                {
                                    MemberUID = user.UserUid,
                                    MemberName = user.UserName,
                                    MemberAccount = user.UserAccount,
                                    DepartmentName = dept.DepartmentName,
                                    RoleName = _enumService.GetDisplayNameByValue<Auth>(user.Role),
                                    StatusName = _enumService.GetDisplayNameByValue<Status>(user.Status),
                                    Role = user.Role,
                                    Status = user.Status
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
                    userList = userList.Where(x => x.StatusName == (model.SearchStatus == "1" ? "啟用" : "停用")).ToList();
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
        public async Task<BaseModel<string>> CreateAccount(VM_Account model)
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

                // 檔案上傳
                var filePath = _CommonService.UploadFile(model.signatureFile);
                if (!filePath.Success)
                {
                    result.Success = false;
                    result.Message = filePath.Message;
                    return result;
                }

                // 交易                
                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
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
                        SignaturePath = filePath.Data,
                        Status = 1, // 預設啟用
                        CreateUser = CurrentUser,
                        CreateDate = DateTime.Now,
                        ModifyUser = null,
                        ModifyDate = null
                    };

                    _dbcontext.Users.Add(newUser);
                    _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _CommonService.WriteActionLog(3, true, CurrentUser); // 3:新增

                    result.Success = true;
                    result.Message = "新增成功";
                }                
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();                
            }

            return result;
        }

        /// <summary>
        /// 編輯員工資料
        /// </summary>
        /// <param name="model">員工資料</param>
        /// <returns>編輯結果</returns>
        public async Task<BaseModel<string>> ModifyUser(VM_Account model)
        {
            var result = new BaseModel<string>();
            var currentUser = _CommonService.GetCurrentUser();

            #region 檢查必填寫
            if (string.IsNullOrEmpty(model.DepartmentUID.ToString()) || string.IsNullOrEmpty(model.Role.ToString()) || string.IsNullOrEmpty(model.Tel))
            {
                result.Success = false;
                result.Message = "有必填欄位沒填寫";
                return result;
            }
            #endregion

            try
            {

                var user = _dbcontext.Users.Where(x => x.UserUid == model.MemberUID).FirstOrDefault();

                // 檔案上傳
                var filePath = _CommonService.UploadFile(model.signatureFile);

                if (!filePath.Success)
                {
                    result.Success = false;
                    result.Message = filePath.Message;
                    return result;
                }

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {                    
                    // 更新欄位                
                    user.DepartmentUid = model.DepartmentUID;
                    user.Role = model.Role;
                    user.Status = model.Status;
                    user.Tel = model.Tel;
                    user.Remark = model.Remark;
                    user.SignaturePath = filePath.Data;
                    user.ModifyUser = currentUser;
                    user.ModifyDate = DateTime.Now;

                    _dbcontext.Users.Update(user);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _CommonService.WriteActionLog(4, true, currentUser, "帳號管理"); // 4:編輯
                    result.Success = true;
                    result.Message = "更新成功";
                }
            }
            catch (Exception ex)
            {
                _CommonService.WriteActionLog(4, false, currentUser, "帳號管理"); // 4:編輯
                result.Success = false;
                result.Message = ex.ToString();               
            }
            return result;
        }

        /// <summary>
        /// 停用帳號
        /// </summary>
        /// <param name="MemberUID">員工UID</param>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> DisableAccount(Guid MemberUID)
        {
            var result = new BaseModel<string>();
            var currentUser = _CommonService.GetCurrentUser();

            try
            {
                
                var user = _dbcontext.Users.Where(x => x.UserUid == MemberUID).FirstOrDefault();

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    // 更新使用者狀態
                    user.Status = 0; // 停用
                    user.ModifyUser = currentUser;
                    user.ModifyDate = DateTime.Now;

                    _dbcontext.Users.Update(user);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 寫入操作日誌
                    await _CommonService.WriteActionLog(4, true, currentUser, "帳號管理");

                    result.Success = true;
                    result.Message = "停用成功";
                }                   
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }


        /// <summary>
        /// 刪除帳號
        /// </summary>
        /// <param name="MemberUID">員工UID</param>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> DeleteAccount(Guid MemberUID)
        {
            var result = new BaseModel<string>();
            var currentUser = _CommonService.GetCurrentUser();

            try
            {
                var user = await _dbcontext.Users.FindAsync(MemberUID);
                if (user == null)
                {
                    result.Success = false;
                    result.Message = "找不到使用者";
                    return result;
                }

                // 檢查是否為部門主管
                var isManager = _dbcontext.Departments.Any(x => x.ManagerUid == MemberUID);

                if (isManager)
                {
                    result.Success = false;
                    result.Message = "此帳號為部門主管，不可刪除";
                    return result;
                }

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 刪除使用者
                        _dbcontext.Users.Remove(user);
                        await _dbcontext.SaveChangesAsync();

                        // 提交交易
                        await transaction.CommitAsync();

                        // 寫入操作日誌
                        await _CommonService.WriteActionLog(5, true, currentUser, "刪除帳號");

                        result.Success = true;
                        result.Message = "刪除成功";
                    }
                    catch
                    {
                        // 發生錯誤時回滾交易
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // 寫入失敗日誌
                await _CommonService.WriteActionLog(5, false, currentUser, "刪除帳號");
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }
    }
}
