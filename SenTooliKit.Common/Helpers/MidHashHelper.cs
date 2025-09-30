using System;
using System.Text;
using Force.Crc32; // 需要安装 NuGet 包：Crc32.NET

namespace SenTooliKit.Common.Helpers
{
    public class MidHashHelper
    {
        // 使用 Crc32.NET 计算字符串的 CRC32
        private static uint Compute(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Crc32Algorithm.Compute(bytes);
        }

        /// <summary>
        /// 通过暴力枚举查找 userId（mid）
        /// </summary>
        /// <param name="expectedHex">目标 CRC32 值（例如 "59417e95"）</param>
        /// <param name="maxSearch">搜索最大范围</param>
        /// <returns>找到的 userId，没找到返回 -1</returns>
        public static int FindUserId(string expectedHex, int maxSearch = 2_000_000_000)
        {
            uint expected = Convert.ToUInt32(expectedHex, 16);

            for (int mid = 0; mid < maxSearch; mid++)
            {
                uint hashid = Compute(mid.ToString());
                if (hashid == expected)
                    return mid;

                if (mid % 10_000_000 == 0)
                    Console.WriteLine($"Searching... mid={mid}");
            }

            return -1;
        }
    }
}