using System;
using System.Web.Security;

namespace Vega.USiteBuilder.MemberBuilder
{
    /// <summary>
    /// Base class for member types
    /// </summary>
    public abstract class MemberTypeBase
    {
        /// <summary>
        /// Creates new instasnce of member
        /// </summary>
        /// <param name="loginName">Member login name. Can be email</param>
        /// <param name="email">The email.</param>
        /// <param name="password">Member password</param>
        protected MemberTypeBase(string loginName, string email, string password)
        {
            LoginName = loginName;
            Password = password;
            Email = email;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="membershipProvider">The membership provider.</param>
        public void ChangePassword(string oldPassword, string newPassword, MembershipProvider membershipProvider)
        {
            membershipProvider.ChangePassword(LoginName, oldPassword, newPassword);
        }

        /// <summary>
        /// Saves this member.
        /// </summary>
        public void Save()
        {
            MemberHelper.Save(this);
        }

        /// <summary>
        /// Member id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Get's member's unique id
        /// </summary>
        public Guid UniqueId { get; internal set; }

        /// <summary>
        /// Member email. Must be unique.
        /// </summary>
        public string Email { get; set; }

        //public string[] Groups { get; set; }
        /// <summary>
        /// Login name of this user. Must be unique.
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Member Password
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// Create date of this content item.
        /// </summary>
        public DateTime CreateDate { get; internal set; }
    }
}