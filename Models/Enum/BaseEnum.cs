using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EquipCheck.Models.Enum
{
    /// <summary>
    /// 操作動作
    /// </summary>
    public enum OperationAction
    {
        [Display(Name = "登入")]
        [EnumMember(Value = "1")]
        Login = 1,
        [Display(Name = "登出")]
        [EnumMember(Value = "2")]
        Logout = 2,
        [Display(Name = "新增")]
        [EnumMember(Value = "3")]
        Create = 3,
        [Display(Name = "編輯")]
        [EnumMember(Value = "4")]
        Edit = 4,
        [Display(Name = "刪除")]
        [EnumMember(Value = "5")]
        Delete = 5,
        [Display(Name = "上傳")]
        [EnumMember(Value = "6")]
        Upload = 6,
        [Display(Name = "下載")]
        [EnumMember(Value = "7")]
        Download = 7,
        [Display(Name = "匯出")]
        [EnumMember(Value = "8")]
        Export = 8,
        [Display(Name = "匯入")]
        [EnumMember(Value = "9")]
        Import = 9,
        [Display(Name = "檢視")]
        [EnumMember(Value = "0")]
        View = 0
    }

    public enum Auth
    {
        [Display(Name = "一般使用者")]
        [EnumMember(Value = "0")]
        CommonUser = 0,

        [Display(Name = "管理者")]
        [EnumMember(Value = "1")]
        admin = 1,

        [Display(Name = "超級管理者")]
        [EnumMember(Value = "2")]
        SuperAdmin = 2,
    }

    public enum Status
    {
        [Display(Name = "停用")]
        [EnumMember(Value = "0")]
        CommonUser = 0,

        [Display(Name = "啟用")]
        [EnumMember(Value = "1")]
        admin = 1,
    }

    public enum Option
    {
        [Display(Name = "資產類型")]
        [EnumMember(Value = "1")]
        assets = 1,
    }
}
