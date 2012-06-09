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

        public NUnitTestFixture(Type type, int sequence)
        {
            _type = type;
            _sequence = sequence;

            // If no sequence is specified, force it to the end of the list.
            if (_sequence == 0)
                _sequence = Int32.MaxValue;

            foreach (var method in type.GetMethods())
            {
                if (HasAttribute<TestFixtureSetUpAttribute>(method))
                {
                    EnsureProperSignature(method);
                    if (FixtureSetups == null) FixtureSetups = new List<MethodInfo>();
                    FixtureSetups.Add(method);
                }
                else if (HasAttribute<TestFixtureTearDownAttribute>(method))
                {
                    EnsureProperSignature(method);
                    if (FixtureTearDowns == null) FixtureTearDowns = new List<MethodInfo>();
                    FixtureTearDowns.Add(method);
                }
                else if (HasAttribute<SetUpAttribute>(method))
                {
                    EnsureProperSignature(method);
                    if (Setups == null) Setups = new List<MethodInfo>();
                    Setups.Add(method);
                }
                else if (HasAttribute<TearDownAttribute>(method))
                {
                    EnsureProperSignature(method);
                    if (Teardowns == null) Teardowns = new List<MethodInfo>();
                    Teardowns.Add(method);
                }
                else if (HasAttribute<IntegrationTestAttribute>(method))
                {
                    EnsureProperSignature(method);
                    if (Tests == null) Tests = new List<MethodInfo>();
                    Tests.Add(method);
                }
            }
        }

        public Type Type { get { return _type; } }

        public string Name { get { return _type.FullName; } }

        public int Sequence { get { return _sequence; } }

        public IList<MethodInfo> FixtureSetups { get; private set; }
        public IList<MethodInfo> FixtureTearDowns { get; private set; }
        public IList<MethodInfo> Setups { get; private set; }
        public IList<MethodInfo> Teardowns { get; private set; }
        public IList<MethodInfo> Tests { get; private set; }
                
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
