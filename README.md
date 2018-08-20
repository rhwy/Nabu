# Nabu

[![MyGet Build](https://www.myget.org/BuildSource/Badge/rhwy?identifier=02dbf8fe-b932-40d5-96e7-7365b0fe19ca)](https://www.myget.org/)

Nabu is a console visualisation library that helps you creating more consistant and beautiful console apps


## Usage

```csharp
// -- Strings --
//there is some extensions useful when you write stuff to the console
Console.WriteLine("=".Replicate(80));
Console.WriteLine("[ Nabu ]".Center(80));
Console.WriteLine("=".Replicate(80));

// -- Styles --
//common usage:
//generate your own functions for common styles you want to use in a simple way:
var error = ConsoleStyles.Build("#red", new ColorPaletteMonokai());
var warning = ConsoleStyles.Build("#yellow#bold", new ColorPaletteMonokai());
var success = ConsoleStyles.Build("#green", new ColorPaletteMonokai());
var info = ConsoleStyles.Build("#blue", new ColorPaletteMonokai());
var underline = ConsoleStyles.Build("#underline");

Console.WriteLine($"{info("[01]")} this is an {error("error")}");
Console.WriteLine($"{info("[02]")} you can also have a {warning("warning")}");
Console.WriteLine($"{info("[03]")} but it's better when all is {success("green")}");
Console.WriteLine($"{info("[04]")} {underline("underline")} when you want to make things important");

Console.WriteLine("-".Replicate(80));
//low level api:
//you have access to a low level api that allows you to a text with
//any style embedded just like you could do in html 
var aBigMessage = @"
#italic`a big message:`
#underline#bold`My Title`
you can write also a big text with #cyan`styles in line`";

Console.WriteLine(TextStyleParser.Print(aBigMessage,new AnsiConsoleAdapter()));

//the advantage is that you can reuse this styled text with any other output
//just like creating directly html content from the exact same message you printed nicely 
//in the console:

var html = TextStyleParser.Print(aBigMessage, new BasicHtmlAdapter());
System.IO.File.WriteAllText("sample.html",html);
System.Diagnostics.Process.Start("open","./sample.html");
Console.Read();
```
