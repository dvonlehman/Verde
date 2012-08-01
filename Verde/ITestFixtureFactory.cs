using System;

namespace Verde
{
	/// <summary>
	/// Provides a way to instantiate test fixtures
	/// </summary>
	public interface ITestFixtureFactory
	{
		object InstantiateFixture(Type fixture);
	}
}