# Release Notes for C5 Generic Collection Library for C#/CLI

## Release 3.0.0 of 2024-06-05

- C5 now targets .NET Standard 2.0, .NET 6.0 and .NET 8.0.
- Breaking change: `Rec<T1, T2, ...>` type removed. Use `ValueTuple<T1, T2, ...>` instead.
- Breaking change: All `public readonly` fields converted to properties.
- Breaking change: `EventTypeEnum` is now `EventType`.
- Breaking change: custom `KeyValuePair<K, V>` has been replaced by the standard `System.Collections.Generic.KeyValue<TKey, TValue>` for better compatibility.
- Breaking change: `EnumerationDirection` is now `Direction`.
- Breaking change: `[Serializable]` attribute removed. Cf. [BinaryFormatter security guide](https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide).

## Release 3.0.0-rc of 2021-12-22

- C5 now targets .NET Standard 2.0 and .NET 6.0.
- Strong name removed.
- Memory safe optimizer removed as they are no longer needed for Unity development.
- Breaking change: All `public readonly` are now properties.
- Breaking change: `EventTypeEnum` is now `EventType`.
- Breaking change: custom `KeyValuePair<K, V>` has been replaced by the standard `System.Collections.Generic.KeyValue<TKey, TValue>` for better compatibility.
- Deprecation warning: As per [BinaryFormatter security guide](https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide), binary deserialization present high security risk. For this reason the `[Serializable]` attribute will be removed from the entire library in the next release.

## Release 2.5 of 2017-07-07

- Support for .NET Standard and .NET Core thanks to Andrew Arnott (<https://github.com/AArnott>)

## Release 2.4.5891 of 2016-02-17

- Memory safe enumerators for mobile development courtesy of nbasakuragi

## Release 2.4.5828 of 2015-12-15

- KeyValuePair Serializable
- KeyValuePair.Create using type inference
- Added .NET 4.5 version: `IIndexed<T>` implements `IReadOnlyList<T>`
- Fixed typos.

## Release 2.4 of 2015-12-15

- Support for Universal Windows Platform

## Release 2.3.0.0 of 2014-06-10

- Added .NET 4.0 build which reintroduces support for serialization
- Added .NET 3.5 build

## Release 2.2.5073.27396 of 2013-11-21

- Update to PCL profile in NuGet package

## Release 2.2.4822.42600 of 2013-03-15

- Mono support is now built-in as Mono 3.0.6 and Xamarin Studio supports portable libraries.
- The C5 assembly is now strong named.

## Release 2.1.4492.18142 of 2012-04-19

- Full support for Mono.
- C5 can now be built for Mono using the `C5.Mono.sln` file.

## Release 2.0.4398.21073 of 2012-01-16

- Included .xml file in build.
- Includes System.Core from the Portable Library Tools in build.
- Fixed binary search bug in `SortedArray`.

## Release 2.0.0 of 2011-05-23

- The new .NET 4.0, Silverlight, and Windows Phone version.
   It is not backwards compatible with the 1.0 branch.

## Release 1.1.1 of 2010-12-17

- Fixed 5 bugs found since the 1.1.0 release.  Only one of this is likely to break code: the specification and implementation of the range indexer `this[i,n]` on `TreeSet` was wrong.

## Release 1.1.0 of 2008-02-10

New functionality:

- Interface `C5.ICollection<T>` now extends generic interface `System.Collections.Generic.ICollection<T>`. The `C5.ICollection<T>` interface in some cases describe different exceptions than specified by `SCG.ICollection<T>`, but we have not attempted to fix this because the .NET collection implemented in some cases throw other exceptions than those specified anyway.
- Interface `C5.IList<T>` now extends interface non-generic interface `System.Collections.IList`, so C5 list collections can be passed to .NET GUI components and other framework methods.
- Exception-free methods

  - `bool TryPredecessor(T x, out T res)`
  - `bool TrySuccessor(T x, out T res)`
  - `bool TryWeakPredecessor(T x, out T res)`
  - `bool TryWeakSuccessor(T x, out T res)`

  have been added to the `ISorted<T>` interface and the classes that implement it.

- Added methods analogous to the above to `ISortedDictionary<K,V>` and the classes that implement it.
- Event raising on `SortedDictionary<T>` finally implemented, thanks to Marcus Griep.  Hence all unit tests should now succeed.
- The missing custom comparers and equality comparers have been added for all primitive types except bool.
- The book "The C5 Generic Collection Library" has been updated to reflect these changes.

Bugs fixed:

- `SortedArray<T>.UpdateOrAdd` and `SortedArray<T>.FindOrAdd` did not expand the underlying array correctly
- `HashDictionary.UpdateOrAdd` returned the new value, not the old one.
- `CollectionBase.StaticEquals` threw exception when exactly one argument was `null`.
- `HashedLinkedList<T>.Remove` could fail with `NullReferenceException`.
- `HashSet<T>.UpdateOrAdd(item, out old)` did not set `old=default(T)` when item was not already in the set.
- `HashBag<T>.CopyTo` could throw when copying from empty collection.

## Release 1.0.2 of 2007-06-01

Bugs fixed:

- `SortedDictionaryBase` was not marked as `[Serializable]`
- `ArrayList.expand` did not update the array field of (other) views
- `IntervalHeap::Replace` would throw an exception on one-element heap

## Release 1.0.1 of 2006-06-27

Bugs fixed:

- `CircularQueue<T>` indexer was wrong
- Some equality comparers created by `C5.EqualityComparer<T>.Default` were not marked serializable
- `HashSet<T>.RetainAll` could leave internal data inconsistent
- `TreeDictionary<K,V>` was not marked serializable
- `HashedLinkedList<T>` problem related to tag group implementation
- `Dispose()` could fail on newly created lists

New features:

- A strong name (.snk) is included
- `C5.IList<T>` now derives from `System.Collections.Generic.IList<T>`
- Added C5.build file for NAnt, due to Johan Warlander

The technical report has been updated to reflect the above changes.

## Release 1.00 of 2006-01-30

First complete release.

There are lots of changes since the PreRelease:

- Interface design has been reorganized and simplified
- New functionality added (too much to describe here)
- Updated for Microsoft C#/CLI 2.0 release version
- Comprehensive documentation in ITU Technical Report ITU-TR-2006-76

## PreRelease 0.5 of 2004-08-06

First public release, essentially a preview of the library, for beta 1 of Microsoft C#/CLI 2.0.
