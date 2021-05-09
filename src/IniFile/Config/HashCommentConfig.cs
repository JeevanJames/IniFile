#region --- License & Copyright Notice ---
/*
IniFile Library for .NET
Copyright (c) 2018-2021 Jeevan James
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

namespace IniFile.Config
{
    /// <summary>
    ///     Configuration on whether the hash symbol (#) can be used for comments.
    /// </summary>
    public sealed class HashCommentConfig
    {
        /// <summary>
        ///     Gets or sets whether comments prefixed with a hash (#) symbol are allowed, in addition
        ///     to the standard semi-colon ones.
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        ///     <para>
        ///         Gets or sets whether comments are to be prefixed with a hash (#) symbol by default,
        ///         instead of the standard semi-colon.
        ///     </para>
        ///     <para>Note that semi-colon prefixed comments will still be allowed.</para>
        /// </summary>
        public bool IsDefault { get; set; }
    }
}