namespace Talegen.Common.Core.Extensions
{
    using System.Globalization;

    /// <summary>
    /// This class contains extensions related to localization code and strings.
    /// </summary>
    public static class LocaleExtensions
    {
        /// <summary>
        /// The default language code
        /// </summary>
        public const string DefaultLanguageCode = "en-US";

        /// <summary>
        /// This method is used to convert a specified character code and return the two-letter language name in return.
        /// </summary>
        /// <param name="characterCode">Contains the character code to find a two-letter language code for.</param>
        /// <returns>Returns the two-letter ISO language code for the specified character code.</returns>
        public static string ToNeutralCultureCode(this string characterCode)
        {
            CultureInfo culture;

            try
            {
                culture = CultureInfo.CreateSpecificCulture(characterCode);

                if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    culture = CultureInfo.CurrentCulture;
                }
            }
            catch
            {
                culture = CultureInfo.CurrentCulture;
            }

            return culture.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// This method is used to convert a specified character code and return the four-letter language name in return.
        /// </summary>
        /// <param name="characterCode">Contains the character code to find a four-letter language code for.</param>
        /// <returns>Returns the four-letter ISO language code for the specified character code.</returns>
        public static string ToLocalCultureCode(this string characterCode)
        {
            CultureInfo culture;
            try
            {
                culture = CultureInfo.CreateSpecificCulture(characterCode);

                if (culture.Equals(CultureInfo.InvariantCulture))
                {
                    culture = CultureInfo.CurrentCulture;
                }
            }
            catch
            {
                culture = CultureInfo.CurrentCulture;
            }

            return culture.Name;
        }

        /// <summary>
        /// This method is used to return the culture info object for the specified locale code.
        /// </summary>
        /// <param name="localeCode">Contains the locale code to convert.</param>
        /// <returns>Returns a Culture Info object.</returns>
        public static CultureInfo ToCultureInfo(this string localeCode)
        {
            CultureInfo result;

            if (!string.IsNullOrWhiteSpace(localeCode))
            {
                // attempt to set the culture info
                try
                {
                    result = CultureInfo.CreateSpecificCulture(localeCode);
                }
                catch (CultureNotFoundException)
                {
                    result = CultureInfo.CreateSpecificCulture(DefaultLanguageCode);
                }
            }
            else
            {
                result = CultureInfo.CreateSpecificCulture(DefaultLanguageCode);
            }

            return result;
        }
    }
}