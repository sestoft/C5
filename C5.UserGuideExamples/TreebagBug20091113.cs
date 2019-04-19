// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 TreeBag bug

// Compile with 
//   csc /r:C5.dll TreebagBug20091113.cs 

using System;
using System.Text;
using C5;
using SCG = System.Collections.Generic;

namespace Try
{
  class MyTest
  {
    public static void Main(String[] args)
    {
      var list = new TreeBag<long>();
      // Sequence generated in FindNodeRandomTest
      list.Add(553284);
      list.Add(817435);
      list.Remove(817435);
      list.Add(155203);
      list.Add(316201);
      list.Remove(316201);
      list.Add(145375);
      list.Remove(155203);
      list.Add(155203);
      list.Add(150788);
      list.Remove(145375);
      list.Add(316201);
      list.Add(263469);
      list.Add(263469);
      list.Add(441406);
      list.Add(553284);
      list.Remove(553284);
      list.Add(553284);
      list.Remove(150788);
      list.Add(769005);
      list.Add(263469);
      list.Remove(316201);
      list.Add(553284);
      list.Remove(769005);
      list.Add(316201);
      list.Remove(263469);
      list.Add(817435);
      list.Add(553284);
      list.Remove(316201);
      list.Add(150788);

//        list.Add(0.553284f);
//        list.Add(0.8174357f);
//        list.Remove(0.8174357f);
//        list.Add(0.1552035f);
//        list.Add(0.3162012f);
//        list.Remove(0.3162012f);
//        list.Add(0.1453752f);
//        list.Remove(0.1552035f);
//        list.Add(0.1552035f);
//        list.Add(0.1507881f);
//        list.Remove(0.1453752f);
//        list.Add(0.3162012f);
//        list.Add(0.2634694f);
//        list.Add(0.2634694f);
//        list.Add(0.4414065f);
//        list.Add(0.553284f);
//        list.Remove(0.553284f);
//        list.Add(0.553284f);
//        list.Remove(0.1507881f);
//        list.Add(0.7690055f);
//        list.Add(0.2634694f);
//        list.Remove(0.3162012f);
//        list.Add(0.553284f);
//        list.Remove(0.7690055f);
//        list.Add(0.3162012f);
//        list.Remove(0.2634694f);
//        list.Add(0.8174357f);
//        list.Add(0.553284f);
//        list.Remove(0.3162012f);
//        list.Add(0.1507881f);


      Console.WriteLine(list);
      Console.WriteLine(list.Count > 3);
       Console.WriteLine(list[0]);
       Console.WriteLine(list[1]);
       Console.WriteLine(list[2]);  // Throws!
//        Console.WriteLine(list[0,1]);
//        Console.WriteLine(list[1,1]);
//        Console.WriteLine(list[2,1]);    // Worse: Throws too!
    }
  }
}

/* 


>     Assert.IsTrue(list.Count > 3);
>     Assert.IsNotNull(list[0]);
>     Assert.IsNotNull(list[1]);
>     Assert.IsNotNull(list[2]);
> 
> throws:
> System.NullReferenceException : Object reference not set to an instance
> of an object.
> at C5.TreeBag`1.findNode(Int32 i) in
> C:\Code\Libs\C5.src\C5\trees\RedBlackTreeBag.cs:line 2544
> at C5.TreeBag`1.get_Item(Int32 i) in
> C:\Code\Libs\C5.src\C5\trees\RedBlackTreeBag.cs:line 2577
> 
> 
> Could you please confirm if this is a bug with C5.TreeBag?
> Much appreciated, thank you.
> 
> --
> Regards,
> Keith Nelson
> 



    [ Part 2: "Attached Text" ]

  [TestFixture]
  public class FindNodeTests
  {
      private TreeBag<float> list;

      [SetUp]
      public void Init()
      {
          list = new TreeBag<float>();
      }

      [TearDown]
      public void Dispose() { list = null; }

      [Test]
      public void FindNode()
      {
          list.AddAll<float>(new float[] { 0.1f, 0.5f, 0.5f, 0.9f });
          Assert.IsTrue(list.Count > 3);
          Assert.IsNotNull(list[0]);
          Assert.IsNotNull(list[1]);
          Assert.IsNotNull(list[2]);
      }

      [Test]
      public void FindNodeException()
      {
          // Sequence generated in FindNodeRandomTest
          list.Add(0.553284f);
          list.Add(0.8174357f);
          list.Remove(0.8174357f);
          list.Add(0.1552035f);
          list.Add(0.3162012f);
          list.Remove(0.3162012f);
          list.Add(0.1453752f);
          list.Remove(0.1552035f);
          list.Add(0.1552035f);
          list.Add(0.1507881f);
          list.Remove(0.1453752f);
          list.Add(0.3162012f);
          list.Add(0.2634694f);
          list.Add(0.2634694f);
          list.Add(0.4414065f);
          list.Add(0.553284f);
          list.Remove(0.553284f);
          list.Add(0.553284f);
          list.Remove(0.1507881f);
          list.Add(0.7690055f);
          list.Add(0.2634694f);
          list.Remove(0.3162012f);
          list.Add(0.553284f);
          list.Remove(0.7690055f);
          list.Add(0.3162012f);
          list.Remove(0.2634694f);
          list.Add(0.8174357f);
          list.Add(0.553284f);
          list.Remove(0.3162012f);
          list.Add(0.1507881f);
          
          Assert.IsTrue(list.Count > 3);
          Assert.IsNotNull(list[0]);
          Assert.IsNotNull(list[1]);
          Assert.IsNotNull(list[2]);
      }

      [Test]
      public void FindNodeRandomTest()
      {
          Random rand = new Random();
          SCG.List<float> fList = new System.Collections.Generic.List<float>();
          string failedSequence;
          for (int k = 0; k < 1000; k++)
          {
              failedSequence = string.Format("--------- FindNodeRandomTest run {0} ------------{1}", k, Environment.NewLine);
              list.Clear();
              fList.Clear();
              for (int j = 0; j < 10; j++)
                  fList.Add((float)rand.NextDouble());

              int length = fList.Count*2;
              float tmp;
              for (int i = 0; i < length; i++)
              {
                  list.Add(tmp = fList[rand.Next(0, fList.Count - 1)]); // Randomly add value from list
                  failedSequence += string.Format("   list.Add({0}f);{1}", tmp, Environment.NewLine);
                  tmp = fList[rand.Next(0, fList.Count - 1)];
                  if (list.Contains(tmp))
                  {
                      list.Remove(tmp); // Randomly remove value from list
                      failedSequence += string.Format("   list.Remove({0}f);{1}", tmp, Environment.NewLine);
                  }
              }
              failedSequence += string.Format("list.Count=={0}{1}", list.Count, Environment.NewLine);
              Assert.IsTrue(list.Count > 3);
              try
              {
                  Assert.IsNotNull(list[0]);
                  Assert.IsNotNull(list[1]);
                  Assert.IsNotNull(list[2]);
              }
              catch (NullReferenceException ne)
              {
                  Console.WriteLine("{0}{1}With test sequence:{1}{2}", ne.ToString(), Environment.NewLine, failedSequence);
                  throw ne;
              }
          }
      }
  }

*/
