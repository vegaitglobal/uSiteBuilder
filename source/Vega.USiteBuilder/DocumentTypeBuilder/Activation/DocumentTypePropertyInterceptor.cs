using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace Vega.USiteBuilder
{
    internal class DocumentTypePropertyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            DocumentTypeBase docType = (DocumentTypeBase)invocation.InvocationTarget;

            string propertyName = invocation.Method.GetPropertyName();

            if (invocation.Method.IsGetter())
            {
                if (docType[propertyName] == null)
                {
                    Type typeDocType = DocumentTypeManager.GetDocumentTypeType(docType.Source.NodeTypeAlias);
                    PropertyInfo propInfo = typeDocType.GetProperty(propertyName);

                    object value = null;
                    try
                    {
                        value = ContentHelper.GetPropertyValueOrMixin(docType, propInfo);
                        docType[propertyName] = value;
                    }
                    catch (Exception exc)
                    {
                        throw new Exception(string.Format("Cannot set the value of a document type property {0}.{1} (document type: {2}) to value: '{3}' (value type: {4}). Error: {5}",
                            typeDocType.Name, propInfo.Name, propInfo.PropertyType.FullName,
                            value, value != null ? value.GetType().FullName : "", exc.Message));
                    }
                }

                invocation.ReturnValue = docType[propertyName];
                if (invocation.ReturnValue == null && invocation.Method.ReturnType == typeof(bool))
                    invocation.ReturnValue = false;
            }
            else
            {
                docType[propertyName] = invocation.Arguments[0];
            }
        }
    }
}
