# Developer notes

## To-do items

- [x] Move exception messages and any other verbatim strings to a resource file.
- [x] Allow comments starting with `#`.
- [x] Reduce netstandard level as much as possible.
- [x] Add support for .NET 3.5, .NET 4.0 and .NET 4.5 targets.
- [x] Allow defaults for padding to be specified.
  - [ ] Add doc comments for all padding configs
- [ ] Handle cross-platform newline characters when formatting. Enable the `Ensure_format_is_retained` test when done.
- [ ] Document object model with an image in the README.md file.
- [ ] Add more tests.
- [ ] Implement a INI specification from https://github.com/SemaiCZE/inicpp/wiki/INI-format-specification
  - [ ] Allow for identifier pattern for sections and property names
- [ ] Implement typed values for lists, numbers, booleans, etc.

- [ ] Allow multiline values using the UNIX style:

```ini
Name = <<EOT
This is line 1
This is line 2
EOT
```
