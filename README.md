# IniFile.NET [![Build status](https://img.shields.io/appveyor/ci/JeevanJames/inifile.svg)](https://ci.appveyor.com/project/JeevanJames/inifile/branch/master) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/inifile.svg)](https://ci.appveyor.com/project/JeevanJames/inifile/branch/master) [![NuGet Version](http://img.shields.io/nuget/v/IniFile.NET.svg?style=flat)](https://www.nuget.org/packages/IniFile.NET/) [![NuGet Downloads](https://img.shields.io/nuget/dt/IniFile.NET.svg)](https://www.nuget.org/packages/IniFile.NET/)

IniFile.NET is a .NET library to open, modify and save .INI files.

1. [Installation](#installation)
1. [Loading an existing .INI](#loading-an-existing-ini)
    1. [IniLoadSettings](#iniloadsettings)
1. [Creating an INI file](#creating-a-ini-file)
    1. [Comments and blank lines](#comments-and-blank-lines)
1. [Using properties](#using-properties)
    1. [Gotcha when using implicitly-typed variables to read property values](#gotcha-when-using-implicitly-typed-variables-to-read-property-values)
    1. [Boolean properties](#boolean-properties)
    1. [Date/time properties](#datetime-properties)
    1. [Enum properties](#enum-properties)
1. [Saving the INI content](#saving-the-ini-content)
1. [Global configuration](#global-configuration)
1. [Formatting the INI content](#formatting-the-ini-content)

## Installation
Install using NuGet:
```ps
Install-Package IniFile.NET
```

Install using `dotnet` CLI:
```sh
dotnet add package IniFile.NET
```

## Object model
![Ini object model](/docs/object-model.png)

The `IniFile.Ini` class maintains the structure of an .INI file as an in-memory object model, with objects for sections, properties (key-value pairs), comments and blank lines, allowing it to model the exact structure of a .INI file.

The `IniFile.Ini` class is a collection of `Section` objects (`IList<Section>`). Each `Section` is additionally a collection of `Property` objects (`IList<Property>`).

Both `Section` and `Property` objects contain a collection of minor objects, namely `Comment` and `BlankLine` objects, which are the comments and blank lines that appear before the respective sections and properties.

## Loading an existing .INI
The `Ini` class provides several constructor overloads to load .INI data from streams, text readers and files.
```cs
// Load INI data from a file
var ini = new Ini(@"Settings.ini");
```

The class also provides a static factory method `Load` to create an `Ini` object from a string.
```cs
const iniStr = File.ReadAllText(@"Settings.ini");
Ini ini = Ini.Load(iniStr);
```

### `IniLoadSettings`
All `Ini` constructors and the `Load` static factory method accept an optional `IniLoadSettings` object to control how the .INI data is loaded and handled.

|Property|Description|Default|
|--------|-----------|-------|
|`CaseSensitive`|A `bool` indicating whether the section names and property key names in the INI file are case sensitive.|`false`|
|`DetectEncoding`|A `bool` indicating whether the character encoding should be automatically detected when the INI file is loaded.|`false`|
|`Encoding`|The character encoding to use when reading or writing data to the INI file.|`UTF-8`|
|`IgnoreBlankLines`|A `bool` indicating whether to ignore blank lines when loading the INI file content.|`false`|
|`IgnoreComments`|A `bool` indicating whether to ignore comments when loading the INI file content.|`false`|

```cs
var loadSettings = new IniLoadSettings
{
    Encoding = Encoding.Unicode,
    DetectEncoding = true,
    CaseSensitive = true
};
var ini = new Ini(stream, loadSettings);
```

## Creating a INI file
An `Ini` object is a collection of `Section` objects (`IList<Section>`) and each `Section` object is a collection of `Property` objects (`IList<Property>`), so you can use regular `IList<T>` mechanisms to add, remove and manage sections and properties.

So, for example, you can create an INI from scratch, using collection initializers, as shown here:
```cs
var ini = new Ini
{
    new Section("Section Name")
    {
        new Property("Property1 Name", "A string value"),
        new Property("Property2 Name", 10)
    }
};
```

Properties are also name-value pairs, so you can also use the dictionary initializer syntax to create them. So the code above can also look like this:
```cs
var ini = new Ini
{
    new Section("Section Name")
    {
        ["Property1 Name"] = "A string value",
        ["Property2 Name"] = 10
    }
};
```

Since they are just regular lists, you can use the regular methods and properties on `IList<T>` to manage sections and properties:
```cs
// Get number of sections
int sectionCount = ini.Count;

// Add a new section
var section = new Section("New section");
ini.Add(section);

// foreach over the properties of a section
foreach (Property property in section)
{
    // Your code goes here
}

// You can also use regular LINQ operations

// Check if there are any properties in a section
if (section.Any())
{
    // Your code goes here
}
```

### Comments and blank lines
Any comments and blank lines appearing before a section or property belong to the respective `Section` or `Property` instance, and are stored in an `Items` property.
```ini
; This comment and the following blank line
; belong to the Connections section.

[Connections]

; The blank line directly above, this comment
; and this comment belong to the SqlDb property
SqlDb = Db=Server;User=admin;Pwd=passw0rd1
```

Comments and blank lines are represented by the `Comment` and `BlankLine` classes, respectively.

The `Items` property is a regular `IList<>` collection, and can contain a mix of `Comment` and `BlankLine` instances.
```cs
// Two ways to add a comment to a section.
section.AddComment("This is a comment");
section.Items.Add(new Comment("This is a comment."));

// Adding a comment to a property.
property.Items.Add(new Comment("No need to specify the ; prefix. It is added automatically"));

// Two ways to add a blank line to a section
section.AddBlankLine();
section.Items.Add(new BlankLine());

// Find all comments for a property
IEnumerable<Comment> comments = property.Comments;
IEnumerable<Comment> comments = property.Items.OfType<Comment>();
```

`Section` and `Property` constructors also accept a range of strings to denote comments or blank lines. If the string is `null`, empty or just whitespace, then it is considered a blank line, otherwise it is considered a comment. This code:
```cs
var section = new Section("SectionName", null, "This is a comment surrounded by blank lines", null);
```
will generate:
```ini

; This is a comment surrounded by blank lines

[SectionName]
```

## Using properties
INI properties are represented by the `Property` class and are name-value pairs, where the name is a `string` and the value can be a `string`, `bool`, any integral number type (`int`, `byte`, `long`, `ushort`, etc.), any floating-point number type (`float`, `double` and `decimal`) and `DateTime`.

```cs
// Write a double value to a property
section["Pi"] = 3.14d;

// Write a boolean value to a property
var property = new Property("SendMail", true);
section.Add(property);

// Read a string value from a property
string name = section["Name"];

// Read a decimal value from a property
decimal price = section["Price"];
```

### Gotcha when using implicitly-typed variables to read property values
When reading property values, if you use an implicitly-typed variable (using `var`), then you will notice that the variable is of type `PropertyValue`. This is the underlying type used by the framework to allow property values to support multiple types like `string`, `int`, `bool`, etc. It does this by allowing implicit conversions between the `PropertyValue` value and all the allowed types.

However, when using `var` to declare the variable, the C# compiler will not know the actual type you intend the property value to be.
```cs
var value = section["property-name"]; // value will be of type PropertyValue
```

To get the value as the actual type, explicitly specify the variable type:
```cs
int value = section["property-name"]; // value will be of type int
```

Alternatively, you can explicitly cast the `PropertyValue` value to the expected type:
```cs
var value = (int)section["property-value"]; // value will be of type int
```

### Boolean properties
The IniFile framework can recognize the following string values when reading boolean properties:

|Boolean value|Allowed string values|
|-------------|---------------------|
|`true`|`1`, `t`, `y`, `on`, `yes`, `enabled`, `true`|
|`false`|`0`, `f`, `n`, `off`, `no`, `disabled`, `false`|

When writing boolean values, the IniFile framework will use the string values configured in the `Ini.Config.Types.TrueString` and `Ini.Config.Types.FalseString` to write the `true` and `false` values respectively to the output INI file.

By default, `Ini.Config.Types.TrueString` is configured to `1` and `Ini.Config.Types.FalseString` is configured to `0`. So, the following code:
```cs
section["HasDiscount"] = true;
section["ValidateParking"] = false;
```
will generate the following properties in the INI file:
```ini
HasDiscount = 1
ValidateParking = 0
```

You can assign custom strings to the `Ini.Config.Types.TrueString` and `Ini.Config.Types.FalseString` config properties.
```cs
Ini.Config.SetBooleanStrings("Oui", "Non");

// Or the long way
Ini.Config.Types.TrueString = "Oui";
Ini.Config.Types.FalseString = "Non";
```

### Date/time properties
Property values can be written as `DateTime` values:
```cs
section["Today"] = DateTime.Now;
```

The IniFile framework uses the `Ini.Config.Types.DateFormat` property to control how date values are represented as strings in the INI file. By default, this is defaulted to the system's short date format (`CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern`).
```cs
// Set date format to US style
Ini.Config.SetDateFormats(dateFormat: "M/dd/yyyy");

// Or the long way
Ini.Config.Types.DateFormat = "M/dd/yyyy";

// Reset the date format to system default
Ini.Config.SetDateFormats();

// Or the long way
Ini.Config.Types.DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
```

When reading date values from a property, the framework will try to parse the property string value according to the format specified by the `Ini.Config.Types.DateFormat` config value.
```cs
DateTime createdDate = section["CreatedDate"];
```

If the date string value in the INI file is a different format from the config, you can use the property's `AsDateTime` method to explicitly specify the format:
```cs
DateTime createdDate = section["CreatedDate"].AsDateTime("yyyy/MM/dd");
```

### Enum properties
Property values cannot be directly read or written as enum values.

To write an enum value to a property, use the enum's `ToString` method to write a string representation of the value.
```cs
section["StartDay"] = DayOfWeek.Monday.ToString();
```

To read an enum value, the property provides an `AsEnum<T>` method:
```cs
DayOfWeek startDay = section["StartDay"].AsEnum<DayOfWeek>();
```

Enum values are not case-sensitive.

## Saving the INI content
The `Ini` class provides several overloads to save the INI content to streams, text writers and files. All these overloads have synchronous and async versions.
```cs
// Synchronous call
ini.SaveTo(@"Setting.ini");

// Asynchronous call
await ini.SaveToAsync(stream);
```

## Global configuration
Certain aspects of the IniFile framework can be configured using the static `Ini.Config` property. This has properties to configure behaviors such as:
* Whether to allow hash symbols (`#`) to represent comments in addition to the default semi-colon (`;`).
* The default spacings for various types of INI objects such as sections, properties and comments.
* How to handle reading and writing of certain types of property values, such as booleans and date/times.

While, you can set the configuration properties manually, the `Ini.Config` property also provides a fluent API to configure related sets of configurations:
```cs
Ini.Config
    .AllowHashForComments(setAsDefault: true)
    .SetSectionPaddingDefaults(insideLeft: 1, insideRight: 1)
    .SetPropertyPaddingDefaults(left: 2)
    .SetBooleanStrings(trueString: "YES", falseString: "NO")
    .SetDateFormats(dateFormat: "M/dd/yy");
```

## Formatting the INI content
The `Ini` class retains the exact formatting from the source .INI file content. It provides a `Format` method to correctly format the contents.

By default, the `Format` method resets the padding for all lines in the INI file.
```cs
var ini = new Ini(iniFilePath);
ini.Format();
ini.SaveTo(iniFilePath);
```

In addition, the `Format` method takes an optional `IniFormatOptions` parameter that can specify additional formatting options:

|Option|Description|Default|
|------|-----------|-------|
|`EnsureBlankLinesBetweenSections`|If true, a blank line is inserted between each section.|`false`|
|`EnsureBlankLinesBetweenProperties`|If true, a blank line is inserted between each property.|`false`|
|`RemoveSuccessiveBlankLines`|If true, any successive blank lines are removed.|`false`|

```cs
var ini = new Ini(iniFilePath);
ini.Format(new IniFormatOptions
{
    EnsureBlankLinesBetweenSections = true,
    RemoveSuccessiveBlankLines = true
});
ini.SaveTo(iniFilePath);
```
