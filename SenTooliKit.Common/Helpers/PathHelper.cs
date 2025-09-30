namespace SenTooliKit.Common.Helpers
{
    public static class PathHelper
    {
        /// <summary>
        /// 获取 AppData 路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        
        /// <summary>
        /// 获取 Cookie 保存路径
        /// </summary>
        /// <returns></returns>
        public static string GetCookieSavePath()
        {
            string appDataPath = GetAppDataPath();
            string cookieDic = Path.Combine(appDataPath,"BiliBiliTK" ,"Cookie");
            Directory.CreateDirectory(cookieDic);
            return Path.Combine(appDataPath,"BiliBiliTK" ,"Cookie", "Cookie.json");
        }

        /// <summary>
        /// 获取二维码保存路径
        /// </summary>
        /// <returns></returns>
        public static string GetQrCOdePath()
        {
            string appDataPath = GetAppDataPath();
            string qrCodeDic = Path.Combine(appDataPath,"BiliBiliTK", "Auth");
            Directory.CreateDirectory(qrCodeDic);
            return Path.Combine(appDataPath,"BiliBiliTK", "Auth", "QrCode.png");
        }
        
        /// <summary>
        /// 获取下载路径
        /// </summary>
        /// <returns></returns>
        public static string GetDownloadPath()
        {
            string appDataPath = GetAppDataPath();
            string downDic = Path.Combine(appDataPath,"BiliBiliTK", "Download");
            Directory.CreateDirectory(downDic);
            return Path.Combine(appDataPath,"BiliBiliTK", "Download", "download.mp4");
        }
        /// <summary>
        /// 获取封面保存路径
        /// </summary>
        /// <param name="bvid"></param>
        /// <returns></returns>
        public static string GetCoverImgPath(string bvid)
        {
            string appDataPath = GetAppDataPath();
            string downDic = Path.Combine(appDataPath,"BiliBiliTK", "Download");
            Directory.CreateDirectory(downDic);
            return Path.Combine(appDataPath,"BiliBiliTK", "Download", $"{bvid}.png");
        }
        /// <summary>
        /// 获取结巴分词目录
        /// </summary>
        /// <returns></returns>
        public static string GetJiebaDirectory()
        {
            string appDataPath = GetAppDataPath();
            string downDic = Path.Combine(appDataPath,"BiliBiliTK", "Jieba");
            Directory.CreateDirectory(downDic);
            return downDic;
        }
    }
}