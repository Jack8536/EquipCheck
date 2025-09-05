using Microsoft.AspNetCore.Mvc;
using EquipCheck.Models.DB;
using EquipCheck.Services;
using System.Threading.Tasks;

namespace EquipCheck.Controllers
{
    public class LoginController : Controller
    {

        private readonly LoginService _LoginService;
        private readonly CommonService _CommonService;

        public LoginController(LoginService loginService, CommonService commonService)
        {
            _LoginService = loginService;
            _CommonService = commonService;
        }

        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登入
        /// </summary>   
        [HttpPost]
        public async Task<IActionResult> Login(Users user)
        {
            var isValid = await _LoginService.IsValidUser(user);

            if (isValid.Success)
            {
                // 設置Session
                var userInfo = _CommonService.GetUserInfo(user.UserAccount);
                if (userInfo != null)
                {
                    HttpContext.Session.SetString("UserUid", userInfo.UserUid.ToString());
                    HttpContext.Session.SetString("UserName", userInfo.UserName);
                    _CommonService.WriteActionLog(1, true, userInfo.UserUid);
                }              
                return Json(isValid);
                //return RedirectToAction("List", "PersonalForm");
            }
            else
            {
                _CommonService.WriteActionLog(1, false, Guid.Empty);
                
            }
            return Json(isValid);
        }

        public async Task<IActionResult> Logout()
        {
            var userUidString = HttpContext.Session.GetString("UserUid");
            Guid userUid = Guid.Empty;
            if (!string.IsNullOrEmpty(userUidString))
            {
                userUid = Guid.Parse(userUidString);
            }
            // 清除Session
            HttpContext.Session.Clear();
            _CommonService.WriteActionLog(2, true, userUid);
            return RedirectToAction("Login", "Login");
        }
        
    }
}
