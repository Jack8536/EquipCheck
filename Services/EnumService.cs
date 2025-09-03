using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EquipCheck.Services
{
    public class EnumService
    {
        /// <summary>
        /// 取得列舉的 Display Name
        /// </summary>
        /// <typeparam name="T">列舉型別</typeparam>
        /// <param name="value">列舉值</param>
        /// <returns>Display Name 或原始名稱</returns>
        public string GetDisplayName<T>(T value) where T : Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// 根據數值取得列舉的 Display Name
        /// </summary>
        /// <typeparam name="T">列舉型別</typeparam>
        /// <param name="value">數值</param>
        /// <returns>Display Name 或原始名稱</returns>
        public string GetDisplayNameByValue<T>(int value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                var enumValue = (T)Enum.ToObject(typeof(T), value);
                return GetDisplayName(enumValue);
            }
            return string.Empty;
        }
    }
}