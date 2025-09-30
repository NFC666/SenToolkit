using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BilibiliAuthServiceTest
    {
        private readonly IBilibiliAuthService _bilibiliAuthService;
        private readonly ITestOutputHelper _output;
        public BilibiliAuthServiceTest(ITestOutputHelper  output)
        {
            _output = output;
            _bilibiliAuthService = new BilibiliAuthService();
            
        }
        [Fact]
        public async Task LoginByScanQr_Should_Success()
        {
            await _bilibiliAuthService.GetQrCodePathAsync();
            bool res = await _bilibiliAuthService.Login();
            Assert.True(res);
        }
    }
}