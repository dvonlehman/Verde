using NUnit.Framework;
using Verde;

namespace MvcMusicStore.IntegrationTests
{
	[IntegrationFixture]
	public class PassTests
	{
		[IntegrationTest]
		public void Assert_Pass()
		{
			Assert.Pass();
		}

		[IntegrationTest]
		public void Assert_Pass_CustomMessage()
		{
			Assert.Pass("Custom message");
		}
	}
}