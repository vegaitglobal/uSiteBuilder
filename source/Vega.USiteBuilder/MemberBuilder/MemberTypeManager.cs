using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.property;
using Vega.USiteBuilder.Configuration;
using Vega.USiteBuilder.DocumentTypeBuilder;

namespace Vega.USiteBuilder.MemberBuilder
{
    /// <summary>
    /// Manages member type synchronization
    /// </summary>
    class MemberTypeManager : ManagerBase
    {
        // Holds all members types found in 
        // Type = Member type type (subclass of MemberTypeBase), string = member type alias
        private static readonly Dictionary<string, Type> _memberTypes = new Dictionary<string, Type>();

        // indicates if any of synced member types had a default value
        private bool _hadDefaultValues;

        /// <summary>
        /// Returns true if there's any member type synchronized (defined)
        /// </summary>
        /// <returns></returns>
        public static bool HasSynchronizedMemberTypes()
        {
            return _memberTypes.Count > 0;
        }

        public void Synchronize()
        {
            _memberTypes.Clear();

            SynchronizeMemberTypes(typeof(MemberTypeBase));

            if (_hadDefaultValues) // if there were default values set subscribe to News event in which we'll set default values.
            {
                // subscribe to New event
                //Member.AfterNew += new Member.NewEventHandler(this.Member_New);
                Member.New += Member_New;
            }
        }

        void Member_New(Member member, NewEventArgs e)
        {
            Type typeMemberType = GetMemberTypeType(member.ContentType.Alias);
            foreach (PropertyInfo propInfo in typeMemberType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                MemberTypePropertyAttribute propAttr = Util.GetAttribute<MemberTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a Member type
                }

                string propertyName;
                string propertyAlias;
                ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                if (propAttr.DefaultValue != null)
                {
                    try
                    {
                        Property property = member.getProperty(propertyAlias);
                        property.Value = propAttr.DefaultValue;
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set default value ('{0}') for property {1}.{2}. Error: {3}",
                            propAttr.DefaultValue, typeMemberType.Name, propInfo.Name, exc.Message), exc);
                    }
                }
            }
        }

        #region [Static methods]
        public static string GetMemberTypeAlias(Type typeMemberType)
        {
            return typeMemberType.Name;
        }

        public static void ReadPropertyNameAndAlias(PropertyInfo propInfo, DocumentTypePropertyAttribute propAttr,
            out string name, out string alias)
        {
            // set name
            name = string.IsNullOrEmpty(propAttr.Name) ? propInfo.Name : propAttr.Name;

            // set a default alias
            alias = propInfo.Name.Substring(0, 1).ToLower();

            // if an alias has been set, use that one explicitly
            if (!String.IsNullOrEmpty(propAttr.Alias)) 
            {
                alias = propAttr.Alias;

            // otherwise
            } 
            else 
            {
                // create the alias from the name 
                if (propInfo.Name.Length > 1) {
                    alias += propInfo.Name.Substring(1, propInfo.Name.Length - 1);
                }

                // This is required because it seems that Umbraco has a bug when property type alias is called pageName.
                if (alias == "pageName") {
                    alias += "_";
                }
            }
        }

        public static MemberType GetMemberType(Type typeMemberType)
        {
            return MemberType.GetByAlias(GetMemberTypeAlias(typeMemberType));
        }

        public static Type GetMemberTypeType(string memberTypeAlias)
        {
            Type retVal = null;

            if (_memberTypes.ContainsKey(memberTypeAlias))
            {
                retVal = _memberTypes[memberTypeAlias];
            }

            return retVal;
        }
        #endregion

        #region [Member types creation]
        private void SynchronizeMemberTypes(Type basetypeMemberType)
        {
            foreach (Type typeMemberType in Util.GetFirstLevelSubTypes(basetypeMemberType))
            {
                SynchronizeMemberType(typeMemberType);
            }
        }

        private void SynchronizeMemberType(Type typeMemberType)
        {
            MemberTypeAttribute memberTypeAttr = GetMemberTypeAttribute(typeMemberType);

            string memberTypeName = string.IsNullOrEmpty(memberTypeAttr.Name) ? typeMemberType.Name : memberTypeAttr.Name;
            string memberTypeAlias = GetMemberTypeAlias(typeMemberType);

            try
            {
                AddToSynchronized(typeMemberType.Name, memberTypeAlias, typeMemberType);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Member type with alias '{0}' already exists! Please use unique class names as class name is used as alias. Member type causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    memberTypeAlias, typeMemberType.FullName, typeMemberType.Assembly.FullName, exc.Message));
            }

            _memberTypes.Add(memberTypeAlias, typeMemberType);

            if (!USiteBuilderConfiguration.SuppressSynchronization)
            {

                var memberType = MemberType.GetByAlias(memberTypeAlias) ??
                                        MemberType.MakeNew(siteBuilderUser, memberTypeName);

                memberType.Text = memberTypeName;
                memberType.Alias = memberTypeAlias;
                memberType.IconUrl = memberTypeAttr.IconUrl;
                memberType.Thumbnail = memberTypeAttr.Thumbnail;
                memberType.Description = memberTypeAttr.Description;

                memberType.MasterContentType = 0;

                SynchronizeMemberTypeProperties(typeMemberType, memberType);

                memberType.Save();
            }
        }

        /// <summary>
        /// Get's the member type attribute or returns attribute with default values if attribute is not found
        /// </summary>
        /// <param name="typeMemberType">An member type type</param>
        /// <returns></returns>
        private MemberTypeAttribute GetMemberTypeAttribute(Type typeMemberType)
        {
            MemberTypeAttribute retVal = Util.GetAttribute<MemberTypeAttribute>(typeMemberType) ??
                                         CreateDefaultMemberTypeAttribute(typeMemberType);

            return retVal;
        }

        private MemberTypeAttribute CreateDefaultMemberTypeAttribute(Type typeMemberType)
        {
            MemberTypeAttribute retVal = new MemberTypeAttribute
            {
                Name = typeMemberType.Name,
                IconUrl = DocumentTypeDefaultValues.IconUrl,
                Thumbnail = DocumentTypeDefaultValues.Thumbnail
            };

            return retVal;
        }
        #endregion


        #region [Member type properties synchronization]
        private void SynchronizeMemberTypeProperties(Type typeMemberType, MemberType memberType)
        {
            int propertySortOrder = 0;
            foreach (PropertyInfo propInfo in typeMemberType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                DocumentTypePropertyAttribute propAttr = Util.GetAttribute<DocumentTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a document type
                }

                // getting name and alias
                string propertyName;
                string propertyAlias;
                DocumentTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                if (HasObsoleteAttribute(propInfo))
                {
                    Util.DeletePropertyType(memberType, propertyAlias);
                    continue;
                }

                if (propAttr.DefaultValue != null)
                {
                    _hadDefaultValues = true; // at least one property has a default value
                }


                umbraco.cms.businesslogic.datatype.DataTypeDefinition dataTypeDefinition = GetDataTypeDefinition(typeMemberType, propAttr, propInfo);

                // getting property if already exists, or creating new if it not exists
                var propertyType = memberType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                if (propertyType == null) // if not exists, create it
                {
                    memberType.AddPropertyType(dataTypeDefinition, propertyAlias, propertyName);
                    propertyType = memberType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                }

                //// Setting up the tab of this property. If tab doesn't exists, create it.
                if (!string.IsNullOrEmpty(propAttr.TabAsString) && !String.Equals(propAttr.TabAsString, DocumentTypeDefaultValues.TabGenericProperties, StringComparison.CurrentCultureIgnoreCase))
                {
                    // try to find this tab
                    umbraco.cms.businesslogic.propertytype.PropertyTypeGroup pg = memberType.PropertyTypeGroups.FirstOrDefault(x => x.Name == propAttr.TabAsString);
                    if (pg == null) // if found
                    {
                        memberType.AddVirtualTab(propAttr.TabAsString);
                        pg = memberType.PropertyTypeGroups.FirstOrDefault(x => x.Name == propAttr.TabAsString);
                    }

                    if (propAttr.TabOrder.HasValue)
                    {
                        pg.SortOrder = propAttr.TabOrder.Value;
                    }

                    propertyType.PropertyTypeGroup = pg.Id;
                }

                propertyType.Name = propertyName;
                propertyType.Mandatory = propAttr.Mandatory;
                propertyType.ValidationRegExp = propAttr.ValidationRegExp;
                propertyType.Description = propAttr.Description;
                propertyType.SortOrder = propertySortOrder;

                propertyType.Save();

                propertySortOrder++;
            }
        }
        #endregion
    }
}
