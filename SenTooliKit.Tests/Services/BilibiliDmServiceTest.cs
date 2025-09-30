using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BilibiliDmServiceTest
    {
        private readonly ITestOutputHelper _output;
        
        private readonly IBilibiliDmService _service;
        private readonly IBilibiliInfoService _infoService;
        private readonly IBilibiliAuthService _authService;
        
        
        public BilibiliDmServiceTest(ITestOutputHelper output)
        { 
            _output = output;
            _infoService = new BilibiliInfoService();
            _authService = new BilibiliAuthService();
            _service = new BilibiliDmService();
        }
        [Fact]
        public async Task GetDmTimesByOid_Should_Success()
        { 
            await _authService.Login();
            var cids = await _infoService.GetCidsAsync("BV1BrhHzXEBq");
            var times = await _service.GetDmTimesByOidAsync(cids[0].ToString(),DateTime.Now);
            foreach (string t in times)
            {
                _output.WriteLine(t);
            }
            Assert.NotNull(times);
        }
        [Fact]
        public async Task GetDmByDate_Should_Success()
        {
            await _authService.Login();
            var cids = await _infoService.GetCidsAsync("BV1qepnzFEtZ");
            var times = await _service.GetDmTimesByOidAsync(cids[0].ToString(),DateTime.Now);
            var dm = await _service.GetDmSegMobileReplyByDateAsync(cids[0],times[0]);
            Assert.NotNull(dm);
        }
        [Fact]
        public async Task GetDmList_Should_Success()
        {
            await _authService.Login();
            var cids = await _infoService.GetCidsAsync("BV1VBWwzBEad");
            var times = await _service.GetDmTimesByOidAsync(cids[0].ToString(),DateTime.Now);
            var dm = await _service.GetDmInfoAsync(cids[0],times[0]);
            foreach (var d in dm)
            {
                _output.WriteLine(d.DmContent);
            }
            Assert.NotNull(dm);
        }
        
    }
}