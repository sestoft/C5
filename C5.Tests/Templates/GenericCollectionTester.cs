// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE for licensing details.

using NUnit.Framework;
using System;
using System.Reflection;
using SCG = System.Collections.Generic;

namespace C5.Tests.Templates
{
    public abstract class GenericCollectionTester<U, W>
    {
        protected CircularQueue<MethodInfo> testMethods;
        public GenericCollectionTester()
        {
            testMethods = new CircularQueue<MethodInfo>();
            foreach (MethodInfo minfo in GetType().GetMethods())
            {
                if (minfo.GetParameters().Length == 0 &&
                     minfo.GetCustomAttributes(typeof(TestAttribute), false).Length > 0)
                {
                    testMethods.Enqueue(minfo);
                }
            }
        }

        public virtual void Test(Func<U> factory)
        {
            foreach (MethodInfo minfo in testMethods)
            {
                foreach (W testSpec in GetSpecs())
                {
                    SetUp(factory(), testSpec);
                    //Console.WriteLine("Testing {0}, with method {1} and testSpec {{{2}}}", typeof(U), minfo.Name, testSpec);
                    try
                    {
                        minfo.Invoke(this, null);
                    }
                    catch (TargetInvocationException)
                    {
                        //if (e.InnerException is ExpectedExceptionAttribute)
                        //{
                        //}
                        //else
                        throw;
                    }
                    //tearDown
                }
            }
        }

        public abstract void SetUp(U collection, W testSpec);
        public abstract SCG.IEnumerable<W> GetSpecs();
    }

    public abstract class GenericCollectionTester<U> : GenericCollectionTester<U, int>
    {
        public override SCG.IEnumerable<int> GetSpecs()
        {
            return [0];
        }

        public override void SetUp(U collection, int testSpec)
        {
            SetUp(collection);
        }

        public abstract void SetUp(object collection);
    }
}