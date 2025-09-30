using SenTooliKit.Common.Helpers;

namespace SenTooliKit.Tests.Helper
{
    public class MidHashHelperTest
    {
        
        [Fact]
        public void GetMidHash_Should_ReturnsCorrectHash()
        {
            int expected = 16997662;
            int actual = MidHashHelper.FindUserId("646309bf", 1000_000_000);
            Assert.Equal(expected, actual);
        }
    }
}