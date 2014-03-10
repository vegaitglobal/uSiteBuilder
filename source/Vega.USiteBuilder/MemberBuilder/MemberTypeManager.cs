using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Vega.USiteBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using umbraco.cms.businesslogic;
    using umbraco.cms.businesslogic.member;

    /// <summary>
    /// Manages member type synchronization
    /// </summary>
    class MemberTypeManager : ManagerBase
    {
        // Holds all members types found in 
        // Type = Member type type (subclass of MemberTypeBase), string = member type alias
        private static Dictionary<string, Type> _memberTypes = new Dictionary<string, Type>();
        static readonly IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;

        // indicates if any of synced member types had a default value
        private bool _hadDefaultValues = false;

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

            this.SynchronizeMemberTypes(typeof(MemberTypeBase));

            if (this._hadDefaultValues) // if there were default values set subscribe to News event in which we'll set default values.
            {
                // subscribe to New event
                Member.New += Member_New;
            }
        }

        void Member_New(Member member, NewEventArgs e)
        {
            Type typeMemberType = MemberTypeManager.GetMemberTypeType(member.ContentType.Alias);
            foreach (PropertyInfo propInfo in typeMemberType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                MemberTypePropertyAttribute propAttr = Util.GetAttribute<MemberTypePropertyAttribute>(propInfo);
                if (propAttr == null)
                {
                    continue; // skip this property - not part of a Member type
                }

                string propertyName;
                string propertyAlias;
                MemberTypeManager.ReadPropertyNameAndAlias(propInfo, propAttr, out propertyName, out propertyAlias);

                if (propAttr.DefaultValue != null)
                {
                    try
                    {
                        umbraco.cms.businesslogic.property.Property property = member.getProperty(propertyAlias);
                        property.Value = Convert.ChangeType(propAttr.DefaultValue, propInfo.PropertyType);
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
            return MemberType.GetByAlias(MemberTypeManager.GetMemberTypeAlias(typeMemberType));
        }

        public static Type GetMemberTypeType(string memberTypeAlias)
        {
            if (!HasSynchronizedMemberTypes())
            {
                FillMemberTypes(typeof(MemberTypeBase));
            }

            Type retVal = null;
            if (MemberTypeManager._memberTypes.ContainsKey(memberTypeAlias))
            {
                retVal = MemberTypeManager._memberTypes[memberTypeAlias];
            }

            return retVal;
        }

        private static void FillMemberTypes(Type baseMemberType)
        {
            foreach (Type typeDocType in Util.GetFirstLevelSubTypes(baseMemberType))
            {
                string memberTypeAlias = GetMemberTypeAlias(typeDocType);
                _memberTypes.Add(memberTypeAlias, typeDocType);

                // create all children document types
                FillMemberTypes(typeDocType);
            }
        }
        #endregion

        #region [Member types creation]
        private void SynchronizeMemberTypes(Type basetypeMemberType)
        {
            foreach (Type typeMemberType in Util.GetFirstLevelSubTypes(basetypeMemberType))
            {
                this.SynchronizeMemberType(typeMemberType, basetypeMemberType);
            }
        }

        private void SynchronizeMemberType(Type typeMemberType, Type basetypeMemberType)
        {
            MemberTypeAttribute memberTypeAttr = this.GetMemberTypeAttribute(typeMemberType);

            string memberTypeName = string.IsNullOrEmpty(memberTypeAttr.Name) ? typeMemberType.Name : memberTypeAttr.Name;
            string memberTypeAlias = MemberTypeManager.GetMemberTypeAlias(typeMemberType);

            try
            {
                this.AddToSynchronized(typeMemberType.Name, memberTypeAlias, typeMemberType);
            }
            catch (ArgumentException exc)
            {
                throw new Exception(string.Format("Member type with alias '{0}' already exists! Please use unique class names as class name is used as alias. Member type causing the problem: '{1}' (assembly: '{2}'). Error message: {3}",
                    memberTypeAlias, typeMemberType.FullName, typeMemberType.Assembly.FullName, exc.Message));
            }


            _memberTypes.Add(memberTypeAlias, typeMemberType);
            
            MemberType memberType = MemberType.GetByAlias(memberTypeAlias);
            if (memberType == null)
            {
                memberType = MemberType.MakeNew(new umbraco.BusinessLogic.User(0), memberTypeAttr.Name);
            }

            memberType.Alias = memberTypeAlias;
            memberType.IconUrl = memberTypeAttr.IconUrl;
            memberType.Thumbnail = memberTypeAttr.Thumbnail;
            memberType.Description = memberTypeAttr.Description;

            SynchronizeMemberTypeProperties(typeMemberType, memberType);

            memberType.Save();
        }

        /// <summary>
        /// Get's the member type attribute or returns attribute with default values if attribute is not found
        /// </summary>
        /// <param name="typeMemberType">An member type type</param>
        /// <returns></returns>
        private MemberTypeAttribute GetMemberTypeAttribute(Type typeMemberType)
        {
            MemberTypeAttribute retVal = Util.GetAttribute<MemberTypeAttribute>(typeMemberType);

            if (retVal == null)
            {
                retVal = this.CreateDefaultMemberTypeAttribute(typeMemberType);
            }

            return retVal;
        }

        private MemberTypeAttribute CreateDefaultMemberTypeAttribute(Type typeMemberType)
        {
            MemberTypeAttribute retVal = new MemberTypeAttribute();

            retVal.Name = typeMemberType.Name;
            retVal.IconUrl = DocumentTypeDefaultValues.IconUrl;
            retVal.Thumbnail = DocumentTypeDefaultValues.Thumbnail;

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
                umbraco.cms.businesslogic.propertytype.PropertyType propertyType = memberType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                if (propertyType == null) // if not exists, create it
                {
                    memberType.AddPropertyType(dataTypeDefinition, propertyAlias, propertyName);
                    propertyType = memberType.PropertyTypes.FirstOrDefault(p => p.Alias == propertyAlias);
                }

                //// Setting up the tab of this property. If tab doesn't exists, create it.
                if (!string.IsNullOrEmpty(propAttr.TabAsString) && propAttr.TabAsString.ToLower() != DocumentTypeDefaultValues.TabGenericProperties.ToLower())
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

                propertySortOrder++;
            }
        }
        #endregion
    }
}
