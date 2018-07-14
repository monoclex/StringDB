# StringDB Beginner Tutorial

*Lead by example*

- [Beforehand](#beforehand)
- [Hello, World!](#hello-world)
- [Insert & Get's Failures](#insert-amp-gets-failures)
- [Multiple Indexes!](#multiple-indexes)
- [GetValueAs](#getvalueas)
- [Thread Safeness?](#thread-safeness)
- [Overwriting Values](#overwriting-values)

## Beforehand

This tutorial should tell you anything you need to know for using StringDB.

I'll assume that you've already gotten the nuget package of it and have a `using` statement at the top of your code already.

It's important that you keep this philosophy in mind:

```
I will use the functions that meet my needs exactly.
```

For example, if you're going to insert a single item, you shouldn't `InsertRange` an `IEnumerable` of `KeyValuePairs` instead of using the built in `Insert` method.
What if later I optimize `Insert` so it doesn't internally use `InsertRange` anymore? This is why you should use the methods that meet your needs exactly.

## Hello, World!

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.Insert<string, string>("my-first-db", "Hello, World!");
	
	Console.WriteLine(db.Get<string>("my-first-db").GetValue<string>());
}
```

```
Hello, World!
```

Congrats, after compiling this code you've stored "Hello, World!" into a StringDB Database! Bravo! But what did you even do besides copy and pasting some code?

```cs
using (var db = Database.FromFile("stringdb.db")) {
```

StringDB has something called a `Database`, and you called upon the static method `Database.FromFile()`.
This automagically creates a FileStream, and by putting the database in a using statement, when you dispose of the database, you'll also dispose of the stream.

Nice nice!

```cs
db.Insert<string, string>("my-first-db", "Hello, World!");
```

Here you took the value `Hello, World!` and associated it with the key `my-first-db`.
We'll get to why you specified them as strings [later](#typemanager)

```cs
Console.WriteLine(db.Get<string>("my-first-db").GetValue<string>());
```

First, we tell the database to find the index called `my-first-db`, and again, specify that as a string. We'll get to that [later](#typemanager)
After that, we get an `IReaderPair`. Read up about IReaderPairs [here](reference/IReaderPair.html)

We call `GetValue<string>()` to read out the string value, since we know we inserted it as a string.

That's it!

## Insert & Get's Failures

If you were to only use Insert and Get, you're probably doing it wrong.

Insert internally uses `InsertRange`, so if you can insert multiple items by calling InsertRange at all, you should.
`InsertRange` is a much better alternative, if you can use it.

Get throws exceptions if it can't find the object you're looking for. `TryGet<T>` doesn't. It returns a boolean if it found it.

## Multiple Indexes!

Most databases, an index has to be *unique* and *special*. Not here. Checking if an index already exists takes up your precious time, which StringDB will not waste.

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.InsertRange<string, string>(new KeyValuePair<string, string>[] {
		new KeyValuePair<string, string>("multidex", "Hello,"),
		new KeyValuePair<string, string>("multidex", " "),
		new KeyValuePair<string, string>("multidex", "World!"),
	});
	
	Console.WriteLine(db.Get<string>("multidex").GetValue<string>());
}
```

This code will compile *and* run *without* errors. But what will it output?

The answer is `Hello,`. This is because it's first.

```
Hello,
```

Now how in the name of heck do you go about getting the `World!` from this?

The answer is using `GetAll<T>`

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.InsertRange<string, string>(new KeyValuePair<string, string>[] {
		new KeyValuePair<string, string>("multidex", "Hello,"),
		new KeyValuePair<string, string>("multidex", " "),
		new KeyValuePair<string, string>("multidex", "World!"),
	});
	
	foreach(var i in db.GetAll<string>("multidex")) {
		Console.Write(i.GetValue<string>());
	}
}
```

```
Hello, World!
```

`Get<T>` internally uses `GetAll<T>` and returns the first occurence of whatever it finds anyways.

By using `GetAll<T>`, you can get the values of multiple indexes.

In this scenario, we will now see `Hello, World!` appear on the screen.

Lovely!

## GetValueAs<T>

We've been using `GetValue<string>` all day. But what if it's not a <string>?

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.Insert<string, byte[]>("test", new byte[]{ 0x48, 0x69, 0x21 });
	
	Console.WriteLine(db.Get<string>("test").GetValue<string>());
}
```

This code has an obvious fault - we're inserting a `byte[]` and trying to read it as a `string`.

```
System.Exception: 'The data you are trying to read is not of type System.String, it is of type System.Byte[]'
```

`GetValueAs<T>` comes to the rescue. `GetValueAs<T>` will ignore whatever type the value is *supposed* to be, and forces it to try be of the type you *want* it to be.

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.Insert<string, byte[]>("test", new byte[]{ 0x48, 0x69, 0x21 });
	
	Console.WriteLine(db.Get<string>("test").GetValueAs<string>());
}
```

This code will work fine! Because `string`, `byte[]`, and `Stream` are very similar in the manner needed to store them, you can use the three of them interchangeably. ( Just beware of really long streams )

```
Hi!
```

Try use `GetValueType();` if you want to see the value of the type.

## Thread Safeness?

No, Database and everything else is *not* thread safe. If you're using it in an async context, or in some kind of Thread, I recommend using `ThreadSafeDatabase`.

```cs
using (var db = Database.FromFile("stringdb.db").MakeThreadSafe()) {
	Parallel.For(0, 1_000_000, (i) => {
		db.Insert("test", "value");
	});
}
```

This code would raise hell if the `.MakeThreadSafe();` function wasn't there.

ThreadSafeness is achieved by wrapping a `lock` around any and all methods, as well as during reading any `IReaderPair`.

I'd try to stay away from a ThreadSafeDatabase whenever possible for speed purposes, but you do you!

## Overwriting Values

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.Insert<string, string>("misclick", "This is a tyop!");
	
	Console.WriteLine(db.Get<string>("misclick").GetValue<string>());
}
```

```
This is a tyop!
```

Uh-oh! You made a boo boo. It says `tyop` instead of `typo`!
Lucky for you, I've made the `OverwriteValue` function just for you!

```cs
using (var db = Database.FromFile("stringdb.db")) {
	db.Insert<string, string>("misclick", "This is a tyop!");
	
	var misclick = db.Get<string>("misclick");
	Console.WriteLine(misclick.GetValue<string>());
	
	db.OverwriteValue<string>(misclick, "This is not a typo!");
	
	// now that we modified it, we have to re-get it
	
	misclick = db.Get<string>("misclick");
	
	Console.WriteLine(misclick.GetValueAs<string>());
}
```

```
This is a tyop!
This is not a typo!
```

Beware of calling `OverwriteValue` too frequently though! Especially if the data you're overwriting is larger then the older data, for example in this case.
It will leave traces within the file itself of a previous value that nothing leads to.

Also beware of calling `OverwriteValue` and then **not** updating your reference value.
If you call `OverwriteValue` without re-getting the item, you've just entered "unknown territory".

If the new value is smaller then the old value, `GetValue<string>()` will work as expected - getting the new value. If the new value is larger then the old value, this will just return the old value. This is why you need to re-get the value.