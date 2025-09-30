using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BilibiliVideoServiceTest
    {
        private readonly ITestOutputHelper _output;
        
        private readonly IBilibiliVideoService _service;
        private readonly IBilibiliInfoService _infoService;
        private readonly IBilibiliAuthService _authService;
        
        
        public BilibiliVideoServiceTest(ITestOutputHelper output)
        { 
            _output = output;
            _infoService = new BilibiliInfoService();
            _authService = new BilibiliAuthService();
            _service = new BilibiliVideoService();
        }

        [Fact]
        public async Task DownloadVideoAsync_Should_SaveFile()
        {
            await _authService.Login();
            string bvid = "BV1p9bhzUEDk";
            var cids = await _infoService.GetCidsAsync(bvid);
            var path = await _service.DownloadVideoAsync(bvid, cids[0],null);
            _output.WriteLine(path);
            Assert.True(File.Exists(path));
        }
        [Fact]
        public async Task DownloadVideoWithFilePathAsync_Should_SaveFile()
        {
            await _authService.Login();
            string bvid = "BV1p9bhzUEDk";
            var cids = await _infoService.GetCidsAsync(bvid);
            var fp = "D:\\软件\\Test\\111.mp4";
            var path = await _service.DownloadVideoAsync(bvid, cids[0],fp);
            _output.WriteLine(path);
            Assert.True(File.Exists(fp));
        }
        [Fact]
        public async Task DownloadCoverAsync_Should_SaveFile()
        {
            await _authService.Login();
            string bvid = "BV1p9bhzUEDk";
            var path = await _service.DownloadCoverAsync(bvid);
            _output.WriteLine(path);
            Assert.True(File.Exists(path));
        }

        [Fact]
        public async Task GetVideoDetail_Should_Success()
        {
            await _authService.Login();
            string bvid = "BV1p9bhzUEDk";
            var info = await _service.GetVideoDetailInfoAsync(bvid);
            _output.WriteLine(info.Title);
        }
    }
}