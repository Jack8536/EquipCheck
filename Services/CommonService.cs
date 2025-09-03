using EquipCheck.Models.DB;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EquipCheck.Services
{
    public class CommonService
    {
        private readonly DBContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly string _encryptionKey = "Yq#8536$GteckTey65";

        public CommonService(DBContext dbcontext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbcontext;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 寫入操作日誌
        /// </summary>
        /// <param name="userId">操作者ID</param>
        /// <param name="action">執行動作(l:登入 2:登出 3:新增 4:編輯 5:刪除 6:上傳 7:下載 8:匯出 9:匯入 0:檢視)</param>
        /// <param name="message">操作訊息</param>
        /// <returns>寫入成功返回true</returns>
        public bool WriteActionLog(int action, bool IsSuccess, Guid User, string EventMsg = null)
        {
            try
            {
                var clientIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                var log = new ActionLogs
                {                    
                    Action = action,
                    Ip = clientIp,
                    IsSuccess = IsSuccess,
                    Event = EventMsg,
                    CreateUser = User,
                    CreatedDate = DateTime.Now
                };

                _dbContext.ActionLogs.Add(log);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // 這裡可以添加異常處理邏輯
                return false;
            }
        }

        /// <summary>
        /// 取得所有啟用使用者資訊
        /// </summary>
        /// <returns>返回所有使用者資訊</returns>
        public List<Users> GetUserInfoList()
        {
            try
            {
                return _dbContext.Users.Where(u => u.Status == 1).ToList();
            }
            catch (Exception ex)
            {
                return new List<Users>();
            }            
        }


        /// <summary>
        /// 取得所有部門
        /// </summary>
        /// <returns>返回部門資訊</returns>
        public List<Departments> GetDepartmentList()
        {
            try
            {
                return _dbContext.Departments.Where(u => u.Status == 1).ToList();
            }
            catch (Exception ex)
            {
                return new List<Departments>();
            }
        }

        /// <summary>
        /// 取得所有類型
        /// </summary>
        /// <returns>返回部門資訊</returns>
        public List<OptionManagements> GetOption()
        {
            try
            {
                return _dbContext.OptionManagements.Where(x => x.Status == 1).ToList();
            }
            catch (Exception ex)
            {
                return new List<OptionManagements>();
            }
        }

        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <returns>返回使用者資訊，如果找不到則返回 null</returns>
        public Users? GetUserInfo(string Account)
        {
            try
            {
                return _dbContext.Users.FirstOrDefault(u => u.UserAccount == Account && u.Status == 1);
            }
            catch (Exception ex)
            {
                // 可以加入日誌記錄
                return null;
            }
        }

        /// <summary>
        /// 取得目前使用者資訊
        /// </summary>        
        /// <returns>返回目前使用者資訊</returns>
        public Guid GetCurrentUser()
        {
            var userUidString = _httpContextAccessor.HttpContext?.Session.GetString("UserUid");
            return Guid.TryParse(userUidString, out Guid userUid) ? userUid : Guid.Empty;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>加密後的字串</returns>
        public string Encrypt(string plainText)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    // 使用金鑰的前16位作為 IV
                    aes.IV = keyBytes.Take(16).ToArray();

                    ICryptoTransform encryptor = aes.CreateEncryptor();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加密失敗", ex);
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText">加密後的字串</param>
        /// <returns>解密後的明文</returns>
        public string Decrypt(string cipherText)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    // 使用金鑰的前16位作為 IV
                    aes.IV = keyBytes.Take(16).ToArray();

                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    using (MemoryStream ms = new MemoryStream(cipherBytes))
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解密失敗", ex);
            }
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="password">密碼明文</param>
        /// <returns>SHA256加密後的字串</returns>
        public string HashPassword(string password)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // 將密碼轉換成 byte 陣列
                    byte[] bytes = Encoding.UTF8.GetBytes(password);

                    // 計算雜湊值
                    byte[] hash = sha256.ComputeHash(bytes);

                    // 將 byte 陣列轉換為十六進位字串
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        builder.Append(hash[i].ToString("x2"));
                    }

                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SHA256加密失敗", ex);
            }
        }

    }
}
