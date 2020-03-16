using ePRJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Security;

namespace ePRJ.Models
{
    public class SiteRole : RoleProvider
    {
        HttpClient client = new HttpClient();
        public SiteRole()
        { 
            client.BaseAddress = new Uri("http://localhost/Data/");
        }
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var data = client.GetAsync(client.BaseAddress + "api/Account/GetUser/" + username + "/").Result.Content.ReadAsAsync<Account>().Result;
            if (data != null)
            {
                string[] result = { data.UserRole };
                return result;
            }
            return  new string[] { };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var data = client.GetAsync(client.BaseAddress + "api/Account/GetUser/" + username + "/").Result.Content.ReadAsAsync<Account>().Result;
            if (data.UserRole == roleName)
                return true;
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}