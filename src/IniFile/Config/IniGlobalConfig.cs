#region --- License & Copyright Notice ---
/*
IniFile Library for .NET
Copyright (c) 2018 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using IniFile.Items;

namespace IniFile.Config
{
    public sealed class IniGlobalConfig
    {
        public HashCommentConfig HashForComments { get; } = new HashCommentConfig();

        public PaddingConfig Padding { get; } = new PaddingConfig();

        public IniGlobalConfig AllowHashForComments(bool setAsDefault = false)
        {
            HashForComments.Allow = true;
            HashForComments.IsDefault = setAsDefault;
            return this;
        }

        public IniGlobalConfig SetSectionPaddingDefaults(PaddingValue? left = null, PaddingValue? insideLeft = null,
            PaddingValue? insideRight = null, PaddingValue? right = null)
        {
            Padding.Section.Left = left ?? 0;
            Padding.Section.InsideLeft = insideLeft ?? 0;
            Padding.Section.InsideRight = insideRight ?? 0;
            Padding.Section.Right = right ?? 0;
            return this;
        }

        public IniGlobalConfig SetPropertyPaddingDefaults(PaddingValue? left = null, PaddingValue? insideLeft = null,
            PaddingValue? insideRight = null, PaddingValue? right = null)
        {
            Padding.Property.Left = left ?? 0;
            Padding.Property.InsideLeft = insideLeft ?? 1;
            Padding.Property.InsideRight = insideRight ?? 1;
            Padding.Property.Right = right ?? 1;
            return this;
        }

        public IniGlobalConfig SetCommentPaddingDefaults(PaddingValue? left = null, PaddingValue? inside = null,
            PaddingValue? right = null)
        {
            Padding.Comment.Left = left ?? 0;
            Padding.Comment.Inside = inside ?? 1;
            Padding.Comment.Right = right ?? 1;
            return this;
        }
    }
}