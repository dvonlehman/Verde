using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verde
{
    internal class TestRunEventListener
    {
        private readonly ResultsDto _result;

        public TestRunEventListener()
        {
            _result = new ResultsDto();
        }

        public void RunStarted()
        {
            _result.StartTime = DateTime.Now;
        }

        public void RunFinished()
        {
            _result.EndTime = DateTime.Now;
            _result.Duration = Convert.ToInt32(new TimeSpan(_result.EndTime.Ticks - _result.StartTime.Ticks).TotalMilliseconds);
        }

        public void TestFinished()
        {
           RecordTestFinished(false, "Passed");
        }

        public void TestFinished(bool isFailure, bool isError, string message, string stackTrace)
        {
			if (isFailure || isError)
			{
				RecordTestFinished(true, message + "\r\n" + stackTrace);
			}
			else
			{
				//To retrieve a message from 'SuccessException' (Assert.Fail)
				string returnedMessage = string.IsNullOrWhiteSpace(message) ? "Passed" : message;
				RecordTestFinished(false, returnedMessage);
			}
        }

        public void TestStarted(string fixtureName, string testName)
        {
            _result.Tests.Add(new TestResultDto { 
                StartTime = DateTime.Now, 
                Fixture=fixtureName, 
                TestName = testName });
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
