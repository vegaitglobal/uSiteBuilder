namespace Vega.USiteBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Marks the property as a document type property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DocumentTypePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTypePropertyAttribute"/> class.
        /// </summary>
        public DocumentTypePropertyAttribute() 
        {
            // setting up default values
            this.Tab = "";
            this.Mandatory = false;
            this.ValidationRegExp = "";
            this.Description = "";
            this.DefaultValue = null;
            this.Alias = "";
        }

        /// <summary>
        /// Only umbraco data type is required.
        /// If you don't specify other named parameters, default values will be used.
        /// </summary>
        /// <param name="type">Umbraco Data Type related with this property</param>
        public DocumentTypePropertyAttribute(UmbracoPropertyType type) : this()
        {
            this.Type = type;

        }

        /// <summary>
        /// Alias of this property.  
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Name of this property. If name is not specified, name of the class is taken as a property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sets the corresponding umbraco data type. 
        /// For types not covered with this enumeration (including custom types) use UmbracoPropertyType.Other and 
        /// than set OtherTypeName attribute.
        /// </summary>
        public UmbracoPropertyType? Type { get; private set; }

        /// <summary>
        /// Gets or sets the type of the custom datatype.
        /// </summary>
        /// <value>
        /// The type of the other custom datatype.
        /// </value>
        public Type OtherType { get; set; }
        
        /// <summary>
        /// Name of the a data type. This property is used only if Type is set to UmbracoPropertyType.Other.
        /// </summary>
        public string OtherTypeName { get; set; }

        /// <summary>
        /// Tab on which this property will be shown.
        /// It can be string but it can also be an Enum value in which case enum value will be used as sort order value for this tab.
        /// If specified Tab does not exists in this document type it will be automatically created.
        /// </summary>
        public object Tab { get; set; }

        /// <summary>
        /// Gets the tab name.
        /// </summary>
        internal string TabAsString
        {
            get
            {
                if (this.Tab == null)
                {
                    return "";
                }
                else if (this.Tab is Enum)
                {
                    // first try to see if there is TabNameAttribute related to this Enum
                    Enum en = (Enum)this.Tab;
                    Type type = en.GetType();
                    MemberInfo[] memInfo = type.GetMember(en.ToString());
                    if (memInfo != null && memInfo.Length > 0)
                    {
                        object[] attrs = memInfo[0].GetCustomAttributes(typeof(TabNameAttribute), false);
                        if (attrs != null && attrs.Length > 0)
                        {
                            return ((TabNameAttribute)attrs[0]).Name;
                        }
                    }

                    // if not, just return Enum name
                    return en.ToString();
                }
                else //this.Tab is String or something else
                {
                    return this.Tab.ToString();
                }
            }
        }

        /// <summary>
        /// Get's tab order or tab is given as Enum. If not, value of this property will be null
        /// </summary>
        internal int? TabOrder
        {
            get
            {
                if (this.Tab != null && this.Tab is Enum)
                {
                    return Convert.ToInt32((Enum)this.Tab);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Indicates if this property is mandatory when user creates content.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Validation reg expression.
        /// </summary>
        public string ValidationRegExp { get; set; }

        /// <summary>
        /// Property type description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Default value for the property. Note that setting up default values for properties 
        /// whose type is a complex type (e.g. Related links) requires this value to be right 
        /// formated xml.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the custom type converter.
        /// </summary>
        /// <value>
        /// The custom type converter.
        /// </value>
        public Type CustomTypeConverter { get; set; }
    }
}
