using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SenTooliKit.Converter
{
    public class PathToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // 避免文件锁定
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache; // 忽略缓存
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    // 解析失败返回默认图
                }
            }

            // 默认返回空，或换成你自己的默认图片
            return null;
            // return new BitmapImage(new Uri("pack://application:,,,/Resources/default.png"));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}