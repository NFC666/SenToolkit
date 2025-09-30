using System.Text.RegularExpressions;

namespace SenTooliKit.Common.Helpers
{
    public class BvHelper
    {
        /// <summary>
        /// 从 URL 中提取 BV 号
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetBvFromUrl(string url)
        {
            // 严谨匹配 BV 号
            // ^ 或 / 确保是 URL 中的分隔符，后面紧跟 BV + 10 个字符
            var match = Regex.Match(url, @"(?:^|/)(BV[0-9A-Za-z]{10})(?:/|$|\?)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            throw new ArgumentException("无效的Url");
        }
    }
}