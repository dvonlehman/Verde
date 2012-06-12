using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Verde
{
    /// <summary>
    /// Represents the results of a test run.
    /// </summary>
    public class ResultsDto
    {
        public ResultsDto()
        {
            this.Tests = new List<TestResultDto>();
        }

        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// The duration of the test run in ms.
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Indicates if the test suite failed.
        /// </summary>
        public bool Failed { get; set; }
        /// <summary>
        /// List of the results for each test executed.
        /// </summary>
        public IList<TestResultDto> Tests { get; private set; }
    }

    /// <summary>
    /// Represents the results of an individual test.
    /// </summary>
    public class TestResultDto
    {
        /// <summary>
        /// The name of the test.
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// The name of the fixture
        /// </summary>
        public string Fixture { get; set; }

        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// The duration of the test in ms.
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Indicates if the test failed.
        /// </summary>
        public bool Failed { get; set; }
        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }
    }

    public class TestFixtureDto
    {
        public string Name { get; set; }
        public IList<string> Tests { get; set; }
    }
}
