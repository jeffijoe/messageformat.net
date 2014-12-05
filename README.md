# ICU MessageFormatter for .NET

This is an implementation of the ICU Message Format in .NET.

## TL;DR:

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

// You have 4 notifications. Have a nice day, Jeff!
````

## Features

* **It's fast.** Everything is hand-written; no parser-generators, *not even regular expressions*.
* **It's portable.** The library is a PCL, and has no dependencies - the only reference is the standard `.NET` in PCL's.
* **It's compatible with other implementations.** I've been peeking a bit at the [MessageFormat.js][0] library to make sure
  the results would be the same.
* **It's (relatively) small**. For a .NET library, ~25kb is not a lot.
* **It's very white-space tolerant.** You can structure your blocks so they are more readable - look at the example above.
* **Nesting is supported.** You can nest your blocks as you please, there's no special structure required to do this, just ensure your braces match.
* **Exceptions make atleast a little sense.** When exceptions are thrown due to a bad pattern, the exception should include useful information.
* **There's unit tests.** Run them yourself.

## Performance

PerfTests coming soon..

## Supported formats

Basically, it should be able to do anything that [MessageFormat.js][0] can do.

* Select Format: `{gender, select, male{He likes} female{She likes} other{They like}} cheeseburgers`
* Plural Format: `There {msgCount, plural, zero {are no unread messages} one {is 1 unread message} other{are # unread messages}}.`
* Simple variable replacement: `Your name is {name}`
 
## Adding your own pluralizer functions

Same thing as with [NessageFormat.js][0], you can add your own pluralizer function.
The `Pluralizers` property is a `Dictionary<string, object>`, so you can remove the built-in
ones if you want.

````csharp
var mf = new MessageFormat();
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
var mf = new MessageFormat("en");
mf.Pluralizers.Add("en", n => {
  // ´n´ is the number being pluralized.
  if(n == 0)
    return "zero";
  if(n == 1)
    return "one";
  if(n > 1000)
    return "thatsalot";
  return "other";
});

mf.FormatMessage("You have {number, plural, thatsalot {a shitload of notifications} other {# notifications}}", new Dictionary<string, object>{
  {"number", 1001}
});
````

## Escaping literals

Simple - the literals are `{`, `}` and `#` (in a plural block). 
To escape a literal, use a `\` - e.g. `\{`.
  
# Anything else?

There's not a lot - Alex Sexton of [MessageFormat.js][0] did a great job documenting his library, and like I said,
I wrote my implementation so it would be (somewhat) compatible with his.

# Bugs / issues

If you have issues with the library, and the exception is rather cryptic, please open an issue
and include your message, as well as the data you used.

# Todo

* A `FormatMessage` overload that takes an object arg, using reflection to create a dictionary of the values.
* Built-in locales (currently only `en` is added per default).
* Optional caching when formatting the same pattern. This may improve speed (although that shouldn't be necessary), but may take up a bit more memory
  for the `MessageFormatter`'s object lifetime (which is usually very short as well).

# Author

I'm Jeff Hansen, a software developer who likes to fiddle with string parsing when it's not too difficult.
I also do a lot of ASP.NET Web API back-end development, and quite a bit of web front-end stuff.

You can find me on Twitter: [@jeffijoe][1].

  [0]: https://github.com/SlexAxton/messageformat.js
  [1]: https://twitter.com/jeffijoe