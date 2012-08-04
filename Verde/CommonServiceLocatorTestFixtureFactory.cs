using System;
using System.Reflection;

namespace Verde
{
	/// <summary>
	/// A TestFixture Factory that is bases on the CommonServiceLocator Library (http://commonservicelocator.codeplex.com/)
	/// </summary>
	public class CommonServiceLocatorTestFixtureFactory : ITestFixtureFactory
	{
		private readonly object serviceLocator;

		private readonly MethodInfo getInstanceMethod;

		private const string ServiceLocatorTypeFullName = "Microsoft.Practices.ServiceLocation.IServiceLocator";

		/// <summary>
		/// Creates a TestFixture Factory using a CommonServiceLocator
		/// </summary>
		/// <param name="serviceLocator">The service locator from the CommonServiceLocator Library (http://commonservicelocator.codeplex.com/), it is of type 'object' to avoid dependency on the CommonServiceLocator library</param>
		/// <exception cref="InvalidOperationException">If the serviceLocator is not a compatible to IServiceLocator from the CommonServiceLocator Library</exception>
		public CommonServiceLocatorTestFixtureFactory(object serviceLocator)
		{
			this.serviceLocator = serviceLocator;
			Type serviceLocatorType = serviceLocator.GetType();

			Type serviceLocatorInterface = serviceLocatorType.GetInterface("IServiceLocator");


			if (serviceLocatorInterface.FullName != ServiceLocatorTypeFullName)
			{
				throw new InvalidOperationException(string.Format("The 'serviceLocator' object must be of type '{0}'", ServiceLocatorTypeFullName));
			}

			try
			{
				getInstanceMethod = serviceLocatorType.GetMethod("GetInstance", new Type[] { typeof(Type) });
			}
			catch (AmbiguousMatchException ex)
			{
				throw new InvalidOperationException("The 'serviceLocator' object must have only one method of with signature : GetInstance(Type type)");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Can't instantiate the IntegrationFixture</exception>
		public object InstantiateFixture(Type type)
		{
			object testFixture;

			try
			{
				testFixture = getInstanceMethod.Invoke(serviceLocator, new object[] { type});
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Could not instantiate IntegrationFixture " + type.FullName, e);
			}

			return testFixture;
		}
	}
}