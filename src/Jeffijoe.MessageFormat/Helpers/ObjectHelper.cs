// MessageFormat for .NET
// - ObjectHelper.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jeffijoe.MessageFormat.Helpers
{
    /// <summary>
    ///     Object helper
    /// </summary>
    internal static class ObjectHelper
    {
        #region Methods

        /// <summary>
        ///     Gets the properties from the specified object.
        /// </summary>
        /// <param name="obj">
        ///     The object.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        internal static IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            var properties = new List<PropertyInfo>();
            var type = obj.GetType();
            var typeInfo = type.GetTypeInfo();
            while (typeInfo != null)
            {
                properties.AddRange(typeInfo.DeclaredProperties);
                if (typeInfo.BaseType == null)
                {
                    break;
                }

                typeInfo = typeInfo.BaseType.GetTypeInfo();
            }

            return properties;
        }

        /// <summary>
        ///     Creates a dictionary from the specified object's properties. 1 level only.
        /// </summary>
        /// <param name="obj">
        ///     The object.
        /// </param>
        /// <returns>
        ///     The <see cref="Dictionary" />.
        /// </returns>
        internal static Dictionary<string, object> ToDictionary(this object obj)
        {
            // We want to be able to read the property, and it should not be an indexer.
            var properties = GetProperties(obj).Where(x => x.CanRead && x.GetIndexParameters().Any() == false);

            var result = new Dictionary<string, object>();
            foreach (var propertyInfo in properties)
            {
                result[propertyInfo.Name] = propertyInfo.GetValue(obj);
            }

            return result;
        }

        #endregion
    }
}