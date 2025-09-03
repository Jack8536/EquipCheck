using Microsoft.AspNetCore.Mvc;
using EquipCheck.Models.DB;
using EquipCheck.Models;

namespace EquipCheck.Services
{
    public class LoginService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;

        public LoginService(DBContext dBContext, CommonService commonService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
        }

        /// <summary>
        /// 驗證使用者
        /// </summary>
        /// <param name="user">使用者資訊</param>        
        /// <returns>驗證成功返回true</returns>
        public async Task<BaseModel<string>> IsValidUser(Users user)
        {
            var result = new BaseModel<string>();
            try 
            {                
                //var User = _dbcontext.Users.FirstOrDefault(u => u.UserAccount == user.UserAccount && u.Password == _CommonService.Encrypt(user.Password) && u.Status == 1);
                var User = _dbcontext.Users.FirstOrDefault(u => u.UserAccount == user.UserAccount && u.Password == user.Password && u.Status == 1);
                if (User != null)
                {
                    result.Success = true;
                    result.Message = "登入成功";
                }
                else
                {
                    result.Success = false;
                    result.Message = "帳號或密碼錯誤";
                }
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
