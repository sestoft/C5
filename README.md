# Welcome to The C5 Generic Collection Library for C# and CLI

The C5 library is a set of generic collection classes (or container classes)
for the C# programming language and other generics-enabled languages on
version 4.0 of the CLI platform, as implemented by Microsoft .NET and Mono.

The C5 library provides a wide range of classic data structures, rich
funcionality, the best possible asymptotic time complexity, documented
performance, and a thoroughly tested implementation.

## Goals of the C5 library

The overall goal is for C5 to be a generic collection library for the C#
programming language and the Common Language Infrastructure (CLI) whose 
functionality, efficiency and quality meets or exeeds what is available for 
similar contemporary programming platforms. 

The design has been influenced by the collection libraries for Java and SmallTalk
and the published critique of these.

However, it contains functionality and a regularity of design that considerably
exceeds that of the standard libraries for those languages.

## Why yet another generic collection library

There are already other generic collection libraries for C#/CLI, including the
System.Collections.Generic namespace introduced with .NET 2.0 and Wintellect's 
Power Collections for .NET by Peter Golde (http://powercollections.codeplex.com/).

The CLI generic collection library as implemented by in Microsoft .NET Framework 2.0
provides a limited choice of data structures.

In general, the CLI Framework library has a proliferation of methods variants and 
rather poor orthogonality.

Collection implementations such as array lists and linked lists have much the same
functionality but do not implement a common interface. This impairs the learnability 
of the library in contexts where nano-second efficiency is more important that rich
functionality, and the need to support also rather resource-constrained run-time
systems.

The Power Collections library by Peter Golde augments the CLI version 2.0 collection
library with various data structures and algorithms. 

However, it accepts the basic design of the CLI collection classes and therefore
suffers from some of the same shortcommings, and also does not provide most of the
advanced functionality (updatable views, snapshots, directed enumeration, priority 
queue handles, ...) of C5.

Thus, in our opinion, C5 provides the most powerful, well-structured and scalable
generic collections library available for C#/CLI. 

## What does the name C5 stand for?

This is not entirely clear, but it may stand for <em>Copenhagen Comprehensive
Collection Classes for C#</em>, although the library may be used from VB.NET, F#
and other CLI languages, not just C#. It has nothing to do with a Microsoft
Dynamics product that used to be called Concorde C5/Damgaard C5/Navision C5, nor a 
real-time operation system called C5 (or Chorus), nor the C5 Corporation (system
visualization), nor an Eclipse plug-in called C5, nor with cars such as the Citroën 
C5 or Corvette C5 or Clive Sinclair's ill-fated C5 concept vehicle.

The name may be inspired by the versatile C4 plastic explosive known from e.g.
James Bond movies.

All trademarks belong to their owners.

## State of completion

At the time of writing, library design and implementation are complete, and extensive
unit tests have been written and applied systematically.

Most of the library API documentation is in place but requires proof-reading.

The C5 implementation was originally built on .NET 2.0 and has only recently been 
upgraded to .NET 4.0. It does not (yet) use a great number of the new features 
introduced with C# 3 and 4, notably LINQ and covariant and contravariant type parameters.

C5 builds and runs on the latest versions of Mono.

C5 is a portable library supporting .NET 4.0, Silverlight, Windows Phone 7, and Xbox 360.

## Getting Started

1. Get C5 from NuGet: <tt>PM> Install-Package C5</tt>

2. C5 is built using the Portable Library Tools extension 
   (http://visualstudiogallery.msdn.microsoft.com/b0e0b5e9-e138-410b-ad10-00cb3caf4981)
   this extension is required to open the C5.csproj in Visual Studio.

3. Documentation can be built using Sandcastle (http://sandcastle.codeplex.com/) and
   Sandcastle Help File Builder (http://shfb.codeplex.com/), and the
   <tt>C5.shfbproj</tt> file.
 
4. Building the unit test project requires NUnit. If you have NuGet installed it 
   should automatically add the reference.
   
   To run the tests, start NUnit, open project C5/C5.Tests/bin/Debug/C5.Tests.dll, 
   press Run.

   There are more than 1430 NUnit test cases which should execute in less
   than 30 seconds. All should pass.
   
## C5 on Mono

1. Open C5.csproj with Xamarin Studio (Mono 3.0) to build C5.dll or

2. Build manually using <tt>xbuild C5.Mono.csproj</tt>.