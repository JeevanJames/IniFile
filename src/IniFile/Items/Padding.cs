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
    ///     <para>
    ///         Represents details of the padding of an INI item, such as a <see cref="Section"/>,
    ///         <see cref="Property"/>, <see cref="Comment"/> and a <see cref="BlankLine"/>.
    ///     </para>
    ///     <para>
    ///         Padding is used to represent the exact formatting of the INI item in the object model.
    ///     </para>
    /// </summary>
    public class Padding
    {
        /// <summary>
        ///     The amount of space to the left of the INI item.
        /// </summary>
        public PaddingValue Left { get; set; }

        /// <summary>
        ///     Resets the padding values to the defaults.
        /// </summary>
        public virtual void Reset()
        {
            Left = 0;
        }
    }
}