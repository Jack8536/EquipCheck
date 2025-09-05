public class PaginationModel
{
    public int TotalItems { get; set; }          // 總記錄數
    public int ItemsPerPage { get; set; }        // 每頁顯示數量
    public int CurrentPage { get; set; }         // 當前頁碼
    public int TotalPages                        // 總頁數
    {
        get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
    }
    public bool ShowPrevious                     // 是否顯示上一頁
    {
        get { return CurrentPage > 1; }
    }
    public bool ShowNext                         // 是否顯示下一頁
    {
        get { return CurrentPage < TotalPages; }
    }
    public int StartPage                         // 起始頁碼
    {
        get
        {
            var start = CurrentPage - 2;
            return start <= 0 ? 1 : start;
        }
    }
    public int EndPage                           // 結束頁碼
    {
        get
        {
            var end = StartPage + 4;
            return end > TotalPages ? TotalPages : end;
        }
    }
}