using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Core.Filters;

namespace Verde
{
    internal class NUnitTestRunner : ITestRunner
    {
        private Settings _settings;
        private TestSuite _testSuite;
        private TestPackage _testPackage;

        public NUnitTestRunner(Settings settings)
        {
            // Important that this line is executed, otherwise NUnit won't find any TestFixtures or TestCases
            CoreExtensions.Host.InstallBuiltins();

            _testPackage = new TestPackage(settings.TestsAssembly.Location);
            _testPackage.Settings["AutoNamespaceSuites"] = false;
            _testSuite = new TestSuiteBuilder().Build(_testPackage);
        }

        public ResultsDto Execute(string testName)
        {
            var runner = CreateTestRunner();

            var listener = new NUnitEventistener();
            runner.BeginRun(listener, new SimpleNameFilter(testName), false, LoggingThreshold.Off);
            return listener.Results;
        }

        public ResultsDto ExecuteAll()
        {
            return null;
        }

        public IList<TestFixtureDto> LoadTestFixtures()
        {
            var fixtures = new List<TestFixtureDto>();
            foreach (TestFixture fixture in _testSuite.Tests)
            {
                fixtures.Add(new TestFixtureDto
                {
                    Name = fixture.FixtureType.Name,
                    Tests = fixture.Tests.Cast<Test>().Select<Test, string>(t => t.TestName.FullName).ToList()
                });
            }
            return fixtures;
        }

        private TestRunner CreateTestRunner()
        {
            var runner = new SimpleTestRunner();

            if (!runner.Load(_testPackage))
                throw new InvalidOperationException("Could not load the TestSuite");

            if (runner.Test.TestCount == 0)
                throw new InvalidOperationException("No tests found");

            return runner;
        }

    }    
}
