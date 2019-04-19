// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example: WrappedArray 2005-07-21

// Compile with 
//   csc /r:C5.dll WrappedArray.cs 

using System;
using C5;
using SCG = System.Collections.Generic;

namespace WrappedArray {
  class WrappedArray {
    public static void Main(String[] args) {
    }


    // System.Array.Exists

    public static bool Exists<T>(T[] arr, Func<T,bool> p) {
      return new WrappedArray<T>(arr).Exists(p);
    }  

    // System.Array.TrueForAll

    public static bool TrueForAll<T>(T[] arr, Func<T,bool> p) {
      return new WrappedArray<T>(arr).All(p);
    }  

    // System.Array.Find(T[], Predicate)
    // This loses the valuable bool returned by C5 Find.

    public static T Find<T>(T[] arr, Func<T,bool> p) {
            new WrappedArray<T>(arr).Find(p, out T res);
            return res;
    }  

    // System.Array.FindAll(T[], Predicate)

    public static T[] FindAll<T>(T[] arr, Func<T,bool> p) {
      return new WrappedArray<T>(arr).FindAll(p).ToArray();
    }  

    // System.Array.FindIndex(T[], Predicate)

    public static int FindIndex<T>(T[] arr, Func<T,bool> p) {
      return new WrappedArray<T>(arr).FindIndex(p);
    }  

    // System.Array.FindIndex(T[], int, Predicate)

    public static int FindIndex<T>(T[] arr, int i, Func<T,bool> p) {
      int j = new WrappedArray<T>(arr).View(i,arr.Length-i).FindIndex(p);
      return j < 0 ? j : j+i;
    }  

    // System.Array.FindIndex(T[], int, int, Predicate)

    public static int FindIndex<T>(T[] arr, int i, int n, Func<T,bool> p) {
      int j = new WrappedArray<T>(arr).View(i,n).FindIndex(p);
      return j < 0 ? j : j+i;
    }  

    // System.Array.FindLast(T[], Predicate)
    // This loses the valuable bool returned by C5 Find.

    public static T FindLast<T>(T[] arr, Func<T,bool> p) {
            new WrappedArray<T>(arr).FindLast(p, out T res);
            return res;
    }  

    // System.Array.FindLastIndex(T[], Predicate)

    public static int FindLastIndex<T>(T[] arr, Func<T,bool> p) {
      return new WrappedArray<T>(arr).FindIndex(p);
    }  

    // System.Array.FindLastIndex(T[], int, Predicate)

    public static int FindLastIndex<T>(T[] arr, int i, Func<T,bool> p) {
      int j = new WrappedArray<T>(arr).View(i,arr.Length-i).FindIndex(p);
      return j < 0 ? j : j+i;
    }  

    // System.Array.FindLastIndex(T[], int, int, Predicate)

    public static int FindLastIndex<T>(T[] arr, int i, int n, Func<T,bool> p) {
      int j = new WrappedArray<T>(arr).View(i,n).FindIndex(p);
      return j < 0 ? j : j+i;
    }  
    
    // System.Array.ForEach(T[], Action)

    public static void ForEach<T>(T[] arr, Action<T> action) {
      new WrappedArray<T>(arr).Apply(action);
    }  

    // System.Array.IndexOf(T[], T)

    public static int IndexOf<T>(T[] arr, T x) {
      int j = new WrappedArray<T>(arr).IndexOf(x);
      return j < 0 ? -1 : j;
    }  
    
    // System.Array.IndexOf(T[], T, int)

    public static int IndexOf<T>(T[] arr, T x, int i) {
      int j = new WrappedArray<T>(arr).View(i, arr.Length-i).IndexOf(x);
      return j < 0 ? -1 : j+i;
    }  
    
    // System.Array.IndexOf(T[], T, int, int)

    public static int IndexOf<T>(T[] arr, T x, int i, int n) {
      int j = new WrappedArray<T>(arr).View(i, n).IndexOf(x);
      return j < 0 ? -1 : j+i;
    }  

    // System.Array.LastIndexOf(T[], T)

    public static int LastIndexOf<T>(T[] arr, T x) {
      int j = new WrappedArray<T>(arr).LastIndexOf(x);
      return j < 0 ? -1 : j;
    }  
    
    // System.Array.LastIndexOf(T[], T, int)

    public static int LastIndexOf<T>(T[] arr, T x, int i) {
      int j = new WrappedArray<T>(arr).View(i, arr.Length-i).LastIndexOf(x);
      return j < 0 ? -1 : j+i;
    }  
    
    // System.Array.LastIndexOf(T[], T, int, int)

    public static int LastIndexOf<T>(T[] arr, T x, int i, int n) {
      int j = new WrappedArray<T>(arr).View(i, n).LastIndexOf(x);
      return j < 0 ? -1 : j+i;
    }  

    // System.Array.Sort(T[])

    public static void Sort<T>(T[] arr) {
      new WrappedArray<T>(arr).Sort();
    }  

    // System.Array.Sort(T[], int, int)

    public static void Sort<T>(T[] arr, int i, int n) {
      new WrappedArray<T>(arr).View(i, n).Sort();
    }  

    // System.Array.Sort(T[], SCG.IComparer<T>)

    public static void Sort<T>(T[] arr, SCG.IComparer<T> cmp) {
      new WrappedArray<T>(arr).Sort(cmp);
    }  
    
    // System.Array.Sort(T[], int, int, SCG.IComparer<T>)

    public static void Sort<T>(T[] arr, int i, int n, SCG.IComparer<T> cmp) {
      new WrappedArray<T>(arr).View(i, n).Sort(cmp);
    }  
  }
}
