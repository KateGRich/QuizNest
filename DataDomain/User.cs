using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class User
    {
        public int UserID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? ReactivationDate { get; set; }
        public bool Active { get; set; }
    }
    public class UserVM : User
    {
        public string Name { get { return GivenName + " " + FamilyName; } }
        public List<string> Roles { get; set; }
        public string RoleList
        {
            get
            {
                string roleList = "";
                if(Roles.Count > 2)
                {
                    for(int i = 0; i < Roles.Count; i++)
                    {
                        roleList += Roles[i];
                        if(i < Roles.Count - 2)
                        {
                            roleList += ", ";
                        }
                        else if(i == Roles.Count - 2)
                        {
                            roleList += ", and ";
                        }
                    }
                }
                else if(Roles.Count == 2)
                {
                    roleList += Roles[0] + " and " + Roles[1];
                }
                else
                {
                    roleList += Roles[0];
                }
                return roleList;
            }
        }
    }
}