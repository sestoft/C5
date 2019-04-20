// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using C5;
using NUnit.Framework;
using SCG = System.Collections.Generic;

namespace C5UnitTests.RecordsTests
{
  [TestFixture]
  public class Basic
  {
    [SetUp]
    public void Init()
    {
    }
    [Test]
    public void FourElement()
    {
      Rec<string, string, int, int> rec1, rec2, rec3;
      rec1 = new Rec<string, string, int, int>("abe", null, 0, 1);
      rec2 = new Rec<string, string, int, int>("abe", null, 0, 1);
      rec3 = new Rec<string, string, int, int>("abe", "kat", 0, 1);
      Assert.IsTrue(rec1 == rec2);
      Assert.IsFalse(rec1 != rec2);
      Assert.IsFalse(rec1 == rec3);
      Assert.IsTrue(rec1 != rec3);
      Assert.IsTrue(rec1.Equals(rec2));
      Assert.IsFalse(rec1.Equals(rec3));
      //
      Assert.IsFalse(rec1.Equals(null));
      Assert.IsFalse(rec1.Equals("bamse"));
      //
      Assert.IsTrue(rec1.GetHashCode() == rec2.GetHashCode());
      Assert.IsFalse(rec1.GetHashCode() == rec3.GetHashCode());
      //
      Assert.AreEqual("abe", rec1.X1);
      Assert.IsNull(rec1.X2);
      Assert.AreEqual(0, rec1.X3);
      Assert.AreEqual(1, rec1.X4);
    }


    [Test]
    public void ThreeElement()
    {
      Rec<string, string, int> rec1, rec2, rec3;
      rec1 = new Rec<string, string, int>("abe", null, 0);
      rec2 = new Rec<string, string, int>("abe", null, 0);
      rec3 = new Rec<string, string, int>("abe", "kat", 0);
      Assert.IsTrue(rec1 == rec2);
      Assert.IsFalse(rec1 != rec2);
      Assert.IsFalse(rec1 == rec3);
      Assert.IsTrue(rec1 != rec3);
      Assert.IsTrue(rec1.Equals(rec2));
      Assert.IsFalse(rec1.Equals(rec3));
      //
      Assert.IsFalse(rec1.Equals(null));
      Assert.IsFalse(rec1.Equals("bamse"));
      //
      Assert.IsTrue(rec1.GetHashCode() == rec2.GetHashCode());
      Assert.IsFalse(rec1.GetHashCode() == rec3.GetHashCode());
      //
      Assert.AreEqual("abe", rec1.X1);
      Assert.IsNull(rec1.X2);
      Assert.AreEqual(0, rec1.X3);

    }

    [Test]
    public void TwoElement()
    {
      Rec<string, string> rec1, rec2, rec3;
      rec1 = new Rec<string, string>("abe", null);
      rec2 = new Rec<string, string>("abe", null);
      rec3 = new Rec<string, string>("abe", "kat");
      Assert.IsTrue(rec1 == rec2);
      Assert.IsFalse(rec1 != rec2);
      Assert.IsFalse(rec1 == rec3);
      Assert.IsTrue(rec1 != rec3);
      Assert.IsTrue(rec1.Equals(rec2));
      Assert.IsFalse(rec1.Equals(rec3));
      //
      Assert.IsFalse(rec1.Equals(null));
      Assert.IsFalse(rec1.Equals("bamse"));
      //
      Assert.IsTrue(rec1.GetHashCode() == rec2.GetHashCode());
      Assert.IsFalse(rec1.GetHashCode() == rec3.GetHashCode());
      //
      Assert.AreEqual("abe", rec1.X1);
      Assert.IsNull(rec1.X2);
    }

    [TearDown]
    public void Dispose()
    {
    }
  }

}