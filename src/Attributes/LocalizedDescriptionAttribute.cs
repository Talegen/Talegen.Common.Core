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

namespace Talegen.Common.Core.Attributes
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using Properties;

    /// <summary>
    /// This class allows for the specification of a resource string key to use for an enumeration description.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        #region Private Fields

        /// <summary>
        /// Contains the localized description key for the attribute.
        /// </summary>
        private readonly string localizedDescriptionKey;

        /// <summary>
        /// The resource manager.
        /// </summary>
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// The culture.
        /// </summary>
        private readonly CultureInfo culture;

        /// <summary>
        /// Gets a value indicating whether to return the key if not found.
        /// </summary>
        private readonly bool returnKeyIfNotFound = true;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDescriptionAttribute" /> class.
        /// </summary>
        /// <param name="localizedDescriptionKey">Contains the resource key name to load into the attribute description property.</param>
        /// <param name="resourceManagerByType">Contains an optional resource manager by type.</param>
        /// <param name="resourceManagerAssemblyName">Contains an optional resource manager assembly name.</param>
        /// <param name="cultureInfo">Contains an optional culture info object for localization.</param>
        /// <param name="returnKeyIfNotFound">Contains a value indicating whether to return the key if not found.</param>
        public LocalizedDescriptionAttribute(string localizedDescriptionKey, Type resourceManagerByType = null, string resourceManagerAssemblyName = null, string cultureInfo = null, bool returnKeyIfNotFound = true)
        {
            this.returnKeyIfNotFound = returnKeyIfNotFound;
            this.localizedDescriptionKey = localizedDescriptionKey;
            this.resourceManager =
                resourceManagerByType != null ? new ResourceManager(resourceManagerByType) :
                (!string.IsNullOrWhiteSpace(resourceManagerAssemblyName) ? new ResourceManager(resourceManagerAssemblyName, Assembly.GetExecutingAssembly()) : Resources.ResourceManager);
            this.culture = !string.IsNullOrWhiteSpace(cultureInfo) ? CultureInfo.GetCultureInfo(cultureInfo) : CultureInfo.CurrentCulture;
            this.returnKeyIfNotFound = returnKeyIfNotFound;
        }

        /// <summary>
        /// Gets the description of the localized description from resource.
        /// </summary>
        public override string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(this.localizedDescriptionKey))
                {
                    var result = this.resourceManager.GetString(this.localizedDescriptionKey, this.culture);

                    if (returnKeyIfNotFound && result == null)
                    {
                        this.DescriptionValue = this.localizedDescriptionKey;
                    }
                    else
                    {
                        this.DescriptionValue = result;
                    }
                }

                return this.DescriptionValue;
            }
        }
    }
}