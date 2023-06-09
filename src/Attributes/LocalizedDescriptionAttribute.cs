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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDescriptionAttribute" /> class.
        /// </summary>
        /// <param name="localizedDescriptionKey">Contains the resource key name to load into the attribute description property.</param>
        /// <param name="resourceManager">Contains an optional resource manager</param>
        /// <param name="cultureInfo">Contains an optional culture info object for localization.</param>
        public LocalizedDescriptionAttribute(string localizedDescriptionKey, object resourceManager = null, object cultureInfo = null)
        {
            this.localizedDescriptionKey = localizedDescriptionKey;
            this.resourceManager = resourceManager as ResourceManager ?? Resources.ResourceManager;
            this.culture = cultureInfo as CultureInfo ?? CultureInfo.CurrentCulture;
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
                    this.DescriptionValue = this.resourceManager.GetString(this.localizedDescriptionKey, this.culture) ?? this.localizedDescriptionKey;
                }

                return this.DescriptionValue;
            }
        }
    }
}