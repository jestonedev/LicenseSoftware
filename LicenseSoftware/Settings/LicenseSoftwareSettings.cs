using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;

namespace LicenseSoftware
{
    public static class LicenseSoftwareSettings
    {
        private static string key = "Z33wYQs+rZOGMUH0RGj8GIKggOB82xwhACg9JCA6/kw=";
        private static string IV = "E0Ci3P9JPj43jUdKNSvtmQ==";
        private static Aes aes = Aes.Create();

        public static string ConnectionString {
            get {
                try
                {
                    byte[] inputBuffer = Convert.FromBase64String(Settings.Properties.Settings.Default.ConnectionString);
                    var decryptor = aes.CreateDecryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
                    byte[] outputBuffer = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                    return Encoding.UTF8.GetString(outputBuffer);
                }
                catch (CryptographicException)
                {
                    return null;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            set
            {
                byte[] inputBuffer = Encoding.UTF8.GetBytes(value);
                var encryptor = aes.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
                byte[] outputBuffer = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                Settings.Properties.Settings.Default.ConnectionString = Convert.ToBase64String(outputBuffer);
            }
        }

        public static string LDAPUserName
        {
            get
            {
                return Settings.Properties.Settings.Default.LDAPUserName;
            }
            set
            {
                Settings.Properties.Settings.Default.LDAPUserName = value;
            }
        }

        public static string LDAPPassword
        {
            get
            {
                try
                {
                    byte[] inputBuffer = Convert.FromBase64String(Settings.Properties.Settings.Default.LDAPEncryptedPassword);
                    var decryptor = aes.CreateDecryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
                    byte[] outputBuffer = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                    return Encoding.UTF8.GetString(outputBuffer);
                }
                catch (CryptographicException)
                {
                    return null;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            set
            {
                byte[] inputBuffer = Encoding.UTF8.GetBytes(value);
                var encryptor = aes.CreateEncryptor(Convert.FromBase64String(key), Convert.FromBase64String(IV));
                byte[] outputBuffer = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                Settings.Properties.Settings.Default.LDAPEncryptedPassword = Convert.ToBase64String(outputBuffer);
            }
        }

        public static bool UseLDAP
        {
            get
            {
                return Settings.Properties.Settings.Default.UseLDAP;
            }
            set
            {
                Settings.Properties.Settings.Default.UseLDAP = value;
            }
        }

        public static string ActivityManagerPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerPath;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerPath = value;
            }
        }

        public static string ActivityManagerOutputCodePage
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerOutputCodePage;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerOutputCodePage = value;
            }
        }

        public static string ActivityManagerConfigsPath
        {
            get
            {
                return Settings.Properties.Settings.Default.ActivityManagerConfigsPath;
            }
            set
            {
                Settings.Properties.Settings.Default.ActivityManagerConfigsPath = value;
            }
        }

        public static int MaxDBConnectionCount
        {
            get
            {
                return Settings.Properties.Settings.Default.MaxDBConnectionCount;
            }
            set
            {
                Settings.Properties.Settings.Default.MaxDBConnectionCount = value;
            }
        }

        public static int DataModelsCallbackUpdateTimeout
        {
            get
            {
                return Settings.Properties.Settings.Default.DataModelsCallbackUpdateTimeout;
            }
            set
            {
                Settings.Properties.Settings.Default.DataModelsCallbackUpdateTimeout = value;
            }
        }

        public static int CalcDataModelsUpdateTimeout
        {
            get
            {
                return Settings.Properties.Settings.Default.CalcDataModelsUpdateTimeout;
            }
            set
            {
                Settings.Properties.Settings.Default.CalcDataModelsUpdateTimeout = value;
            }
        }

        public static void Save()
        {
            Settings.Properties.Settings.Default.Save();
        }
    }
}
