# ICU MessageFormatter for .NET

This is an implementation of the ICU Message Format in .NET.

## TL;DR:

```c#
var mf = new MessageFormatter();

var str = @"You have {notifications, plural,
              zero {no notifications}
              one {one notification}
              =42 {a universal amount of notifications}
              other {# notifications}
            }. Have a nice day, {name}!";
var formatted = mf.FormatMessage(str, new Dictionary<string, object>{
  {"notifications", 4},
  {"name", "Jeff"}
});

// You have 4 notifications. Have a nice day, Jeff!
```

## Features

* **It's fast.** Everything is hand-written; no parser-generators, *not even regular expressions*. Will post benchmarks later.
* **It's portable.** The library is a PCL, and has no dependencies - the only reference is the standard `.NET` in PCL's.