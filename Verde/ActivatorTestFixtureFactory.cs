using System;

namespace Verde
{
	/// <summary>
	/// This test fixture factory uses the .NET Activator to instantiate the test by it's type
	/// </summary>
	public class ActivatorTestFixtureFactory : ITestFixtureFactory
	{
		public object InstantiateFixture(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Could not instantiate IntegrationFixture " + type.FullName, e);
			}
		}
	}
}