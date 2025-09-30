using SenTooliKit.Common.Helpers;
using SenTooliKit.Repository.Factory;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BilibiliInfoServiceTest
    {
        private readonly HttpClient _httpClient;
        private readonly BilibiliAuthService _bilibiliAuthService;
        private readonly ITestOutputHelper _output;
        private readonly BilibiliInfoService _bilibiliInfoService;
        public BilibiliInfoServiceTest(ITestOutputHelper  output)
        {
            _output = output;
            _httpClient = HttpFactory.GetHttpClient();
            _bilibiliAuthService = new BilibiliAuthService();
            _bilibiliInfoService = new BilibiliInfoService();
        }
        [Fact]
        public async Task GetCidsAsync_Should_Success()
        {
            var result = await _bilibiliInfoService.GetCidsAsync("BV1iipRzdEyS");
            _output.WriteLine(result.Length.ToString());
            Assert.NotNull(result);
        }
        [Fact]
        public async Task GetOldEpisodeCidsAsync_Should_Success()
        {
            var result = await _bilibiliInfoService.GetCidsAsync("BV1Th41117mf");
            _output.WriteLine(result.Length.ToString());
            Assert.Equal(12, result.Length);
        }

        [Fact]
        public async Task GetStreamInfo_Should_NotNull()
        {
            var cid = StreamCidHelper.GetCidFromUrl(
                "https://live.bilibili.com/6?live_from=84001&trackid=undefined&spm_id_from=333.337.search-card.all.click");
            var res = await _bilibiliInfoService.GetStreamInfoAsync(cid);
            _output.WriteLine(res);
            Assert.NotNull(res);
        }

        
    }
}