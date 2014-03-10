using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace Vega.USiteBuilder
{
    internal class MemberTypePropertyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            MemberTypeBase memberType = (MemberTypeBase)invocation.InvocationTarget;

            string propertyName = invocation.Method.GetPropertyName();

            if (invocation.Method.IsGetter())
            {
                if (memberType[propertyName] == null)
                {
                    Type typeDocType = MemberTypeManager.GetMemberTypeType(memberType.Source.ContentType.Alias);
                    PropertyInfo propInfo = typeDocType.GetProperty(propertyName);

                    object value = null;
                    try
                    {
                        value = MemberHelper.GetPropertyValue(memberType.Source, propInfo);
                        memberType[propertyName] = value;
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set the value of a Member type property {0}.{1} (member type: {2}) to value: '{3}' (value type: {4}). Error: {5}",
                            typeDocType.Name, propInfo.Name, propInfo.PropertyType.FullName,
                            value, value != null ? value.GetType().FullName : "null", exc.Message));
                    }
                }

                invocation.ReturnValue = memberType[propertyName];
                if (invocation.ReturnValue == null && invocation.Method.ReturnType == typeof(bool))
                    invocation.ReturnValue = false;
            }
            else
            {
                memberType[propertyName] = invocation.Arguments[0];
            }
        }
    }
}
