using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EquipCheck.Models.ViewModels.VM_PersonalForm;

namespace EquipCheck.Services
{
    public class PersonalFormService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public PersonalFormService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得上架表單清單
        /// </summary>
        /// <returns>回傳表單清單</returns>
        public async Task<BaseModel<List<EnableFormListModel>>> GetEnableFormList(VM_PersonalForm model)
        {
            var result = new BaseModel<List<EnableFormListModel>>();
            try
            {
                var Forms = (from form in _dbcontext.FormsManagements.Where(x => x.Status == 1 || x.Status == 2)
                             join user in _dbcontext.Users on form.Sponsor equals user.UserUid                             
                             join sub in _dbcontext.FormSubmissions on user.UserUid equals sub.UserUid into subJoin
                             from sub in subJoin.DefaultIfEmpty()
                             select new EnableFormListModel
                             {
                                 FormUID = form.FormUid,
                                 FormName = form.FormName,
                                 Year = form.Year,
                                 StartDate = form.PeriodStart.ToString("yyyy/MM/dd"),
                                 EndDate = form.PeriodEnd.ToString("yyyy/MM/dd"),
                                 SponsorName = user.UserName,
                                 StatusName = _enumService.GetDisplayNameByValue<FormStatus>(form.Status),
                                 IsFinished = sub != null ? sub.Status : -1 // -1 尚未填寫
                             }).ToList();

                // 查詢                
                if (!string.IsNullOrEmpty(model.keyword))
                {
                    Forms = Forms.Where(x => x.FormName.Contains(model.keyword)).ToList();
                }

                if (!string.IsNullOrEmpty(model.year))
                {
                    Forms = Forms.Where(x => x.Year.Contains(model.year)).ToList();
                }

                if (model.startDate.HasValue && model.endDate.HasValue)
                {
                    Forms = Forms.Where(x => DateTime.Parse(x.StartDate) >= model.startDate.Value && DateTime.Parse(x.EndDate) <= model.endDate.Value.AddDays(1).AddSeconds(-1)).ToList();
                }

                result.Success = true;
                result.Data = Forms;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }


        /// <summary>
        /// 取得完整表單
        /// </summary>
        /// <returns>回傳完整表單</returns>
        public async Task<BaseModel<Form>> GetFullForm(Guid FormUID)
        {
            var result = new BaseModel<Form>();
            var CurrentUser = _CommonService.GetCurrentUser();

            try
            {
                var Forms = (from form in _dbcontext.FormsManagements.Where(x => x.FormUid == FormUID)
                             join item in _dbcontext.FormChecklistItems on form.FormUid equals item.FormUid into checklistItems
                             join user in _dbcontext.Users on CurrentUser equals user.UserUid
                             join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                             select new Form
                             {
                                 FormUID = form.FormUid,
                                 EmployeeId = user.UserUid,
                                 EmployeeName = user.UserName,
                                 DepartmentId = dept.DepartmentUid,
                                 DepartmentName = dept.DepartmentName,
                                 Tel = user.Tel,
                                 //Items = checklistItems.Select(x => x.ItemName).ToList()
                                 Items = checklistItems.Select(x => new FormSubmissionItem
                                 {
                                     ItemUID = x.ChecklistItemUid,
                                     ItemName = x.ItemName,
                                     Sort = x.Sort
                                 }).OrderBy(x => x.Sort).ToList(),
                             }).FirstOrDefault();

                result.Success = true;
                result.Data = Forms;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 取得已作答表單
        /// </summary>
        /// <returns>回傳完整表單</returns>
        

        /// <summary>
        /// 儲存表單作答答案
        /// </summary>
        /// <param name="FormUID">表單UID</param>
        /// <param name="model">表單資料</param>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<String>> SaveForm(VM_PersonalForm model)
        {
            var result = new BaseModel<string>();
            var CurrentUser = _CommonService.GetCurrentUser();

            try
            {
                //using var transaction = await _dbcontext.Database.BeginTransactionAsync();
                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    var SubmissionUID = Guid.NewGuid();
                    // 建立表單提交記錄
                    var submission = new FormSubmissions
                    {
                        SubmissionUid = SubmissionUID,
                        FormUid = null,
                        AssestUid = model.AssetUID,
                        UserUid = model.FullForm.EmployeeId,
                        DepartmentUid = model.FullForm.DepartmentId,
                        CheckDate = model.CheckDate,
                        Tel = model.FullForm.Tel,
                        Status = 0, // 狀態：待審核
                        CreateUser = CurrentUser,
                        CreatedDate = DateTime.Now
                    };

                    _dbcontext.FormSubmissions.Add(submission);
                    await _dbcontext.SaveChangesAsync();

                    // 表單提交記錄
                    var submissionItems = model.Results.Select(item => new FormSubmissionItems
                    {
                        SubmissionItemUid = Guid.NewGuid(),
                        SubmissionUid = SubmissionUID,
                        ChecklistItemUid = item.ItemUID,
                        IsChecked = item.IsChecked,
                        Remark = item.Remark ?? string.Empty
                    }).ToList();

                    // 建立提交日誌
                    var submissionLog = new FormSubmissionLogs
                    {
                        SubmissionLogUid = Guid.NewGuid(),
                        SubmissionUid = SubmissionUID,
                        UserUid = CurrentUser,
                        ModifyDate = DateTime.Now
                    };

                    // 寫入

                    _dbcontext.FormSubmissionItems.AddRange(submissionItems);
                    _dbcontext.FormSubmissionLogs.Add(submissionLog);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    // 日誌
                    await _CommonService.WriteActionLog(3, true, CurrentUser, "提交表單");
                }
                
                result.Success = true;
                result.Message = "提交成功";
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
