# Developer notes

## To-do items

- [x] Move exception messages and any other verbatim strings to a resource file.
- [x] Allow comments starting with `#`.
- [x] Reduce netstandard level as much as possible.
- [x] Add support for .NET 3.5, .NET 4.0 and .NET 4.5 targets.
- [x] Allow defaults for padding to be specified.
  - [ ] Add doc comments for all padding configs
- [ ] Implement a INI specification from https://github.com/SemaiCZE/inicpp/wiki/INI-format-specification
  - [x] Allow for identifier pattern for sections and property names
- [ ] Handle cross-platform newline characters when formatting. Enable the `Ensure_format_is_retained` test when done.
- [ ] Document object model with an image in the README.md file.
- [ ] Add more tests.
- [ ] Implement typed values for lists, numbers, booleans, etc.

- [ ] Allow multiline values using the UNIX style:
  - [x] Writing multiline values
  - [ ] Reading multiline values

```ini
Name = <<EOT
This is line 1
This is line 2
EOT
```

- [ ] Add support for strongly-typed INI classes. Perhaps in a different package.

The following code:
```cs
public class PersonalDetails
{
    [Required]
    public string FullName { get; set; }
    public int Age { get; set; }
    [Date("yyyy-MM-dd")]
    public DateTime? Dob { get; set; }
}

public class Employment
{
    [Required]
    public string Company { get; set; }
    public string Position { get; set; }
}

public class EmployeeConfig
{
    [Name("Personal-Details"), Required]
    public PersonalDetails Personal { get; set; }

    public Employment Employment { get; set; }
}
```

maps to the following INI definition:
```ini
[Personal-Details]
FullName = Barry Allen
Age = 32
Dob = 1986-06-14

[Employment]
Company = Justice League
Position = Founding member
```
