namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using umbraco.cms.businesslogic.member;
using System.Web.Security;
    using System.Security.Cryptography;

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
        public MemberTypeBase(string loginName, string email, string password)
        {
            this.LoginName = loginName;
            this.Password = password;
            this.Email = email;
            
        }

        /// <summary>
        /// Changes the member password using password hash.
        /// </summary>
        /// <param name="newHashedPassword">The new hashed password.</param>
        public void ChangePassword(string newHashedPassword)
        {
            Member member = new Member(this.Id);

            member.ChangePassword(newHashedPassword);
        }

        /// <summary>
        /// Changes the member pasword using HMACSHA1 has on plain password.
        /// </summary>
        /// <param name="newPlainPassword">Plain password text</param>
        public void ChangeAndHashPassword(string newPlainPassword)
        {
            HMACSHA1 hash = new HMACSHA1();
            hash.Key = Encoding.Unicode.GetBytes(newPlainPassword);
            string hashedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(newPlainPassword)));

            this.ChangePassword(hashedPassword);
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

        /// <summary>
        /// Source member object.
        /// </summary>
        internal Member Source { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> value at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal object this[string index]
        {
            get
            {
                if (_propertyValues.ContainsKey(index))
                {
                    return _propertyValues[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_propertyValues.ContainsKey(index))
                {
                    _propertyValues[index] = value;
                }
                else
                {
                    _propertyValues.Add(index, value);
                }
            }
        }

        private Dictionary<string, object> _propertyValues = new Dictionary<string, object>();
    }
}
