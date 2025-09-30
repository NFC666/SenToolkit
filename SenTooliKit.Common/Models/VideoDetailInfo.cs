namespace SenTooliKit.Common.Models
{
    public class VideoDetailInfo
    {
        public string Bvid { get; set; } // 稿件bvid
        public long Aid { get; set; } // 稿件avid
        public int Videos { get; set; } = 1; // 稿件分P总数，默认为1
        public int Tid { get; set; } // 分区tid
        public int TidV2 { get; set; } // 分区tid (v2)
        public string Tname { get; set; } // 子分区名称
        public string TnameV2 { get; set; } // 子分区名称 (v2)
        public int Copyright { get; set; } // 视频类型 1：原创 2：转载
        public string Pic { get; set; } // 稿件封面图片url
        public string Title { get; set; } // 稿件标题
        public long Pubdate { get; set; } // 稿件发布时间（秒级时间戳）
        public long Ctime { get; set; } // 用户投稿时间（秒级时间戳）
        public string Desc { get; set; } // 视频简介
        public List<string> DescV2 { get; set; } // 新版视频简介
        public int State { get; set; } // 视频状态
        [Obsolete("Attribute 已弃用")] public int Attribute { get; set; } // 稿件属性位配置
        public int Duration { get; set; } // 稿件总时长（单位：秒）
        public long Forward { get; set; } // 撞车视频跳转avid，仅撞车视频存在
        public long MissionId { get; set; } // 稿件参与的活动id
        public string RedirectUrl { get; set; } // 重定向url，仅番剧或影视视频存在
        public object Rights { get; set; } // 视频属性标志
        public object Owner { get; set; } // 视频UP主信息
        public object Stat { get; set; } // 视频状态数
        public object ArgueInfo { get; set; } // 争议/警告信息
        public string Dynamic { get; set; } // 视频同步发布动态内容
        public long Cid { get; set; } // 视频1P cid
        public object Dimension { get; set; } // 视频1P分辨率
        public object Premiere { get; set; } // null
        public int TeenageMode { get; set; } // 青少年模式
        public bool IsChargeableSeason { get; set; }
        public bool IsStory { get; set; } // 是否可以在 Story Mode 展示
        public bool IsUpowerExclusive { get; set; } // 是否为充电专属视频
        public bool IsUpowerPlay { get; set; }
        public bool IsUpowerPreview { get; set; } // 充电专属视频是否支持试看
        public bool NoCache { get; set; } // 是否不允许缓存
        public List<VideoPage> Pages { get; set; } // 视频分P列表
        public object Subtitle { get; set; } // 视频CC字幕信息
        public object UgcSeason { get; set; } // 视频合集信息
        public List<object> Staff { get; set; } // 合作成员列表
        public bool IsSeasonDisplay { get; set; }
        public object UserGarb { get; set; } // 用户装扮信息
        public object HonorReply { get; set; }
        public string LikeIcon { get; set; } // 空串
        public bool NeedJumpBv { get; set; } // 需要跳转到BV号
        public bool DisableShowUpInfo { get; set; } // 禁止展示UP主信息
        public bool IsStoryPlay { get; set; } // 是否为 Story Mode 视频
        public bool IsViewSelf { get; set; } // 是否为自己投稿的视频
    }
}