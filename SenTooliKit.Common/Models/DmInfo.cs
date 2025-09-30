using System.ComponentModel.DataAnnotations;

namespace SenTooliKit.Common.Models
{
    public class DmInfo
    {
        [Display(Name = "发送人Id")]
        public string SendId { get; set; }

        [Display(Name = "弹幕内容")]
        public string DmContent { get; set; }

        [Display(Name = "发送时间")]
        public string SendTime { get; set; }

        [Display(Name = "弹幕Id")]
        public long DmId { get; set; }
    }
}