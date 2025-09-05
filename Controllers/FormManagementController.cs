using EquipCheck.Models.ViewModels;
using EquipCheck.Services;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace EquipCheck.Controllers
{
    public class FormManagementController : Controller
    {
        private readonly CommonService _CommonService;
        private readonly FormManagementService _formManagementService;
        
        public FormManagementController(CommonService commonService, FormManagementService formManagementService)
        {
            _CommonService = commonService;
            _formManagementService = formManagementService;
        }

        public async Task<IActionResult> List()
        {
            VM_Form model = new VM_Form();
            var Forms = await _formManagementService.GetFormList();

            if (Forms.Success)
            {
                model.FormList = Forms.Data;
            }
            else
            {
                model.FormList = new List<FormListModel>();
            }

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var model = new VM_Form();
            var AllUser = _CommonService.GetUserInfoList();
            model.SponsorDDL = AllUser.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.UserName,
                Value = x.UserUid.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(VM_Form model)
        {
            var result = await _formManagementService.CreateForm(model);
            
            return Json(result);
        }

        public async Task<IActionResult> Edit()
        {
            // 取得完整表單

            return View();
        }

        public async Task<IActionResult> Judge()
        {
            var model = new VM_Form();

            // 取得使用者已提交表單
            var result = await _formManagementService.GetSubmissionForm();

            if (result.Success)
            {
                model.SubmissionFormList = result.Data;
            }
            else
            {
                model.SubmissionFormList = new List<SubmissionFormModel>();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DetailView(Guid id)
        {
            // 撈取使用者已填寫的表單
            var model = new VM_Form();

            var FullForm = await _formManagementService.GetUserSubmissionForm(id);

            if (FullForm.Success)
            {
                model.FullForm = FullForm.Data;
            }
            else
            {
                model.FullForm = new FullForm();
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Approve(Guid submissionUid)
        {
            var result = await _formManagementService.ApproveForm(submissionUid);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> reject(Guid submissionUid)
        {
            var result = await _formManagementService.RejectForm(submissionUid);

            return Json(result);
        }


        // 產出Word
        [HttpPost]
        public async Task<IActionResult> ExportWord(Guid formId)
        {
            try
            {
                // 取得表單資料
                var formData = await _formManagementService.GetUserSubmissionForm(formId);
                if (!formData.Success || formData.Data == null)
                {
                    return NotFound();
                }

                var form = formData.Data;

                // 建立文件
                var doc = new XWPFDocument();

                // 設定標題
                var title = doc.CreateParagraph();
                title.Alignment = ParagraphAlignment.CENTER;
                var titleRun = title.CreateRun();
                titleRun.SetText("設備檢查表");
                titleRun.IsBold = true;
                titleRun.FontSize = 16;

                // 新增空行
                doc.CreateParagraph();

                // 建立基本資料表格
                var basicTable = doc.CreateTable(5, 2);
                SetTableStyle(basicTable);

                // 填寫基本資料
                FillTableRow(basicTable.GetRow(0), "填寫人", form.EmployeeName);
                FillTableRow(basicTable.GetRow(1), "部門", form.DepartmentName);
                FillTableRow(basicTable.GetRow(2), "分機", form.Tel);
                FillTableRow(basicTable.GetRow(3), "資產編號", form.AssetCode);
                FillTableRow(basicTable.GetRow(4), "檢查日期", form.CheckDate.ToString("yyyy/MM/dd"));

                // 新增空行
                doc.CreateParagraph();

                // 建立檢查項目表格
                if (form.Items != null && form.Items.Any())
                {
                    var itemTable = doc.CreateTable(form.Items.Count + 1, 4);
                    SetTableStyle(itemTable);

                    // 設定表頭
                    var headerRow = itemTable.GetRow(0);
                    headerRow.GetCell(0).SetText("#");
                    headerRow.GetCell(1).SetText("檢查項目");
                    headerRow.GetCell(2).SetText("檢查結果");
                    headerRow.GetCell(3).SetText("查核紀錄");

                    // 填寫檢查項目
                    for (int i = 0; i < form.Items.Count; i++)
                    {
                        var item = form.Items[i];
                        var row = itemTable.GetRow(i + 1);
                        row.GetCell(0).SetText((i + 1).ToString());
                        row.GetCell(1).SetText(item.ItemName);
                        row.GetCell(2).SetText(item.IsChecked == 1 ? "符合" : "不符合");
                        row.GetCell(3).SetText(item.Remark ?? "");
                    }
                }

                // 輸出檔案
                using (var ms = new MemoryStream())
                {
                    doc.Write(ms);
                    var currentUser = _CommonService.GetCurrentUser();
                    await _CommonService.WriteActionLog(8, true, currentUser, "匯出設備檢查表"); // 8:匯出
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"設備檢查表_{DateTime.Now:yyyyMMdd}.docx");
                }
            }
            catch (Exception ex)
            {
                var currentUser = _CommonService.GetCurrentUser();
                await _CommonService.WriteActionLog(8, false, currentUser, "匯出設備檢查表失敗"); // 8:匯出
                return StatusCode(500, "產生文件時發生錯誤");
            }
        }

        private void SetTableStyle(XWPFTable table)
        {
            // 設定表格寬度
            table.Width = 5000;

            // 設定第一欄寬度
            if (table.Rows.Count > 0)
            {
                var firstRow = table.GetRow(0);
                //firstRow.GetCell(0).SetWidth("500");
            }

            // 設定表格邊框
            var tablePr = table.GetCTTbl().AddNewTblPr();
            var borders = tablePr.AddNewTblBorders();

            // 設定邊框顏色和寬度
            var borderSize = 8; // 4 * 2 (原始值的兩倍，因為 NPOI 2.7.4 的單位不同)

            // 外框設定
            borders.top = new CT_Border();
            borders.top.val = ST_Border.single;
            borders.top.sz = (ulong)borderSize;
            borders.top.color = "000000";

            borders.bottom = new CT_Border();
            borders.bottom.val = ST_Border.single;
            borders.bottom.sz = (ulong)borderSize;
            borders.bottom.color = "000000";

            borders.left = new CT_Border();
            borders.left.val = ST_Border.single;
            borders.left.sz = (ulong)borderSize;
            borders.left.color = "000000";

            borders.right = new CT_Border();
            borders.right.val = ST_Border.single;
            borders.right.sz = (ulong)borderSize;
            borders.right.color = "000000";

            // 內框設定
            borders.insideH = new CT_Border();
            borders.insideH.val = ST_Border.single;
            borders.insideH.sz = (ulong)borderSize;
            borders.insideH.color = "000000";

            borders.insideV = new CT_Border();
            borders.insideV.val = ST_Border.single;
            borders.insideV.sz = (ulong)borderSize;
            borders.insideV.color = "000000";

            // 設定表格對齊方式
            tablePr.jc = new CT_Jc() { val = ST_Jc.center };
        }

        private void FillTableRow(XWPFTableRow row, string label, string value)
        {
            var labelCell = row.GetCell(0);
            var valueCell = row.GetCell(1);

            labelCell.SetText(label);
            valueCell.SetText(value ?? "");

            // 設定樣式
            labelCell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
            valueCell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
        }
    }
}
