using System.Text.RegularExpressions;

namespace SenTooliKit.Common.Helpers
{
    public class StreamCidHelper
    {
        /// <summary>
        /// 从直播间链接中提取 CId（long 类型）
        /// </summary>
        /// <param name="url">B站直播间链接，例如 https://live.bilibili.com/1939022927?xx=yy</param>
        /// <returns>CId（直播间号）</returns>
        /// <exception cref="ArgumentException">当URL无效或无法解析为long时抛出</exception>
        public static long GetCidFromUrl(string url)
        {
            // 严谨匹配 live.bilibili.com/ 后面的一串数字（直播间号）
            var match = Regex.Match(url, @"live\.bilibili\.com/(\d+)", RegexOptions.IgnoreCase);
            if (match.Success && long.TryParse(match.Groups[1].Value, out long cid))
            {
                return cid;
            }
            throw new ArgumentException("无效的Url");
        }
    }
}