using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Verde.Executor;

namespace Verde
{
    internal class NUnitTestRunner : ITestRunner
    {
        private Settings _settings;
        private List<NUnitTestFixture> _fixtures;

        public NUnitTestRunner(Settings settings)
        {
            _settings = settings;

            _fixtures = new List<NUnitTestFixture>();
            foreach (var type in settings.TestsAssembly.GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(IntegrationFixtureAttribute), false);
                if (attrs.Length == 0)
                    continue;

                var fixture = new NUnitTestFixture(type, ((IntegrationFixtureAttribute)attrs[0]).Sequence);
                _fixtures.Add(fixture);
            }

            _fixtures.Sort((f1, f2) => f1.Sequence.CompareTo(f2.Sequence));        
        }

        public ResultsDto Execute(string fixtureName, string testName)
        {
            var listener = new TestRunEventListener();
            listener.RunStarted();

            // If no fixture name is specified execute all the tests.
            if (String.IsNullOrEmpty(fixtureName))
            {
                foreach (var fixture in _fixtures)
                    ExecuteFixture(fixture, listener);
            }
            // If there is a fixture specified but no test, run all the tests in that fixture.
            else if (String.IsNullOrEmpty(testName))
            {
                var fixture = FindFixture(fixtureName);
                ExecuteFixture(fixture, listener);
            }
            // If both a fixture and a test is specified then execute only the specified test.
            else
            {
                var fixture = FindFixture(fixtureName);
                var fixtureInstance = InstantiateFixture(fixture);

                var method = fixture.Tests.FirstOrDefault(m => string.Compare(m.Name, testName, true) == 0);
                if (method == null)
                    throw new InvalidOperationException("Invalid integration test " + testName);

                InvokeTest(fixtureInstance, fixtureName, method, listener);
            }

            listener.RunFinished();
            return listener.Results;
        }

        private void ExecuteFixture(NUnitTestFixture fixture, TestRunEventListener listener)
        {
            var fixtureInstance = InstantiateFixture(fixture);
            foreach (var method in fixture.Tests)
                InvokeTest(fixtureInstance, fixture.Name, method, listener);
        }

        private void InvokeTest(object fixtureInstance, string fixtureName, MethodInfo method, TestRunEventListener listener)
        {
            listener.TestStarted(fixtureName, method.Name);
            try
            {
                method.Invoke(fixtureInstance, null);
            }
            catch (TargetInvocationException e)
            {
                var assertException = e.InnerException as AssertionException;
                if (assertException != null)
                    listener.TestFinished(true, false, assertException.Message, null);
                else
                {
                    var executorException = e.InnerException as ExecutorScopeException;
                    var realException = (executorException != null) ? executorException.InnerException : e.InnerException;
                    listener.TestFinished(true, true, realException.Message, realException.ToString());
                }

                return;
            }

            listener.TestFinished(false, false, null, null);
        }

        public IList<TestFixtureDto> LoadTestFixtures()
        {
            return _fixtures.Select<NUnitTestFixture, TestFixtureDto>(f =>
            {
                var dto = new TestFixtureDto { Name = f.Name};
                if (f.Tests != null)
                    dto.Tests = f.Tests.Select<MethodInfo, string>(m => m.Name).ToList();
                else
                    dto.Tests = new List<String>();

                return dto;
            }).ToList();
        }
                
        private NUnitTestFixture FindFixture(string fixtureName)
        {
           var fixture = _fixtures.Single(f => String.Compare(f.Type.FullName, fixtureName, true) == 0);
           if (fixture == null)
              throw new ArgumentException("Invalid IntegrationFixture " + fixtureName);

            return fixture;
        }

        private object InstantiateFixture(NUnitTestFixture fixture)
        {
            try
            {
                return Activator.CreateInstance(fixture.Type);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not instantiate IntegrationFixture " + fixture.Type.FullName, e);
            }
        }        
    }    
}
