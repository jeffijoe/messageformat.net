# MessageFormatter for .NET

#### - better UI strings.

 [![Build status](https://ci.appveyor.com/api/projects/status/9g7dplst1vyibc3e?svg=true)](https://ci.appveyor.com/project/jeffijoe/messageformat-net) [![Join the chat at https://gitter.im/jeffijoe/messageformat.net](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/jeffijoe/messageformat.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This is an implementation of the ICU Message Format in .NET. For official information about the format, go to:
http://userguide.icu-project.org/formatparse/messages

## Quickstart

````csharp
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

//Result: You have 4 notifications. Have a nice day, Jeff!

````

Or, if you don't like dictionaries, and don't mind a bit of reflection..

````csharp
var formatted = mf.FormatMessage(str, new {
  notifications = 0,
  name = "Jeff"
});

//Result: You have no notifications. Have a nice day, Jeff!
````

You can use a static method, too:

````csharp
var formatted = MessageFormatter.Format(str, new {
  notifications = 0,
  name = "Jeff"
});

//Result: You have no notifications. Have a nice day, Jeff!
````

## Installation

Either clone this repo and build it, or install it with NuGet:

```
Install-Package MessageFormat
```

## Features

* **It's fast.** Everything is hand-written; no parser-generators, *not even regular expressions*.
* **It's portable.** The library is a PCL, and has no dependencies - the only reference is the standard `.NET` in PCL's.
* **It's compatible with other implementations.** I've been peeking a bit at the [MessageFormat.js][0] library to make sure
  the results would be the same.
* **It's (relatively) small**. For a .NET library, ~25kb is not a lot.
* **It's very white-space tolerant.** You can structure your blocks so they are more readable - look at the example above.
* **Nesting is supported.** You can nest your blocks as you please, there's no special structure required to do this, just ensure your braces match.
* **Adding your own formatters.** I don't know why you would need to, but if you want, you can add your own formatters, and
  take advantage of the code in my base classes to help you parse patterns. Look at the source, this is how I implemented the built-in formatters.
* **Exceptions make atleast a little sense.** When exceptions are thrown due to a bad pattern, the exception should include useful information.
* **There are unit tests.** Run them yourself if you want, they're using XUnit.
* **Built-in cache.** If you are formatting messages in a tight loop, with different data for each iteration, 
  and if you are reusing the same instance of `MessageFormatter`, the formatter will cache the tokens of each pattern (nested, too),
  so it won't have to spend CPU time to parse out literals every time. I benchmarked it, and on my monster machine, 
  it didn't make much of a difference (10000 iterations).

## Performance

If you look at `MessageFormatter_caching_tests`, you will find a "with cache" and "without cache" test.

My machine runs on a Core i7 3960x, and with about **100,000** iterations with random data (generated beforehand), it takes about 2 seconds (1892ms) with the cache,
and about 3 seconds (3236ms) without it. **These results are with a debug build, when it is in release mode the time taken is reduced by about 40%! :)**

## Supported formats

Basically, it should be able to do anything that [MessageFormat.js][0] can do.

* Select Format: `{gender, select, male{He likes} female{She likes} other{They like}} cheeseburgers`
* Plural Format: `There {msgCount, plural, zero {are no unread messages} one {is 1 unread message} other{are # unread messages}}.` (where `#` is the actual number, with the offset (if any) subtracted).
* Simple variable replacement: `Your name is {name}`
 
## Adding your own pluralizer functions

Same thing as with [MessageFormat.js][0], you can add your own pluralizer function.
The `Pluralizers` property is a `Dictionary<string, object>`, so you can remove the built-in
ones if you want.

````csharp
var mf = new MessageFormatter();
mf.Pluralizers.Add("<locale>", n => {
  // ´n´ is the number being pluralized.
  if(n == 0)
    return "zero";
  if(n == 1)
    return "one";
  return "other";
});
````

There's no restrictions on what strings you may return, nor what strings
you may use in your pluralization block.

````csharp
var mf = new MessageFormatter(true, "en"); // true = use cache
mf.Pluralizers["en"] = n =>
{
    // ´n´ is the number being pluralized.
    if (n == 0)
        return "zero";
    if (n == 1)
        return "one";
    if (n > 1000)
        return "thatsalot";
    return "other";
};

mf.FormatMessage("You have {number, plural, thatsalot {a shitload of notifications} other {# notifications}}", new Dictionary<string, object>{
  {"number", 1001}
});
````

## Escaping literals

Simple - the literals are `{`, `}` and `#` (in a plural block). 
To escape a literal, use a `\` - e.g. `\{`.
  
# Anything else?

There's not a lot - Alex Sexton of [MessageFormat.js][0] did a great 
job documenting his library, and like I said,
I wrote my implementation so it would 
be (somewhat) compatible with his.

# Bugs / issues

If you have issues with the library, and the exception makes no sense, please open an issue
and include your message, as well as the data you used.

# Todo

* Built-in locales (currently only `en` is added per default).

Don't expect this in the near future - you're welcome to submit a PR. :)

# Author

I'm Jeff Hansen, a software developer who likes to fiddle with string parsing when it is not too difficult.
I also do a lot of ASP.NET Web API back-end development, and quite a bit of web front-end stuff.

You can find me on Twitter: [@jeffijoe][1].

  [0]: https://github.com/SlexAxton/messageformat.js
  [1]: https://twitter.com/jeffijoe
