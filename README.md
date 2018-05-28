# StringDB
StringDB is a fast, powerful, resource light DB for strings only.

# Why?
I was unsatisfied. LiteDB was too resouce hungry, and took up too much space for my liking.
Most other DB solutions required some form of "server exe" to be running.
Storing a bunch of tiny files wouldn't work for me.
It took forever to find something by an index.
All I needed was to store a string attatched to a string, I can serialize the class myself using Newtonsoft.JSON or something of the sort.

Nothing seemed to work quite like how I wanted it to.
So I set out to *change that*. I made my own.

# How well is this supported?
Not at all. Only use this if you know what you're doing and it's some simple home project.

# How do I use this?
See StringDB.Tester

# Unit tests?
Run an issue if something breaks. Don't be retarded while using it either.

# Performance tests?
Soon.

#Overhead?
Approximately 17 bytes per string index and it's correlating data, if I remember correctly.