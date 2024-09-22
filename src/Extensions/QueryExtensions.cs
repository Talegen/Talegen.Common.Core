/*
 *
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Talegen.Common.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    
    /// <summary>
    /// Contains an enumerated list of sort directions for a query.
    /// </summary>
    
    public enum SortDirection
    {
        /// <summary>
        /// Ascending
        /// </summary>
        Ascending,

        /// <summary>
        /// Descending
        /// </summary>
        Descending
    }

    /// <summary>
    /// This class contains query extensions used for enhancing LINQ queries.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// This method converts a string value into a sort direction.
        /// </summary>
        /// <param name="value">Contains the string value to convert.</param>
        /// <returns>Returns the converted <see cref="SortDirection" /> value.</returns>
        public static SortDirection ToSortDirection(this string value)
        {
            Enum.TryParse(value, true, out SortDirection sortDirection);
            return sortDirection;
        }

        /// <summary>
        /// This method is used to order an enumerable result by a property name stored in a string.
        /// </summary>
        /// <typeparam name="T">Contains the enumerable type to order.</typeparam>
        /// <param name="query">Contains the query for which an order sort will be executed.</param>
        /// <param name="propertyName">Contains the property name to order the entities by.</param>
        /// <param name="direction">Contains the direction of the order.</param>
        /// <returns>Returns an ordered query for the specified entities by property name.</returns>
        public static IQueryable<T> OrderByName<T>(this IQueryable<T> query, string propertyName, SortDirection direction = SortDirection.Ascending)
        {
            IQueryable<T> result = query;

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                string methodName = direction == SortDirection.Ascending ? "OrderBy" : "OrderByDescending";
                string[] props = propertyName.Split('.');
                Type type = typeof(T);
                ParameterExpression arg = Expression.Parameter(type, "x");
                Expression expr = arg;

                foreach (string prop in props)
                {
                    // use reflection (not ComponentModel) to mirror LINQ
                    PropertyInfo pi = type.GetProperty(prop);

                    if (pi != null)
                    {
                        expr = Expression.Property(expr, pi);
                        type = pi.PropertyType;
                    }
                }

                Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
                LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

                result = (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                                method => method.Name == methodName
                                && method.IsGenericMethodDefinition
                                && method.GetGenericArguments().Length == 2
                                && method.GetParameters().Length == 2)
                                .MakeGenericMethod(typeof(T), type)
                                .Invoke(null, new object[] { query, lambda });
            }

            return result;
        }
    }
}