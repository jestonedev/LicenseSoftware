using System;
using System.Security.Cryptography;
using System.Text;

namespace Settings
{
    public static class LicenseSoftwareSettings
    {
        private const string Key = "Z33wYQs+rZOGMUH0RGj8GIKggOB82xwhACg9JCA6/kw=";
        private const string Iv = "E0Ci3P9JPj43jUdKNSvtmQ==";
        private static readonly Aes Aes = Aes.Create();

        public static string ConnectionString {
            get {
                try
                {
                    var inputBuffer = Convert.FromBase64String(Properties.Settings.Default.ConnectionString);
                    var decryptor = Aes.CreateDecryptor(Convert.FromBase64String(Key), Convert.FromBase64String(Iv));
                    var outputBuffer = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
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
                var inputBuffer = Encoding.UTF8.GetBytes(value);
                var encryptor = Aes.CreateEncryptor(Convert.FromBase64String(Key), Convert.FromBase64String(Iv));
                var outputBuffer = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                Properties.Settings.Default.ConnectionString = Convert.ToBase64String(outputBuffer);
            }
        }

        public static string LdapUserName
        {
            get
            {
                return Properties.Settings.Default.LDAPUserName;
            }
            set
            {
                Properties.Settings.Default.LDAPUserName = value;
            }
        }

        public static string LdapPassword
        {
            get
            {
                try
                {
                    var inputBuffer = Convert.FromBase64String(Properties.Settings.Default.LDAPEncryptedPassword);
                    var decryptor = Aes.CreateDecryptor(Convert.FromBase64String(Key), Convert.FromBase64String(Iv));
                    var outputBuffer = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
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
                var inputBuffer = Encoding.UTF8.GetBytes(value);
                var encryptor = Aes.CreateEncryptor(Convert.FromBase64String(Key), Convert.FromBase64String(Iv));
                var outputBuffer = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                Properties.Settings.Default.LDAPEncryptedPassword = Convert.ToBase64String(outputBuffer);
            }
        }

        public static bool UseLdap
        {
            get
            {
                return Properties.Settings.Default.UseLDAP;
            }
            set
            {
                Properties.Settings.Default.UseLDAP = value;
            }
        }

        public static string ActivityManagerPath
        {
            get
            {
                return Properties.Settings.Default.ActivityManagerPath;
            }
            set
            {
                Properties.Settings.Default.ActivityManagerPath = value;
            }
        }

        public static string ActivityManagerOutputCodePage
        {
            get
            {
                return Properties.Settings.Default.ActivityManagerOutputCodePage;
            }
            set
            {
                Properties.Settings.Default.ActivityManagerOutputCodePage = value;
            }
        }

        public static string ActivityManagerConfigsPath
        {
            get
            {
                return Properties.Settings.Default.ActivityManagerConfigsPath;
            }
            set
            {
                Properties.Settings.Default.ActivityManagerConfigsPath = value;
            }
        }

        public static int MaxDbConnectionCount
        {
            get
            {
                return Properties.Settings.Default.MaxDBConnectionCount;
            }
            set
            {
                Properties.Settings.Default.MaxDBConnectionCount = value;
            }
        }

        public static int DataModelsCallbackUpdateTimeout
        {
            get
            {
                return Properties.Settings.Default.DataModelsCallbackUpdateTimeout;
            }
            set
            {
                Properties.Settings.Default.DataModelsCallbackUpdateTimeout = value;
            }
        }

        public static int CalcDataModelsUpdateTimeout
        {
            get
            {
                return Properties.Settings.Default.CalcDataModelsUpdateTimeout;
            }
            set
            {
                Properties.Settings.Default.CalcDataModelsUpdateTimeout = value;
            }
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
