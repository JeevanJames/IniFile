# Developer notes

## To-do items

- [x] Allow defaults for padding to be specified.
  - [ ] Add doc comments for all padding configs
- [ ] Implement a INI specification from https://github.com/SemaiCZE/inicpp/wiki/INI-format-specification
  - [x] Allow for identifier pattern for sections and property names
  - [x] Typed property values for booleans, integral numbers and floating numbers
  - [x] Typed property values for enums
  - [ ] Typed property values for lists
  - [ ] Support space before and after names with `\`
- [ ] Handle cross-platform newline characters when formatting. Enable the `Ensure_format_is_retained` test when done.
- [ ] Document object model with an image in the README.md file.
- [ ] Add more tests.

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
