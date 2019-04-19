// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

using System;
using C5;
using NUnit.Framework;
using SCG = System.Collections.Generic;

namespace C5UnitTests.SortingTests
{
	[TestFixture]
	public class SortRandom
	{
		IC ic;

		Random ran;

		int[] a;

		int length;


		[SetUp]
		public void Init()
		{
			ic = new IC();
			ran = new Random(3456);
			length = 100000;
			a = new int[length];
			for (int i = 0; i < length; i++)
				a[i] = ran.Next();
		}


		[Test]
		public void HeapSort()
		{
			Sorting.HeapSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void IntroSort()
		{
			Sorting.IntroSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void InsertionSort()
		{
			length = 1000;
			Sorting.InsertionSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);

			Sorting.InsertionSort<int>(a, length, 2 * length, ic);
			for (int i = length + 1; i < 2 * length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[TearDown]
		public void Dispose() { ic = null; }
	}



	[TestFixture]
	public class SortRandomDuplicates
	{
		IC ic;

		Random ran;

		int[] a;

		int length;


		[SetUp]
		public void Init()
		{
			ic = new IC();
			ran = new Random(3456);
			length = 100000;
			a = new int[length];
			for (int i = 0; i < length; i++)
				a[i] = ran.Next(3, 23);
		}


		[Test]
		public void HeapSort()
		{
			Sorting.HeapSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void IntroSort()
		{
			Sorting.IntroSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void InsertionSort()
		{
			length = 1000;
			Sorting.InsertionSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);

			Sorting.InsertionSort<int>(a, length, 2 * length, ic);
			for (int i = length + 1; i < 2 * length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[TearDown]
		public void Dispose() { ic = null; a = null; ran = null; }
	}



	[TestFixture]
	public class SortIncreasing
	{
		IC ic;

		int[] a;

		int length;


		[SetUp]
		public void Init()
		{
			ic = new IC();
			length = 100000;
			a = new int[length];
			for (int i = 0; i < length; i++)
				a[i] = i;
		}


		[Test]
		public void HeapSort()
		{
			Sorting.HeapSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void IntroSort()
		{
			Sorting.IntroSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void InsertionSort()
		{
			length = 1000;
			Sorting.InsertionSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);

			Sorting.InsertionSort<int>(a, length, 2 * length, ic);
			for (int i = length + 1; i < 2 * length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[TearDown]
		public void Dispose() { ic = null; a = null; }
	}



	[TestFixture]
	public class SortDecreasing
	{
		IC ic;

		int[] a;

		int length;


		[SetUp]
		public void Init()
		{
			ic = new IC();
			length = 100000;
			a = new int[length];
			for (int i = 0; i < length; i++)
				a[i] = -i;
		}


		[Test]
		public void HeapSort()
		{
			Sorting.HeapSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void IntroSort()
		{
			Sorting.IntroSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[Test]
		public void InsertionSort()
		{
			length = 1000;
			Sorting.InsertionSort<int>(a, 0, length, ic);
			for (int i = 1; i < length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);

			Sorting.InsertionSort<int>(a, length, 2 * length, ic);
			for (int i = length + 1; i < 2 * length; i++)
				Assert.IsTrue(a[i - 1] <= a[i], "Inversion at " + i);
		}


		[TearDown]
		public void Dispose() { ic = null; a = null; }
	}
}