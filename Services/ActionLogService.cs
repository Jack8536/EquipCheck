using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EquipCheck.Services
{
    public class ActionLogService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;
        private readonly ActionLogService _actionLogService;

        public ActionLogService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得操作日誌
        /// </summary>
        /// <returns>回傳操作日誌</returns>
        public async Task<BaseModel<List<ActionlogListModel>>> GetActionLogList()
        {
            var result = new BaseModel<List<ActionlogListModel>>();

            try
            {
                var logs = (from log in _dbcontext.ActionLogs
                            join user in _dbcontext.Users on log.CreateUser equals user.UserUid
                            select new ActionlogListModel
                            {
                                OperateDate = log.CreatedDate.ToString("yyyy/MM/dd HH:mm:ss"),
                                Event = log.Event,
                                OperateUser = user.UserName,
                                Message = _enumService.GetDisplayNameByValue<OperationAction>(log.Action) + '-' + (log.IsSuccess ? "成功" : "失敗"),
                                IP = log.Ip,
                                CreateDate = log.CreatedDate
                            }).OrderBy(x => x.CreateDate).ToList();

                result.Success = true;
                result.Message = "查詢成功";
                result.Data = logs;
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
