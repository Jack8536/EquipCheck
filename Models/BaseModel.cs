namespace EquipCheck.Models
{
    public class BaseModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public T? Data { get; set; }
    }
}
