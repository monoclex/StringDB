# StringDB [![Build status](https://ci.appveyor.com/api/projects/status/github/SirJosh3917/StringDB?svg=true)](https://ci.appveyor.com/project/sirjosh3917/stringdb)
StringDB is a fast, powerful, lightweight archival-style DB that uses mainly (you guessed it!) strings.

# Install
Install from [nuget,](https://www.nuget.org/packages/StringDB) or from the [github releases page](https://github.com/SirJosh3917/StringDB/releases/latest)

[```Install-Package StringDB```](https://www.nuget.org/packages/StringDB)

## Why make another DB engine?
I wanted a DB engine that was light on RAM, storage space and relatively fast. I was archiving multiple things, and I needed a way to access those archives really quickly. so I made this.

## How do I use it?
It's very straight forward - and the library is documented so you shouldn't need to refer to anything online, it's hilariously easy to use.

First, make a new database.

```using(var db = Database.FromFile("my.db")) { }```

Next, look at the [available methods!](https://github.com/SirJosh3917/StringDB/blob/master/StringDB/Database.cs)

## What's the byte overhead?
Not much. There's 8 bytes at the beginning of the file representing the index chan, and 9 bytes per index and 2-9 bytes per value ( depending on the length of the value. ). For every index chain, it's just 9 bytes.

## Performance?

```
Final Results
=============

> StringDBTest
--------------
Insert............ 192ms
FetchLast......... 3101ms

DB Size: 5MiB

> MongoDBTest
-------------
Insert............ 335ms
FetchLast......... 539ms

DB Size: 12MiB

> LiteDBNoJournalTest
---------------------
Insert............ 531ms
FetchLast......... 135ms

DB Size: 15MiB

=============
Insert inserts 5000 objects at once
FetchLast gets the very last item in the database 5000 times
```