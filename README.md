# StringDB
<p align="center">
    <img src="https://rawcdn.githack.com/SirJosh3917/StringDB/master/icons/banner_ad.png" alt="StringDB" />
</p>

[![AppVeyor Build Status][badge_appveyor_build_image]][badge_appveyor_build_page]

[Getting Started][wiki_tutorials]

[```Install-Package StringDB```][link_nuget]

StringDB embodies 3 things:

 - [Modularity][section_modular]
 - [Fluency][section_fluent]
 - [Understandability][section_understandable]
 
 In addition to this, the StringDB file format is made to be a very compact yet decently fast way of storing data. [Read up about it on the wiki.][wiki_stringdb_format]

# Modular ![icon_modular]

StringDB was made to be *modular*. Pick up features and use them as you need them, *when* you need them.

By chaining database types to extend functionality, you become freed and can easily add and extend functionality without sacrificing cleanliness or testability.

# Fluent ![icon_fluent]

StringDB allows for a *fluent interface*. Easily create a chain of databases, and fluently build database initialization code that's easy to read and understand.

```cs
var db = new DatabaseBuilder()
    .UseMemoryDatabase<string, int>()
    .WithThreadLock();
```

# Understandable ![icon_understand]

StringDB is easily understandable. Get started in as little as *5 lines*, and apply more knowledge as you go. [See the tutorials.][wiki_tutorials]

```cs
using(var db = StringDatabase.Create())
{
    db.Insert("init", "Hello, World!");
    Console.WriteLine(db.Get("init"));
}
```

[icon_banner_ad]: ./icons/banner_ad.png
[icon_modular]: ./icons/modular.png
[icon_fluent]: ./icons/fluent.png
[icon_understand]: ./icons/understand.png
[icon_simple]: ./icons/simple.png

[badge_appveyor_build_image]: https://ci.appveyor.com/api/projects/status/github/SirJosh3917/StringDB?svg=true
[badge_appveyor_build_page]: https://ci.appveyor.com/project/sirjosh3917/stringdb

[link_nuget]: https://www.nuget.org/packages/StringDB

[section_modular]: #modular-
[section_fluent]: #fluent-
[section_understandable]: #understandable-
[section_simple]: #simple-

[wiki_stringdb_format]: .
[wiki_tutorials]: .