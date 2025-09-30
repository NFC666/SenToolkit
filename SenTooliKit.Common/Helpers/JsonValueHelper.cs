using Newtonsoft.Json.Linq;

using SenTooliKit.Repository.Factory;

namespace SenTooliKit.Common.Helpers
{
    public class JsonValueHelper
    {
        private static readonly HttpClient _httpClient = HttpFactory.GetHttpClient();
        /// <summary>
        /// 获取 Json 中指定路径的值
        /// </summary>
        /// <param name="response"></param>
        /// <param name="jsonPath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetJsonValue<T>(string response, string jsonPath)
        {
            JObject json = JObject.Parse(response);

            // 如果传的是简单字段名（例如 "url"），就递归查找
            if (!jsonPath.StartsWith("$"))
            {
                jsonPath = "$.." + jsonPath;
            }

            // 使用 JsonPath 查询
            var tokens = json.SelectTokens(jsonPath).ToList();

            if (tokens.Count == 0)
            {
                throw new InvalidOperationException($"未找到路径: {jsonPath}");
            }


            if (typeof(T).IsArray)
            {
                var elementType = typeof(T).GetElementType()!;
                // 如果 tokens 里只有一个，并且是 JArray，就展开
                if (tokens.Count == 1 && tokens[0] is JArray arr)
                {
                    return arr.ToObject<T>();
                }

                var array = Array.CreateInstance(elementType, tokens.Count);
                for (int i = 0; i < tokens.Count; i++)
                {
                    array.SetValue(tokens[i].ToObject(elementType), i);
                }
                return (T)(object)array;
            }

            // 如果是简单类型（int, string, bool 等）
            if (typeof(T).IsPrimitive || typeof(T) == typeof(string) || typeof(T) == typeof(decimal))
                return tokens.First().Value<T>();

            // 复杂对象（类、结构体等）
            return tokens.First().ToObject<T>()!;
        }

        /// <summary>
        /// 使用 query 参数获取 Json 中指定路径的值
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="jsonPath"></param>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> GetJsonValueWithQueryAsync<T>(string baseUrl,string jsonPath, Dictionary<string, string> query)
        {
            string url = baseUrl + "?" + string.Join("&", query.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            string response = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            return GetJsonValue<T>(response, jsonPath);
        }
    }
}