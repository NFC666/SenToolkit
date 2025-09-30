using SenTooliKit.Common.Helpers;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Helper
{
    public class JieBaHelperTest
    {
        private readonly ITestOutputHelper _output;
        public JieBaHelperTest(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void GetJieBaResult_Should_ReturnsCorrectResult()
        { 
            
            var text = "一个有趣的测试";
            var result = JiebaHelper.CutFiltered(text);
            _output.WriteLine(string.Join(",", result));
            Assert.Equal(3, result.Count);
            
        }
    }
}