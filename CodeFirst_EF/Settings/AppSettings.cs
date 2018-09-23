using System.ComponentModel;
using System.Configuration;

namespace CodeFirst_EF.Settings
{
    public static class AppSettings
    {
        public static T Get<T>(string key)
        {
            var appSetting = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting)) throw new AppSettingNotFoundException(key);

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T) (converter.ConvertFromInvariantString(appSetting));
        }
    }

    /// <summary>
    /// .NET doesn't provide a concise exception for invalid App settings, normally would avoid
    /// defining user exception types.
    /// </summary>
    public class AppSettingNotFoundException : ConfigurationErrorsException
    {
        public AppSettingNotFoundException(string message) : base(message)
        {
        }
    }
}
