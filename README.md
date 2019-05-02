# StringDB
<div align="center">
    <img src="https://rawcdn.githack.com/SirJosh3917/StringDB/master/icons/banner_ad.png" alt="StringDB" />

[![Build Status][badge_appveyor_build_image]][badge_appveyor_build_page]
[![Test Status][badge_tests_image]][badge_appveyor_build_page]
[![Nuget Version][badge_nuget_version_image]][link_nuget]
[![Nuget Downloads][badge_nuget_downloads_image]][link_nuget]

</div>

[```Install-Package StringDB```][link_nuget]

## Introduction

StringDB is a key/value pair store with a friendly API to use as little RAM and space as possible.

Verify the claims for yourself:

- [Api][section_api]
- [Tiny][section_tiny]

## Api

Use fluent extensions to create a database:

```cs
using IDatabase<string, string> db = new DatabaseBuilder()
    .UseIODatabase(StringDBVersions.Latest, "database.db")
    .ApplyStringTransformation();

using IDatabase<int, string> memDb = new DatabaseBuilder()
    .UseMemoryDatabase<int, string>();
```

Use the IDatabase interface to interface with databases

```cs
void InsertGreeting(IDatabase<string, string> database, string user)
{
    database.Insert(user, $"Greetings, {user}!");
}
```

And inherit BaseDatabase to create your own IDatabases with minimal effort

```cs
public class TestDatabase : BaseDatabase<int, string>
{
    private class LazyValue : ILazyLoader<string>
    {
        private readonly string _value;
        public LazyValue(int value) => _value = value.ToString();
        public string Load() => _value;
        public void Dispose() {}
    }

    protected override void InsertRange(KeyValuePair<int, string>[] items)
    {
        foreach(var item in items)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }

    protected override IEnumerable<KeyValuePair<int, ILazyLoader<string>>> Evaluate()
    {
        for(var i = 0; i < int.MaxValue)
        {
            yield return KeyValuePair.Create(i, new LazyValue(i));
        }
    }
}
```

## Tiny ![icon_tiny]

StringDB is *tiny*. Use *tiny* amounts of RAM, and *tiny* amounts of space.

### [StringDB 10.0.0 file size: single inserts, 128 byte keys, 1024 byte values][source_insert_test]

| Inserts | Size (in KB, 1000 bytes) | Absolute Minimum Size Possible | StringDB Overhead Percentage |
| --- | --- | --- | --- |
| 1 | 1.172 KB | 1.152 KB | 1.706485% |
| 50 | 58.208 KB | 57.6 KB | 1.04453% |
| 100 | 116.408 KB | 115.2 KB | 1.037729% |

This chart shows the size of a StringDB file after multiple *single inserts*. Every key is 128 bytes long, and every value is 1024 bytes long. By doing single inserts, file size is dramatically affected due to the additional overhead for the index chain.

### [StringDB 10.0.0 file size: insert range, 128 byte keys, 1024 byte values][source_insertrange_test]

| Elements in Insert Range | Size (in KB, 1000 bytes) | Absolute Minimum Size Possible | StringDB Overhead Percentage |
| --- | --- | --- | --- |
| 1 | 1.172 KB | 1.152 KB | 1.706485% |
| 50 | 57.963 KB | 57.6 KB | 0.626262% |
| 100 | 115.913 KB | 115.2 KB | 0.615117% |

This chart shows the size of a StringDB file after a single insert range with the amount of items specified.

## Addons

Official addon support will be maintained for [these libraries.][link_addons]

## We welcome issues!

Don't be afraid to make an issue about anything and everything!

- Is there something weird with how databases are created? Submit an issue!
- Is there something that you wish you could do but can't? Submit an issue!
- Is this library not suitable for your purposes? Submit an isssue!
- Want it to do something? Submit an issue!

It's an honour to have you use this library, and feedback is needed to make this the greatest it can be.

Need immediate assistence? [Join the discord!](discord)

[icon_banner_ad]: ./icons/banner_ad.png
[icon_tiny]: ./icons/tiny.png

[badge_appveyor_build_image]: https://img.shields.io/appveyor/ci/SirJosh3917/StringDB/master.svg?style=flat-square
[badge_tests_image]: https://img.shields.io/codecov/c/github/SirJosh3917/StringDB/master.svg?style=flat-square
[badge_nuget_version_image]: https://img.shields.io/nuget/v/StringDB.svg?style=flat-square
[badge_nuget_downloads_image]: https://img.shields.io/nuget/dt/StringDB.svg?style=flat-square

[badge_appveyor_build_page]: https://ci.appveyor.com/project/sirjosh3917/stringdb

[link_nuget]: https://www.nuget.org/packages/StringDB
[link_addons]: ./addons/addons.md

[section_tiny]: #tiny-
[section_api]: #api-

[source_insert_test]: ./src/StringDB.PerformanceNumbers/SingleInsertFileSize.cs
[source_insertrange_test]: ./src/StringDB.PerformanceNumbers/InsertRangeFileSize.cs

[discord]: https://discord.gg/wVcnkKJ