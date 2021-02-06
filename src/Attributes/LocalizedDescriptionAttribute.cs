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
        public LocalizedDescriptionAttribute(string localizedDescriptionKey, ResourceManager resourceManager = null, CultureInfo cultureInfo = null)
        {
            this.localizedDescriptionKey = localizedDescriptionKey;
            this.resourceManager = resourceManager ?? Resources.ResourceManager;
            this.culture = cultureInfo ?? CultureInfo.CurrentCulture;
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