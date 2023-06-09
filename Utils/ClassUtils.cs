using System.Text.Json;

namespace VRPTW.Utils
{
    public static class ClassUtils
    {
        public static T DeepClone<T>(this T obj)
        {
            var serialized = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(serialized) ?? throw new System.Exception("Error while cloning object");
        }
    }
}
