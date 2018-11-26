using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace IniFile.Items
{
    public abstract class IniItem
    {
    }

    public interface IPaddedItem<TPadding>
        where TPadding : Padding
    {
        TPadding Padding { get; }
    }

    public class Padding
    {
        public PaddingValue Left { get; set; }

        public PaddingValue Right { get; set; }

        public virtual void Reset()
        {
            Left = 0;
            Right = 0;
        }
    }

    [DebuggerDisplay("{Value}")]
    public readonly struct PaddingValue : IEquatable<PaddingValue>
    {
        public PaddingValue(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public int Value { get; }

        public override bool Equals(object obj)
        {
            return obj is PaddingValue && Equals((PaddingValue)obj);
        }

        public bool Equals(PaddingValue other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }

        public override string ToString() =>
            Value == 0 ? string.Empty : new string(' ', Value);

        public static bool operator ==(PaddingValue value1, PaddingValue value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(PaddingValue value1, PaddingValue value2)
        {
            return !(value1 == value2);
        }

        public static implicit operator PaddingValue(int value) =>
            new PaddingValue(value);

        public static implicit operator int(PaddingValue paddingValue) =>
            paddingValue.Value;
    }

    public abstract class MajorIniItem : IniItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _name;

        protected MajorIniItem(string name)
        {
            Name = name;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name should contain at least one alpha-numeric character.", nameof(value));
                _name = value.Trim();
            }
        }

        public IList<MinorIniItem> MinorItems { get; } = new List<MinorIniItem>();

        public Comment AddComment(string text)
        {
            var comment = new Comment(text);
            MinorItems.Add(comment);
            return comment;
        }

        public BlankLine AddBlankLine()
        {
            var blankLine = new BlankLine();
            MinorItems.Add(blankLine);
            return blankLine;
        }
    }

    public abstract class MajorIniItemCollection<TItem> : KeyedCollection<string, TItem>
        where TItem : MajorIniItem
    {
        protected MajorIniItemCollection() : base(StringComparer.Ordinal)
        {
        }

        protected override string GetKeyForItem(TItem item) =>
            item.Name;
    }

    public sealed class Section : MajorIniItem, IPaddedItem<SectionPadding>
    {
        public Section(string name) : base(name)
        {
        }

        public PropertyCollection Properties { get; } = new PropertyCollection();

        public SectionPadding Padding { get; } = new SectionPadding();

        public string this[string name]
        {
            get => Properties[name].Value;
            set => Properties[name].Value = value;
        }

        public override string ToString() =>
            $"{Padding.Left.ToString()}[{Padding.InsideLeft.ToString()}{Name}{Padding.InsideRight.ToString()}]{Padding.Right.ToString()}";
    }

    public sealed class SectionPadding : Padding
    {
        public PaddingValue InsideLeft { get; set; }

        public PaddingValue InsideRight { get; set; }

        public override void Reset()
        {
            base.Reset();
            InsideLeft = 0;
            InsideRight = 0;
        }
    }

    public sealed class Property : MajorIniItem, IPaddedItem<PropertyPadding>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _value;

        public Property(string name, string value) : base(name)
        {
            Value = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value ?? string.Empty;
        }

        public PropertyPadding Padding { get; } = new PropertyPadding();

        public override string ToString() =>
            $"{Padding.Left.ToString()}{Name}{Padding.InsideLeft.ToString()}={Padding.InsideRight.ToString()}{Value}{Padding.Right.ToString()}";
    }

    public sealed class PropertyPadding : Padding
    {
        public PaddingValue InsideLeft { get; set; } = 1;

        public PaddingValue InsideRight { get; set; } = 1;

        public override void Reset()
        {
            base.Reset();
            InsideLeft = 1;
            InsideRight = 1;
        }
    }

    public sealed class PropertyCollection : MajorIniItemCollection<Property>
    {
        public PropertyCollection()
        {
        }
    }

    public abstract class MinorIniItem : IniItem
    {
        protected MinorIniItem()
        {
        }
    }

    public sealed class Comment : MinorIniItem, IPaddedItem<CommentPadding>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _text;

        public Comment()
        {
            Text = string.Empty;
        }

        public Comment(string text)
        {
            Text = text;
        }

        public string Text
        {
            get => _text;
            set => _text = value ?? string.Empty;
        }

        public CommentPadding Padding { get; } = new CommentPadding();

        public override string ToString() =>
            $"{Padding.Left.ToString()};{Padding.Inside.ToString()}{Text}{Padding.Right.ToString()}";
    }

    public sealed class CommentPadding : Padding
    {
        public PaddingValue Inside { get; set; } = 1;

        public override void Reset()
        {
            base.Reset();
            Inside = 1;
        }
    }

    [DebuggerDisplay("----------")]
    public sealed class BlankLine : MinorIniItem, IPaddedItem<Padding>
    {
        public BlankLine()
        {
        }

        public Padding Padding { get; } = new Padding();

        public override string ToString() =>
            $"{Padding.Left.ToString()}{Padding.Right.ToString()}";
    }

    public static class IniItemFactory
    {
        public static IniItem CreateItem(string line)
        {
            IniItem item = TryCreateProperty(line);
            if (item != null)
                return item;

            item = TryCreateSection(line);
            if (item != null)
                return item;

            item = TryCreateComment(line);
            if (item != null)
                return item;

            item = TryCreateBlankLine(line);
            if (item != null)
                return item;

            throw new FormatException($"Unrecognized line in INI file{Environment.NewLine}{line}");
        }

        private static Section TryCreateSection(string line)
        {
            Match match = SectionPattern.Match(line);
            if (!match.Success)
                return null;
            var section = new Section(match.Groups[3].Value);
            section.Padding.Left = match.Groups[1].Length;
            section.Padding.InsideLeft = match.Groups[2].Length;
            section.Padding.InsideRight = match.Groups[4].Length;
            section.Padding.Right = match.Groups[5].Length;
            return section;
        }

        private static readonly Regex SectionPattern = new Regex(@"^(\s*)\[(\s*)([\w_-][\s\w_-]+)(\s*)\](\s*)$");

        private static Property TryCreateProperty(string line)
        {
            Match match = PropertyPattern.Match(line);
            if (!match.Success)
                return null;
            var property = new Property(match.Groups[2].Value, match.Groups[5].Value);
            property.Padding.Left = match.Groups[1].Length;
            property.Padding.InsideLeft = match.Groups[3].Length;
            property.Padding.InsideRight = match.Groups[4].Length;
            property.Padding.Right = match.Groups[6].Length;
            return property;
        }

        private static readonly Regex PropertyPattern = new Regex(@"^(\s*)([\w_-]+)(\s*)=(\s*)(.*)(\s*)$");

        private static Comment TryCreateComment(string line)
        {
            Match match = CommentPattern.Match(line);
            if (!match.Success)
                return null;
            var comment = new Comment(match.Groups[3].Value);
            comment.Padding.Left = match.Groups[1].Length;
            comment.Padding.Inside = match.Groups[2].Length;
            comment.Padding.Right = match.Groups[4].Length;
            return comment;
        }

        private static readonly Regex CommentPattern = new Regex(@"^(\s*);(\s*)(.+)(\s*)$");

        private static BlankLine TryCreateBlankLine(string line)
        {
            if (line.Trim().Length == 0)
            {
                var blankLine = new BlankLine();
                blankLine.Padding.Left = line.Length;
                return blankLine;
            }
            return null;
        }
    }
}