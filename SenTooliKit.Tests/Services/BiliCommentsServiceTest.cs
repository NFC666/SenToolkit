using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Services
{
    public class BiliCommentsServiceTest
    {
        private readonly IBiliCommentsService _biliCommentsService = new BiliCommentsService();
        private readonly IBilibiliInfoService _bilibiliInfoService = new BilibiliInfoService();
        private readonly ITestOutputHelper _output;
        public BiliCommentsServiceTest(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public async Task GetComments_Should_Success()
        {
            var comments = await _biliCommentsService.GetCommentsAsync("BV16gnuzFE4o");
            foreach (var c in comments)
            {
                _output.WriteLine(c.Content.Message);
                if (c.Replies is not null)
                {
                    foreach (var reply in c.Replies)
                    {
                        _output.WriteLine(reply.Content.Message);
                    }
                }
                _output.WriteLine("\n");
            }
            Assert.NotNull(comments);
        }

        [Fact]
        public async Task GetAllComments_Should_Success()
        {
            var comments = await _biliCommentsService.GetBaseAllCommentsAsync("BV16gnuzFE4o");
            foreach (var c in comments)
            {
                _output.WriteLine(c.Content.Message);
                if (c.Replies is not null)
                {
                    foreach (var reply in c.Replies)
                    {
                        _output.WriteLine(reply.Content.Message);
                    }
                }
                _output.WriteLine("\n");
            }
            Assert.NotNull(comments);
            
        }
    }
}