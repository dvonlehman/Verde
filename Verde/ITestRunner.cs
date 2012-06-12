using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verde
{
    public interface ITestRunner
    {
        ResultsDto Execute(string fixtureName, string testName);
        
        IList<TestFixtureDto> LoadTestFixtures();
    }
}
