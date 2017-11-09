using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;

namespace Settings
{
    public class UserDomain
    {
        private static readonly UserDomain CurrentUser = null;

        // ReSharper disable once InconsistentNaming
        public string sAMAccountName { get; set; }
        public string DisplayName { get; set; }

        private UserDomain()
        {
        }

        public static UserDomain Current {
            get
            {
                if (CurrentUser != null)
                    return CurrentUser;
                try
                {
                    return GetUserDomain(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                }
                catch
                {
                    return null;
                }
            }
        }

        private static IEnumerable<string> GetDomains()
        {
            return (from Domain d in Forest.GetCurrentForest().Domains select d.Name).ToList();
        }

        public static UserDomain GetUserDomain(string login)
        {
            foreach (var domainName in GetDomains())
            {
                var context = new DirectoryContext(DirectoryContextType.Domain, domainName,
                LicenseSoftwareSettings.LdapUserName, LicenseSoftwareSettings.LdapPassword);
                var domain = Domain.GetDomain(context);
                using (var domainEntry = domain.GetDirectoryEntry())
                {
                    using (var searcher = new DirectorySearcher())
                    {
                        searcher.SearchRoot = domainEntry;
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.PropertiesToLoad.Add("samAccountName");
                        searcher.PropertiesToLoad.Add("displayName");
                        if (string.IsNullOrEmpty(login))
                            throw new ArgumentNullException("login","Не задано имя пользователя");
                        var loginParts = login.Split('\\');
                        searcher.Filter = string.Format(CultureInfo.InvariantCulture, 
                            "(&(objectClass=user)(samAccountName={0}))", loginParts[loginParts.Length - 1]);
                        try
                        {
                            var results = searcher.FindAll();
                            if (results.Count == 0)
                                continue;
                            var user = new UserDomain
                            {
                                DisplayName = results[0].Properties["displayName"][0].ToString(),
                                sAMAccountName = results[0].Properties["samAccountName"][0].ToString()
                            };
                            return user;
                        }
                        finally
                        {
                            domain.Dispose();
                        }
                    }
                }
            }
            return null;
        }
    }
}
