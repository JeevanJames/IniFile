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

namespace IniFile.Items
{
    /// <summary>
    ///     Base class for the minor INI items, namely the <see cref="Comment"/> and
    ///     <see cref="BlankLine"/>.
    /// </summary>
    public abstract class MinorIniItem : IniItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MinorIniItem"/> class.
        /// </summary>
        protected MinorIniItem()
        {
        }

        public static implicit operator MinorIniItem(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length == 0)
                return new BlankLine();
            return new Comment(str);
        }
    }
}