using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BilibiliStreamServiceTest
    {
        private readonly IBilibiliStreamService _bilibiliStreamService;
        private readonly ITestOutputHelper _output;

        public BilibiliStreamServiceTest(ITestOutputHelper output)
        {
            _output = output;
            _bilibiliStreamService = new BilibiliStreamService();
        }

        [Fact]
        public async Task GetStreamSourceAsync_Should_Success()
        {
            var cid = StreamCidHelper.GetCidFromUrl(
                "https://live.bilibili.com/1939022927?live_from=84001&trackid=undefined&spm_id_from=333.337.search-card.all.click");
            var result = await _bilibiliStreamService
                .GetStreamSourceAsync(cid);
            foreach (string r in result)
            {
                _output.WriteLine(r);
            }

            Assert.NotNull(result);
        }

        [Fact]
        public async Task DownloadStreamToFileAsync_Should_Cancel()
        {
            try
            {
                var savePath = "D:\\软件";
                using CancellationTokenSource cts = new();

                var cid = StreamCidHelper.GetCidFromUrl(
                    "https://live.bilibili.com/6?live_from=84001&trackid=undefined&spm_id_from=333.337.search-card.all.click");
                var downloadTask = _bilibiliStreamService
                    .DownloadStreamToFileAsync(cid, savePath, cancellationToken: cts.Token);
            
                await Task.Delay(2000);
                await cts.CancelAsync();
                await downloadTask;
            }catch (OperationCanceledException)
            {
                _output.WriteLine("Canceled");
                Assert.True(true);
            }
        }
    }
}