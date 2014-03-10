using System;

using Castle.DynamicProxy;

using umbraco.NodeFactory;
using umbraco.cms.businesslogic.member;

namespace Vega.USiteBuilder
{
    internal class MemberTypeActivator
    {
        private ProxyGenerator _generator;
        private ProxyGenerationOptions _options;
        private IInterceptor[] _interceptors;

        public MemberTypeActivator()
        {
            _generator = new ProxyGenerator();
            _options = new ProxyGenerationOptions(new MemberTypePropertiesProxyGenerationHook());
            _interceptors = new IInterceptor[] 
                               {
                                   new MemberTypePropertyInterceptor()
                               };
        }

        public virtual T CreateAndPopulateTypedInstance<T>(Member member) where T : MemberTypeBase
        {
            T retVal = null;
            if (member != null)
            {
                Type typeMemberType = MemberTypeManager.GetMemberTypeType(member.ContentType.Alias);
                T typedMember = (T)CreateInstance(typeMemberType, new[] { member.LoginName, member.Email, member.Password });
                if (MemberHelper.PopuplateInstance<T>(member, typeMemberType, typedMember))
                {
                    retVal = typedMember;
                }
            }

            return retVal;
        }

        public virtual object CreateInstance(Type typeMemberType)
        {
            return CreateInstance(typeMemberType, new object[] {});
        }

        protected virtual object CreateInstance(Type typeMemberType, object[] ctorArguments)
        {
            return _generator.CreateClassProxy(typeMemberType, new Type[] { }, _options, ctorArguments, _interceptors);
        }
    }
}
