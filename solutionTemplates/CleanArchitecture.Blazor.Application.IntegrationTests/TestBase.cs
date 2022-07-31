using NUnit.Framework;
using System.Threading.Tasks;

namespace CleanArchitecture.$safeprojectname$
{
    using static Testing;

    public class TestBase
    {
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();
        }
    }
}
