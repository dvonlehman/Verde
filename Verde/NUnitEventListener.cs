using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace Verde
{
    internal class NUnitEventistener : EventListener
    {
        private readonly ResultsDto _result;

        public NUnitEventistener()
        {
            _result = new ResultsDto();
        }

        void EventListener.RunFinished(Exception exception)
        {
        }

        void EventListener.RunFinished(NUnit.Core.TestResult result)
        {
            _result.EndTime = DateTime.Now;
            _result.Duration = Convert.ToInt32(new TimeSpan(_result.EndTime.Ticks - _result.StartTime.Ticks).TotalMilliseconds);
        }

        void EventListener.RunStarted(string name, int testCount)
        {
            _result.StartTime = DateTime.Now;
        }

        void EventListener.SuiteFinished(NUnit.Core.TestResult result)
        {
        }

        void EventListener.SuiteStarted(TestName testName)
        {
        }

        void EventListener.TestFinished(NUnit.Core.TestResult result)
        {
            if (result.IsFailure || result.IsError)
                RecordTestFinished(true, result.Message + "\r\n" + result.StackTrace);
            else
                RecordTestFinished(false, "Passed");
        }

        void EventListener.TestOutput(TestOutput testOutput)
        {
        }

        void EventListener.TestStarted(TestName testName)
        {
            _result.Tests.Add(new TestResultDto { StartTime = DateTime.Now, TestName = testName.FullName });
        }

        void EventListener.UnhandledException(Exception exception)
        {
            RecordTestFinished(true, "Unhandled exception\r\n" + exception.ToString());
        }

        private void RecordTestFinished(bool failed, string message)
        {
            // If a single test in the suite fails, the overall result should be marked as a failure.
            if (failed)
                _result.Failed = true;

            var testResult = _result.Tests[_result.Tests.Count - 1];
            testResult.EndTime = DateTime.Now;
            testResult.Duration = Convert.ToInt32(new TimeSpan(testResult.EndTime.Ticks - testResult.StartTime.Ticks).TotalMilliseconds);
            testResult.Failed = failed;
            testResult.Message = message;
        }

        public ResultsDto Results
        {
            get { return _result; }
        }
    }
}
