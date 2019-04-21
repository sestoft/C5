// This file is part of the C5 Generic Collection Library for C# and CLI
// See https://github.com/sestoft/C5/blob/master/LICENSE.txt for licensing details.

// C5 example sketch -- not so interesting after all
// 2004-08

// KWIC = keyword-in-context.

// Read a file of words and create an ordered list of keywords, giving
// for each keyword a list of the contexts in which it appears.  For
// instance, for this text

This book presents C# version 2.0 as used in Microsoft Visual Studio
2005, including generics, iterators, anonymous methods and partial
type declarations, but excluding most of Microsoft's .Net Framework
class libraries except threads, input-output, and generic collection
classes.  The book does not cover unsafe code, destructors,
finalization, reflection, pre-processing directives (#define,
#if) or details of \textsc{ieee754} floating-point
numbers.

// the resulting list may look like this:
// book 
//   this book presents
//   the book does
// Microsoft
//   in Microsoft Visual
//   of Microsoft's .Net
  
// How to proceed: (1) read a stream of words from file; (2) turn this
// into a stream of (2n+1)-tuples of words, namely a keyword
// surrounded by n words on either side; (3) create a dictionary
// mapping each keyword to a set of its contexts; (4) output the
// entries of the dictionary sorted by keyword, for each keyword
// building an HTML <item> list or similar.

// Step (2) can be improved by not generating (2n+1)-tuples for
// keywords that are stop words (in, of, the, a, ...).  Also, each
// such triple may have an associated line number or page number on
// which it appears, in which case the dictionary should map to a
// dictionary that maps each context to a list of line numbers or page
// numbers.

using System;
using C5;
using SCG = System.Collections.Generic;

class MyTest {
  public static void Main(String[] args) {

  }
}
