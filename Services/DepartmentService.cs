using AspNetCoreGeneratedDocument;
using EquipCheck.Models;
using EquipCheck.Models.DB;
using EquipCheck.Models.Enum;
using EquipCheck.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquipCheck.Services
{
    public class DepartmentService
    {
        private readonly DBContext _dbcontext;
        private readonly CommonService _CommonService;
        private readonly EnumService _enumService;

        public DepartmentService(DBContext dBContext, CommonService commonService, EnumService enumService)
        {
            _dbcontext = dBContext;
            _CommonService = commonService;
            _enumService = enumService;
        }

        /// <summary>
        /// 取得部門清單
        /// </summary>        
        /// <returns>成功返回部門資訊</returns>
        public async Task<BaseModel<List<DeptListModel>>> GetDeptList(VM_Department model)
        {
            var result = new BaseModel<List<DeptListModel>>();

            try
            {
                var deptList = (from dept in _dbcontext.Departments
                                join user in _dbcontext.Users on dept.DepartmentUid equals user.DepartmentUid into departmentUsers
                                select new DeptListModel
                                {
                                    DepartmentUID = dept.DepartmentUid,
                                    DepartmentName = dept.DepartmentName,
                                    DepartmentStatus = _enumService.GetDisplayNameByValue<Status>(dept.Status),
                                    Manager = _dbcontext.Users.Where(u => u.UserUid == dept.ManagerUid).Select(u => u.UserName).FirstOrDefault() ?? string.Empty,
                                    Members = departmentUsers.Select(u => u.UserName).ToList()
                                }).OrderBy(x => x.DepartmentName).ToList();

                // 查詢
                if (!string.IsNullOrEmpty(model.keyword))
                {
                    deptList = deptList.Where(x => x.DepartmentName.Contains(model.keyword) || x.Manager.Contains(model.keyword) || x.Members.Contains(model.keyword)).ToList();
                }

                result.Success = true;
                result.Data = deptList;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        public async Task<BaseModel<string>> CreateDept(VM_Department model)
        {
            BaseModel<string> result = new BaseModel<string>();
            var CurrentUser = _CommonService.GetCurrentUser();

            #region 檢查必填寫
            if (string.IsNullOrEmpty(model.DepartmentName))
            {
                result.Success = false;
                result.Message = "有必填欄位沒填寫";
                return result;
            }

            // 部門名稱不能重複
            var IsExist = _dbcontext.Departments.Where(x => x.DepartmentName == model.DepartmentName).FirstOrDefault();
            if (IsExist != null)
            {
                result.Success = false;
                result.Message = "部門名稱重複";
                return result;
            }
            #endregion

            try
            {

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    // 建立新部門資料
                    var newDepartment = new Departments
                    {
                        DepartmentUid = Guid.NewGuid(),
                        DepartmentName = model.DepartmentName,
                        ManagerUid = model.Manager,
                        Status = 1, // 預設啟用
                        CreatedUser = CurrentUser,
                        CreatedDate = DateTime.Now,
                        ModifyUser = null,
                        ModifyDate = null
                    };

                    _dbcontext.Departments.Add(newDepartment);
                    _dbcontext.SaveChanges();                    
                    _CommonService.WriteActionLog(3, true, CurrentUser, "部門管理"); // 3:新增
                    result.Success = true;
                    result.Message = "新增成功";

                    await transaction.CommitAsync();
                }                
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
                _CommonService.WriteActionLog(3, false, CurrentUser, "部門管理"); 
            }

            return result;
        }

        public async Task<BaseModel<string>> EditDept(VM_Department model)
        {
            var CurrentUser = _CommonService.GetCurrentUser();
            
            BaseModel<string> result = new BaseModel<string>();

            try
            {
                var dept = _dbcontext.Departments.Where(x => x.DepartmentUid == model.DepartmentUID).FirstOrDefault();

                using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                    dept.ManagerUid = model.Manager;
                    
                    _dbcontext.Departments.Update(dept);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _CommonService.WriteActionLog(4, true, CurrentUser, "部門管理");
                }
                result.Success = true;
                result.Message = "修改成功";
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
