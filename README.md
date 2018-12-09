# IniFile.NET [![Build status](https://img.shields.io/appveyor/ci/JeevanJames/inifile.svg)](https://ci.appveyor.com/project/JeevanJames/inifile/branch/master) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/inifile.svg)](https://ci.appveyor.com/project/JeevanJames/inifile/branch/master) [![NuGet Version](http://img.shields.io/nuget/v/IniFile.NET.svg?style=flat)](https://www.nuget.org/packages/IniFile.NET/) [![NuGet Downloads](https://img.shields.io/nuget/dt/IniFile.NET.svg)](https://www.nuget.org/packages/IniFile.NET/)

IniFile.NET is a .NET library to open, modify and save .INI files.

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
|`Encoding`|The character encoding to use when reading or writing data to the INI file.|`UTF-8`|
|`DetectEncoding`|A `bool` indicating whether the character encoding should be automatically detected when the INI file is loaded.|`false`|
|`CaseSensitive`|A `bool` indicating whether the section names and property key names in the INI file are case sensitive.|`false`|

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
An `Ini` object is an `IList<Section>` and each `Section` object in an `Ini` object is an `IList<Property>`, so you can use regular `IList<T>` mechanisms to add, remove and manage sections and properties.

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
TBD

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
TBD

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

## Saving the INI content
The `Ini` class provides several overloads to save the INI content to streams, text writers and files. All these overloads have synchronous and async versions.
```cs
// Synchronous call
ini.SaveTo(@"Setting.ini");

// Asynchronous call
await ini.SaveToAsync(stream);
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
