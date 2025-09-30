using Bilibili.Community.Service.Dm.V1;

namespace SenTooliKit.Common.Models
{
    public class DmDetailInfo(DanmakuElem danmakuElem) : DanmakuElem(danmakuElem)
    {

        public string ProgressTime => MillisecondsToMinuteSecond(danmakuElem.Progress);
        public string CDate => SecondsToDateTime(danmakuElem.Ctime);
        
        // 毫秒转 "mm:ss"
        public static string MillisecondsToMinuteSecond(long ms)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(ms);
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        // 秒转 "yyyy-MM-dd HH:mm:ss"
        public static string SecondsToDateTime(long sec)
        {
            // Unix 时间戳是从 1970-01-01 00:00:00 UTC 开始的
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(sec);
            // 转换到本地时间（如果你要北京时间，可以加上 8 小时偏移）
            return dto.ToLocalTime().ToString("yyyy-MM-dd");
        }
    }
    
}