using EquipCheck.Models.DB;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquipCheck.Models.ViewModels
{
    public class VM_ActionLog
    {
        public List<ActionlogListModel> ActionlogList { get; set; } = new List<ActionlogListModel>();

        // 查詢
    }

    public class ActionlogListModel
    {
        // 操作日期
        public string OperateDate { get; set; }

        //事件
        public string? Event { get; set; }

        //操作人員
        public string OperateUser { get; set; }

        // 成功與否
        public string IsSuccess { get; set; }

        // 訊息
        public string Message { get; set; }

        // 登入ip
        public string IP { get; set; }

        // 操作日期
        public DateTime CreateDate { get; set; }
    }
}
