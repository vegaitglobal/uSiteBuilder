using System;

using Castle.DynamicProxy;

namespace Vega.USiteBuilder
{
    internal class DocumentTypePropertiesProxyGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonVirtualMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, System.Reflection.MethodInfo memberInfo)
        {
            return (memberInfo.IsGetterOrSetterForPropertyWithAttribute(typeof(DocumentTypePropertyAttribute))
                || memberInfo.IsGetterOrSetterForPropertyWithAttribute(typeof(MixinPropertyAttribute)))
                && memberInfo.IsCompilerGenerated();
        }

        public void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
        }

        public override bool Equals(object obj)
        {
            return ((obj != null) && (obj.GetType() == typeof(DocumentTypePropertiesProxyGenerationHook)));
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
