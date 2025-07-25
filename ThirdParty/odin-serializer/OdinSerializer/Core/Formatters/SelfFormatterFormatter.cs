﻿//-----------------------------------------------------------------------
// <copyright file="SelfFormatterFormatter.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

namespace EasyToolKit.ThirdParty.OdinSerializer
{
    using System;

    /// <summary>
    /// Formatter for types that implement the <see cref="ISelfFormatter"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="BaseFormatter{T}" />
    public sealed class SelfFormatterFormatter<T> : BaseFormatter<T> where T : ISelfFormatter
    {
        /// <summary>
        /// Calls <see cref="ISelfFormatter.Deserialize" />  on the value to deserialize.
        /// </summary>
        protected override void DeserializeImplementation(ref T value, IDataReader reader)
        {
            value.Deserialize(reader);
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Serialize" />  on the value to deserialize.
        /// </summary>
        protected override void SerializeImplementation(ref T value, IDataWriter writer)
        {
            value.Serialize(writer);
        }
    }

    public sealed class WeakSelfFormatterFormatter : WeakBaseFormatter
    {
        public WeakSelfFormatterFormatter(Type serializedType) : base(serializedType)
        {
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Deserialize" />  on the value to deserialize.
        /// </summary>
        protected override void DeserializeImplementation(ref object value, IDataReader reader)
        {
            ((ISelfFormatter)value).Deserialize(reader);
        }

        /// <summary>
        /// Calls <see cref="ISelfFormatter.Serialize" />  on the value to deserialize.
        /// </summary>
        protected override void SerializeImplementation(ref object value, IDataWriter writer)
        {
            ((ISelfFormatter)value).Serialize(writer);
        }
    }
}