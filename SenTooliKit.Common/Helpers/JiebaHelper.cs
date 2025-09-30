using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;

using JiebaNet.Segmenter;

namespace SenTooliKit.Common.Helpers
{
    public class JiebaHelper
    {
        private static string JiebaDirectory = PathHelper.GetJiebaDirectory();
        static JiebaHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("Jieba.zip")
                ?? throw new FileNotFoundException("Jieba.zip");
            var filePath = Path.Combine(JiebaDirectory, "dict.txt");
            // 解压到指定目录
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(JiebaDirectory, overwriteFiles: true);
            }
            JiebaNet.Segmenter.ConfigManager.ConfigFileBaseDir = Path.Combine(JiebaDirectory,"Resources");
            
        }
        
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            // 语气词/叹词
            "啊", "呢", "吗", "呀", "吧", "哇", "哦", "呵", "嗯", "哈", "嘿", "哎", "哟", "唉", "呗", "咦", "嘻", "呃", "哇塞", "呵呵",

            // 常用助词/虚词
            "的", "了", "在", "是", "就", "都", "而", "及", "与", "或", "也", "只是", "甚至", "然后", "而且", "还有", "因为", "所以", "但是", "如果", "并", "等", "究竟", "虽然", "可", "嘛", "咧", "啊呀",

            // 代词
            "我", "你", "他", "她", "它", "我们", "你们", "他们", "她们", "它们", "自己", "别人", "谁", "什么", "哪", "哪儿", "哪里",

            // 指示代词
            "这", "那", "这些", "那些", "这里", "那里", "之", "此", "其",

            // 常用虚词
            "不", "还", "没", "就", "和", "与", "或", "而", "等", "被", "给", "被", "由", "将", "比", "向", "关于",

            // 数字（中文）
            "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "百", "千", "万", "亿",

            // 数字（阿拉伯数字）
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            
            "有","这个","那个","哈","哈哈","哈哈哈","哈哈哈哈","哈哈哈哈哈","哈哈哈哈哈哈"
        };

        /// <summary>
        /// 分词并过滤标点、空白字符和停用词
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> CutFiltered(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var segmenter = new JiebaSegmenter();
            var words = segmenter.Cut(text);

            // 定义正则，去掉标点符号和空白字符
            var regex = new Regex(@"^[\p{P}\p{S}\s]+$");

            // 过滤标点、空白和停用词
            var filtered = words
                .Where(w => !regex.IsMatch(w) && !StopWords.Contains(w))
                .ToList();

            return filtered;
        }
    }
}