using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace Survey.Service.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            // Default(T) yerine null kontrolü yapıp varsayılan değer döndürün
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
