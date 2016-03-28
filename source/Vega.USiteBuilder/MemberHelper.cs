using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.property;
using Vega.USiteBuilder.DocumentTypeBuilder;
using Vega.USiteBuilder.MemberBuilder;

namespace Vega.USiteBuilder
{
    /// <summary>
    /// This class contains methods for getting the strongly typed members from Umbraco
    /// </summary>
    public static class MemberHelper
    {
        #region [Save methods]
        /// <summary>
        /// Updates or adds the member using current user. If member already exists, it updates it. 
        /// If member doesn't exists, it creates new member.
        /// </summary>
        /// <param name="member">Content item to update/add</param>
        public static void Save(MemberTypeBase member)
        {
            Save(member, Util.GetAdminUser());
        }

        /// <summary>
        /// Updates or adds the member. If member already exists, it updates it. 
        /// If member doesn't exists, it creates new member.
        /// </summary>
        /// <param name="member">Member to update/add</param>
        /// <param name="user">User used for add or updating the content</param>
        private static void Save(MemberTypeBase member, User user)
        {
            if (user == null)
            {
                throw new Exception("User cannot be null!");
            }

            if (string.IsNullOrEmpty(member.LoginName))
            {
                throw new Exception("Member Login Name cannot be empty");
            }

            MemberType memberType = MemberTypeManager.GetMemberType(member.GetType());

            Member umember;
            if (member.Id == 0) // member is new so create Member
            {
                Member.MakeNew(member.LoginName, member.Email, memberType, user);

                // reload
                umember = Member.GetMemberFromLoginName(member.LoginName);
                umember.Password = member.Password;
                member.Id = umember.Id;
            }
            else // member already exists, so load it
            {
                umember = new Member(member.Id);
            }
            
            umember.Email = member.Email;
            umember.LoginName = member.LoginName;

            foreach (PropertyInfo propInfo in member.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a Document Type
                }

                string propertyName;
                string propertyAlias;
                MemberTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                Property property = umember.getProperty(propertyAlias);
                if (property == null)
                {
                    throw new Exception(string.Format("Property '{0}' not found in this node: {1}. Content type: {2}.",
                        propertyAlias, member.Id, memberType.Alias));
                }

                if (ContentHelper.PropertyConvertors.ContainsKey(propInfo.PropertyType))
                {
                    property.Value = ContentHelper.PropertyConvertors[propInfo.PropertyType].ConvertValueWhenWrite(propInfo.GetValue(member, null));
                }
                else
                {
                    property.Value = propInfo.GetValue(member, null);
                }
            }

            umember.Save();
        }
        #endregion

        #region [Get methods]

        /// <summary>
        /// Gets all members
        /// </summary>
        /// <returns>List of all members</returns>
        public static List<MemberTypeBase> GetAllMembers()
        {
            List<MemberTypeBase> retVal = new List<MemberTypeBase>();
            Member[] members = Member.GetAll;

            foreach (Member member in members)
            {
                MemberTypeBase m = GetMember(member);
                if (m != null)
                {
                    retVal.Add(m);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get's the currently logged in member
        /// </summary>
        /// <returns></returns>
        public static MemberTypeBase GetCurrentMember()
        {
            return GetMember(Member.GetCurrentMember());
        }

        /// <summary>
        /// Gets members by name
        /// </summary>
        /// <param name="usernameToMatch">Part or full username (login name)</param>
        /// <param name="matchByNameInsteadOfLogin">If true, uses it compares usernameToMatch with name instead of login name</param>
        /// <returns></returns>
        public static List<MemberTypeBase> GetMembersByName(string usernameToMatch, bool matchByNameInsteadOfLogin)
        {
            Member[] members = Member.GetMemberByName(usernameToMatch, matchByNameInsteadOfLogin);

            return members.Select(GetMember).Where(m => m != null).ToList();
        }

        /// <summary>
        /// Gets member by email. Returns null if not found.
        /// </summary>
        /// <param name="email">Member email</param>
        /// <returns>Member or null if not member with given email is not found</returns>
        public static MemberTypeBase GetMemberFromEmail(string email)
        {
            return GetMember(Member.GetMemberFromEmail(email));
        }

        /// <summary>
        /// Gets member by login and encoded password
        /// </summary>
        /// <param name="loginName">Member login name</param>
        /// <param name="password">Member password</param>
        /// <returns>Member found or null if no members are found</returns>
        public static MemberTypeBase GetMemberFromLoginAndEncodedPassword(string loginName, string password)
        {
            return GetMember(Member.GetMemberFromLoginAndEncodedPassword(loginName, password));
        }

        /// <summary>
        /// Gets member by login name
        /// </summary>
        /// <param name="loginName">Member login name</param>
        /// <returns>Member or null if member is not found</returns>
        public static MemberTypeBase GetMemberFromLoginName(string loginName)
        {
            return GetMember(Member.GetMemberFromLoginName(loginName));
        }

        /// <summary>
        /// Gets member from login name and password.
        /// </summary>
        /// <param name="loginName">Member login name</param>
        /// <param name="password">Password</param>
        /// <returns>Member or null if no members are founds</returns>
        public static MemberTypeBase GetMemberFromLoginNameAndPassword(string loginName, string password)
        {
            return GetMember(Member.GetMemberFromLoginNameAndPassword(loginName, password));
        }

        /// <summary>
        /// Gets member by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MemberTypeBase GetMemberById(int id)
        {
            MemberTypeBase retVal = null;
            Member member = null;

            try
            {
                member = new Member(id);
            }
            catch 
            { 
                // member is not found
            }

            if (member != null)
            {
                retVal = GetMember(member);
            }

            return retVal;
        }

        /// <summary>
        /// Get's the member.
        /// </summary>
        /// <param name="member">Member</param>
        /// <returns>Member</returns>
        public static MemberTypeBase GetMember(Member member)
        {
            if (member == null)
            {
                return null;
            }

            MemberTypeBase retVal = null;

            if (member.ContentType.Alias == null)
            {
                throw new Exception(string.Format("Member has no associated member type. Member: id: {0}, login name: {1}", member.Id, member.LoginName));
            }

            Type typeMemberType = MemberTypeManager.GetMemberTypeType(member.ContentType.Alias);
            if (typeMemberType != null)
            {
                retVal = ((MemberTypeBase)Activator.CreateInstance(typeMemberType, member.LoginName, member.Email, member.Password));

                if (retVal == null)
                {
                    throw new ArgumentException(string.Format("Member type class whose member type alias is '{0}' not found!. Member: (id: {1}, login name: {2}).",
                        member.ContentType.Alias, member.Id, member.LoginName));
                }

                retVal.Id = member.Id;
                retVal.Email = member.Email;
                retVal.UniqueId = member.UniqueId;
                retVal.CreateDate = member.CreateDateTime;

                foreach (PropertyInfo propInfo in typeMemberType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    MemberTypePropertyAttribute propAttr = Util.GetAttribute<MemberTypePropertyAttribute>(propInfo);
                    if (propAttr == null)
                    {
                        continue; // skip this property - not part of a Document Type
                    }

                    string propertyName;
                    string propertyAlias;
                    MemberTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                    Property property = member.getProperty(propertyAlias);

                    object value = null;
                    try
                    {
                        if (property == null)
                        {
                            value = null;
                        }
                        else if (propInfo.PropertyType == typeof(Boolean))
                        {
                            if (property.Value == null || String.IsNullOrEmpty(Convert.ToString(property.Value)) 
                                || Convert.ToString(property.Value) == "0")
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                        }
                        else if (ContentHelper.PropertyConvertors.ContainsKey(propInfo.PropertyType))
                        {
                            // will be transformed later. TODO: move transformation here
                            value = property.Value;
                        }
                        else if (propInfo.PropertyType.IsGenericType &&
                                 propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            if (!String.IsNullOrEmpty(Convert.ToString(property.Value)))
                            {
                                value = Convert.ChangeType(property.Value,
                                    Nullable.GetUnderlyingType(propInfo.PropertyType));
                            }

                            // TODO: If data type is DateTime and is nullable and is less than 1.1.1000 than set it to NULL
                        }
                        else
                        {
                            value = Convert.ChangeType(property.Value, propInfo.PropertyType);
                        }

                        if (ContentHelper.PropertyConvertors.ContainsKey(propInfo.PropertyType))
                        {
                            value = ContentHelper.PropertyConvertors[propInfo.PropertyType].ConvertValueWhenRead(value);
                        }

                        propInfo.SetValue(retVal, value, null);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set the value of a Member type property {0}.{1} (member type: {2}) to value: '{3}' (value type: {4}). Error: {5}",
                            typeMemberType.Name, propInfo.Name, propInfo.PropertyType.FullName,
                            value, value != null ? value.GetType().FullName : "null", exc.Message));
                    }
                }
            }

            return retVal;
        }
        #endregion

        #region [Login/Logout]
        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="username">Member username</param>
        /// <param name="password">Member password</param>
        /// <param name="rememberMe">If true, cookie will stay persistent untill manual logout</param>
        /// <param name="timeOut">If user is not active on the website, after timeout seconds it'll be loggout</param>
        /// <param name="redirectAfterAuthentication">If true, application redirects user away from Login page to url given in web.config (Forms authentication settings).</param>
        /// <returns>True if user successfully logged in or false otherwise</returns>
        public static bool LoginWithFormsAuthentication(string username, string password, bool rememberMe, int timeOut, bool redirectAfterAuthentication)
        {
            bool retVal = true;

            if (Membership.ValidateUser(username, password))
            {
                LoginWithFormsAuthentication(username, rememberMe, timeOut, redirectAfterAuthentication);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Logins the specified user identified by username without password checking.
        /// </summary>
        /// <param name="username">Member username</param>
        /// <param name="rememberMe">If true, cookie will stay persistent untill manual logout</param>
        /// <param name="timeOut">If user is not active on the website, after timeout seconds it'll be loggout</param>
        /// <param name="redirectAfterAuthentication">If true, application redirects user away from Login page to url given in web.config (Forms authentication settings).</param>
        /// <returns>True if user successfully logged in or false otherwise</returns>
        public static void LoginWithFormsAuthentication(string username, bool rememberMe, int timeOut, bool redirectAfterAuthentication)
        {
            DateTime expirationDate = DateTime.Now.AddMinutes(timeOut);

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(username, rememberMe, timeOut);

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (rememberMe)
            {
                cookie.Expires = expirationDate;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;

            HttpContext.Current.Response.Cookies.Add(cookie);
            if (redirectAfterAuthentication)
            {
                FormsAuthentication.RedirectFromLoginPage(username, false);
            }
        }

        /// <summary>
        /// Logouts the current logged-in user.
        /// </summary>
        public static void LogoutFormsAuthentication()
        {
            FormsAuthentication.SignOut();
            //HttpContext.Current.Response.Redirect(FormsAuthentication.DefaultUrl);
        }

        /// <summary>
        /// Determines whether user is authenticated.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if user is authenticated; otherwise <c>false</c>.
        /// </returns>
        public static bool IsLoggedIn()
        {
            bool retVal = false;

            if (HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated && Member.GetCurrentMember() != null)
            {
                retVal = true;
            }
            else
            {
                if (HttpContext.Current.Response.Cookies.AllKeys.Contains(FormsAuthentication.FormsCookieName))
                {
                    HttpCookie authCookie = HttpContext.Current.Response.Cookies.Get(FormsAuthentication.FormsCookieName);
                    if (authCookie != null && !String.IsNullOrEmpty(authCookie.Value))
                    {
                        retVal = true;
                    }
                }
            }

            return retVal;
        }
        #endregion
    }
}
