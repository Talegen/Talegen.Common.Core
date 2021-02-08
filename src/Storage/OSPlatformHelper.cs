namespace Bastille.Id.Server.Core.IO
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class contains extension methods for working with operating systems.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class OSPlatformHelper
    {
        /// <summary>
        /// Gets a value indicating whether this instance is windows.
        /// </summary>
        /// <value><c>true</c> if this instance is windows; otherwise, <c>false</c>.</value>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Gets the current operating system version.
        /// </summary>
        /// <returns>Returns a Version object if found. Otherwise returns an empty version.</returns>
        public static Version GetVersion()
        {
            Version result = new Version();
            string[] descriptionStringParts = RuntimeInformation.OSDescription.Split(' ');

            // find the numeric string
            if (descriptionStringParts != null && descriptionStringParts.Length > 0)
            {
                int i = 0;
                while (i < descriptionStringParts.Length)
                {
                    if (descriptionStringParts[i].Contains("."))
                    {
                        result = new Version(descriptionStringParts[i]);
                        break;
                    }
                    ++i;
                }
            }

            return result;
        }
    }
}