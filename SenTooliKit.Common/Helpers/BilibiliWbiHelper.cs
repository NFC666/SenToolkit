using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SenTooliKit.Common.Helpers
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public static class BilibiliWbiHelper
    {
        private static readonly int[] MixinKeyEncTab =
        {
            46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35, 27, 43, 5, 49, 33, 9, 42, 19, 29, 28, 14,
            39, 12, 38, 41, 13, 37, 48, 7, 16, 24, 55, 40, 61, 26, 17, 0, 1, 60, 51, 30, 4, 22, 25, 54, 21, 56, 59,
            6, 63, 57, 62, 11, 36, 20, 34, 44, 52
        };
        
        public static string GetMixinKey(string orig)
        {
            return MixinKeyEncTab.Aggregate("", (s, i) => s + orig[i])[..32];
        }

        /// <summary>
        /// Wbi 加密
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="imgKey"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public static Dictionary<string, string> EncWbi(Dictionary<string, string> parameters, string imgKey,
            string subKey)
        {
            string mixinKey = GetMixinKey(imgKey + subKey);
            string currTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            //添加 wts 字段
            parameters["wts"] = "1758973738";
            // 按照 key 重排参数
            parameters = parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
            //过滤 value 中的 "!'()*" 字符
            parameters = parameters.ToDictionary(
                kvp => kvp.Key,
                kvp => new string(kvp.Value.Where(chr => !"!'()*".Contains(chr)).ToArray())
            );
            // 序列化参数
            string query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
            //计算 w_rid
            using MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query + mixinKey));
            string wbiSign = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            parameters["w_rid"] = wbiSign;

            return parameters;
        }

        /// <summary>
        /// 从 URL 中提取 Key
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ExtractKeyFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                var uri = new Uri(url);
                // 获取最后一个路径部分
                var fileName = Path.GetFileName(uri.AbsolutePath);
                // 去掉扩展名
                var key = Path.GetFileNameWithoutExtension(fileName);
                return key;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}