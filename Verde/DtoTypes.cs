using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verde
{
    public class ResultsDto
    {
        public ResultsDto()
        {
            this.Tests = new List<TestResultDto>();
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public bool Failed { get; set; }
        public IList<TestResultDto> Tests { get; private set; }
    }

    public class TestResultDto
    {
        public string TestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Duration { get; set; }
        public bool Failed { get; set; }
        public string Message { get; set; }
    }

    public class TestFixtureDto
    {
        public string Name { get; set; }
        public IList<string> Tests { get; set; }
    }
}
