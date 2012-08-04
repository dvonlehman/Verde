using System;
using System.Globalization;


namespace Verde
{
	/// <summary>
	/// This test fixture factory uses the .NET Activator to instantiate the test by it's type
	/// </summary>
	public class ActivatorTestFixtureFactory : ITestFixtureFactory
	{


		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Can't instantiate the IntegrationFixture</exception>
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