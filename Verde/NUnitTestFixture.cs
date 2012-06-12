using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;

namespace Verde
{
    internal class NUnitTestFixture
    {
        private readonly Type _type;
        private readonly int _sequence;
        private readonly IList<MethodInfo> _testMethods; 

        public NUnitTestFixture(Type type, int sequence)
        {
            _type = type;
            _sequence = sequence;

            // If no sequence is specified, force it to the end of the list.
            if (_sequence == 0)
                _sequence = Int32.MaxValue;

            _testMethods = new List<MethodInfo>();
            foreach (var method in type.GetMethods())
            {
               if (HasAttribute<IntegrationTestAttribute>(method))
               {
                EnsureProperSignature(method);
                _testMethods.Add(method);
               }
            }
        }

        public Type Type 
        { 
            get { return _type; } 
        }

        public string Name 
        { 
            get { return _type.FullName; } 
        }

        public int Sequence 
        { 
            get { return _sequence; } 
        }

        public IList<MethodInfo> Tests
        {
            get { return _testMethods; }
        }
                
        private static bool HasAttribute<T>(MethodInfo method) where T : Attribute
        {
            return method.GetCustomAttributes(typeof(T), false).Length > 0;
        }

        private static void EnsureProperSignature(MethodInfo method)
        {
            if (method.ReturnType != typeof(void))
                throw new InvalidOperationException(String.Format(
                    "Method {0}.{1} has a return value.", 
                    method.DeclaringType.FullName, method.Name));

            if (method.GetParameters().Length > 0)
                throw new InvalidOperationException(string.Format(
                    "IntegrationTest {0}.{1} has parameters specified.",
                    method.DeclaringType.FullName, method.Name));
        }
    }
}
