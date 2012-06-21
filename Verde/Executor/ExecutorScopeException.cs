using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Verde.Executor
{
    /// <summary>
    /// Wrapper exception around any exception thrown from by code executing within an <see cref="ExecutorScope"/>.
    /// </summary>
    public class ExecutorScopeException : Exception
    {
        public ExecutorScopeException(Exception innerException) : base("Exception thrown from within the ExecutorScope.", innerException)
        {
        }
    }
}
