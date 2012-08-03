using NUnit.Framework;
using Verde;

namespace MvcMusicStore.IntegrationTests
{
	[IntegrationFixture]
	public class GeneralTests
	{
		[IntegrationTest]
		public void Assert_Pass()
		{
			Assert.Pass("Custom message");
		}
	}
}