using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;

using SenTooliKit.Common.Helpers;
using SenTooliKit.IServices.IServices;
using SenTooliKit.Services.Services;

using Xunit.Abstractions;

namespace SenTooliKit.Tests.Helper
{
    public class BilibiliWbiHelperHelper
    {
        private IBilibiliInfoService _bilibiliInfoService = new BilibiliInfoService();
        private readonly ITestOutputHelper _output;

        public BilibiliWbiHelperHelper(ITestOutputHelper output)
        {
            _output = output;
        }
        


    }
}