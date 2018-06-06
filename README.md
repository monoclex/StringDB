# StringDB [![Build status](https://ci.appveyor.com/api/projects/status/github/SirJosh3917/StringDB?svg=true)](https://ci.appveyor.com/project/sirjosh3917/stringdb)
StringDB is a fast, powerful, lightweight archival-style DB that uses mainly (you guessed it!) strings.

# Install
Install from [nuget,](https://www.nuget.org/packages/StringDB) or from the [github releases page](https://github.com/SirJosh3917/StringDB/releases/latest)
[```Install-Package StringDB```](https://www.nuget.org/packages/StringDB)

## Why make another DB engine?
I wanted a DB engine that I could just write to once, and read from later. I was storing a lot of data, and modern day DB programs are very fancy, and add lots of overhead as a result.
I didn't need that overhead. So I made my own.

## How do I use it?
It's very straight forward - and the library is documented so you shouldn't need to refer to anything online, it's hilariously easy to use.

First, make a new database.

```var db = new Database(File.Open("mydb.db"));```

Next, look at the [available methods](https://github.com/SirJosh3917/StringDB/blob/master/StringDB/Database.cs)!

## What's the byte overhead?
There's exactly 9 bytes of overhead for each index, 4 bytes of overhead for each value, and an additional 9 bytes of overhead per each IndexChain.

An IndexChain is a place in the DB where it links the previous IndexChain to the newest IndexChain. IndexChains are created anytime you do a single Insert(), or if you do an InsertRange().
To minimize the size of your DB and speed up read times, it's recommended to use InsertRange as frequently as possible to prevent overhead.

## Can I calculate the byte overhead using math?
Yeah sure! Here's the forumla: `(13 * a) + 9`, with `a` representing how many indexes/values there are in the specified IndexChain. You have to repeat this formula for each and every IndexChain you write to.