using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using System.Globalization;

namespace LicenseSoftware
{
    public class UserDomain
    {
        private static UserDomain _currentUser = null;

        public string sAMAccountName { get; set; }
        public string DisplayName { get; set; }

        private UserDomain()
        {
        }

        public static UserDomain Current {
            get {
                if (_currentUser != null)
                    return _currentUser;
                else
                {
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
        }

        private static ICollection<string> GetDomains()
        {
            ICollection<string> domains = new List<string>();
            foreach (Domain d in Forest.GetCurrentForest().Domains)
                domains.Add(d.Name);

            return domains;
        }

        public static UserDomain GetUserDomain(string login)
        {
            foreach (string domainName in GetDomains())
            {
                DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, domainName,
                LicenseSoftwareSettings.LDAPUserName, LicenseSoftwareSettings.LDAPPassword);
                Domain domain = Domain.GetDomain(context);
                using (DirectoryEntry domainEntry = domain.GetDirectoryEntry())
                {
                    using (DirectorySearcher searcher = new DirectorySearcher())
                    {
                        searcher.SearchRoot = domainEntry;
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.PropertiesToLoad.Add("samAccountName");
                        searcher.PropertiesToLoad.Add("displayName");
                        if (String.IsNullOrEmpty(login))
                            throw new ArgumentNullException("login","Не задано имя пользователя");
                        string[] loginParts = login.Split('\\');
                        searcher.Filter = string.Format(CultureInfo.InvariantCulture, 
                            "(&(objectClass=user)(samAccountName={0}))", loginParts[loginParts.Count() - 1]);
                        try
                        {
                            SearchResultCollection results = searcher.FindAll();
                            if (results == null || results.Count == 0)
                                continue;
                            UserDomain user = new UserDomain();
                            user.DisplayName = results[0].Properties["displayName"][0].ToString();
                            user.sAMAccountName = results[0].Properties["samAccountName"][0].ToString();
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
