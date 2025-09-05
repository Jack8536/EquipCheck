using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace EquipCheck.Services
{
    public class FormManagementService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public FormManagementService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得表單清單
        /// </summary>
        /// <returns>回傳表單清單</returns>
        public async Task<BaseModel<List<FormListModel>>> GetFormList()
        {
            var result = new BaseModel<List<FormListModel>>();
            try
            {
                var Forms = (from form in _dbcontext.FormsManagements
                             join user in _dbcontext.Users on form.Sponsor equals user.UserUid
                             select new FormListModel
                             {
                                 FormUID = form.FormUid,
                                 FormName = form.FormName,
                                 Year = form.Year,
                                 StartDate = form.PeriodStart.ToString("yyyy/MM/dd"),
                                 EndDate = form.PeriodEnd.ToString("yyyy/MM/dd"),
                                 SponsorName = user.UserName,
                                 StatusName = _enumService.GetDisplayNameByValue<FormStatus>(form.Status)
                             }).ToList();

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
        /// 儲存表單設計結果
        /// </summary>
        /// <returns>儲存結果</returns>
        public async Task<BaseModel<string>> CreateForm(VM_Form model)
        {
            var result = new BaseModel<string>();

            var CurrentUser = _CommonService.GetCurrentUser();

            try
            {
                var FormUID = Guid.NewGuid();
                // 建立主表單
                var form = new FormsManagements
                {
                    FormUid = FormUID,
                    FormName = model.FormName,
                    Year = model.Year,
                    PeriodStart = model.StartDate,
                    PeriodEnd = model.EndDate,
                    Sponsor = model.Sponsor,
                    Status = 1,
                    CreateUser = CurrentUser,
                    CreatedDate = DateTime.Now,
                };

                // 建立檢查項目
                List<FormChecklistItems> items = new List<FormChecklistItems>();

                if (model.Items != null && model.Items.Any())
                {
                    items = model.Items.Select((item, index) => new FormChecklistItems
                    {
                        ChecklistItemUid = item.ChecklistItemUid,
                        FormUid = FormUID,
                        ItemName = item.ItemName,
                        Sort = index + 1,
                        Status = item.Status == true ? 1 : 0,
                        CreateUser = CurrentUser,
                        CreatedDate = DateTime.Now
                    }).ToList();
                }

                // 交易
                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    await _dbcontext.FormsManagements.AddAsync(form);
                    await _dbcontext.FormChecklistItems.AddRangeAsync(items);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                   
                _CommonService.WriteActionLog(3, true, CurrentUser, "表單管理");

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
        /// 取得使用者已提交表單
        /// </summary>
        /// <returns>返回使用者已提交表單</returns>
        public async Task<BaseModel<List<SubmissionFormModel>>> GetSubmissionForm()
        {
            var result = new BaseModel<List<SubmissionFormModel>>();

            try
            {
                var Subs = (from sub in _dbcontext.FormSubmissions
                            join asset in _dbcontext.AssetsManagements on sub.AssestUid equals asset.AssetUid
                            join user in _dbcontext.Users on sub.UserUid equals user.UserUid
                            join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                            select new SubmissionFormModel
                            {
                                SubmissionFormUID = sub.SubmissionUid,
                                FormUID = sub.FormUid ?? Guid.Empty,
                                SubmissionDate = sub.CreatedDate.ToString("yyyy/MM/dd HH:mm:ss"),
                                EmployeeUID = user.UserUid,
                                EmployeeName = user.UserName,                                
                                DepartmentName = dept.DepartmentName,
                                AssetTag = asset.AssetCode,
                                CheckDate = sub.CheckDate.ToString("yyyy/MM/dd"),
                                Status = _enumService.GetDisplayNameByValue<SubmissionStatus>(sub.Status)
                            }).ToList();

                result.Success = true;
                result.Data = Subs;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }


        /// <summary>
        /// 取得特定使用者作答結果
        /// </summary>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<FullForm>> GetUserSubmissionForm(Guid id)
        {
            var result = new BaseModel<FullForm>();

            try
            {
                var Form = (from sub in _dbcontext.FormSubmissions.Where(x => x.SubmissionUid == id)
                            join user in _dbcontext.Users on sub.UserUid equals user.UserUid
                            join dept in _dbcontext.Departments on user.DepartmentUid equals dept.DepartmentUid
                            join asset in _dbcontext.AssetsManagements on sub.AssestUid equals asset.AssetUid
                            //where sub.UserUid == id && sub.FormUid == Formid
                            select new FullForm
                            {
                                FormSub = id,
                                FormUID = sub.FormUid.Value,
                                EmployeeId = user.UserUid,
                                EmployeeName = user.UserName,
                                DepartmentId = dept.DepartmentUid,
                                DepartmentName = dept.DepartmentName,
                                Tel = user.Tel,
                                AssetCode = asset.AssetCode,
                                CheckDate = sub.CheckDate,
                                // 取得檢查項目及結果
                                Items = _dbcontext.FormSubmissionItems.Where(i => i.SubmissionUid == sub.SubmissionUid)
                                    .Select(i => new FormItem
                                    {
                                        ItemName = _dbcontext.FormChecklistItems
                                            .Where(f => f.ChecklistItemUid == i.ChecklistItemUid)
                                            .Select(f => f.ItemName)
                                            .FirstOrDefault(),
                                        IsChecked = i.IsChecked,
                                        Remark = i.Remark
                                    }).ToList()
                            }).FirstOrDefault();
                result.Success = true;
                result.Data = Form;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 通過表單
        /// </summary>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> ApproveForm(Guid id)
        {
            var result = new BaseModel<string>();
            var CurrentUser = _CommonService.GetCurrentUser();
            try
            {
                var SubForm = _dbcontext.FormSubmissions.Where(x => x.SubmissionUid == id).FirstOrDefault();

                var formLog = new FormSubmissionLogs
                {
                    SubmissionLogUid = Guid.NewGuid(),
                    SubmissionUid = id,
                    Status = 1,
                    UserUid = CurrentUser,
                    ModifyDate = DateTime.Now
                };


                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    SubForm.Status = 1;        
                    _dbcontext.FormSubmissionLogs.Add(formLog);
                    _dbcontext.FormSubmissions.Update(SubForm);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        /// <summary>
        /// 退回表單
        /// </summary>
        /// <returns>回傳結果</returns>
        public async Task<BaseModel<string>> RejectForm(Guid id)
        {
            var result = new BaseModel<string>();

            var CurrentUser = _CommonService.GetCurrentUser();
            try
            {
                var SubForm = _dbcontext.FormSubmissions.Where(x => x.SubmissionUid == id).FirstOrDefault();

                var formLog = new FormSubmissionLogs
                {
                    SubmissionLogUid = Guid.NewGuid(),
                    SubmissionUid = id,
                    Status = 2,
                    UserUid = CurrentUser,
                    ModifyDate = DateTime.Now
                };

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    SubForm.Status = 2;
                    _dbcontext.FormSubmissionLogs.Add(formLog);
                    _dbcontext.FormSubmissions.Update(SubForm);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                result.Success = true;
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
