using System;

using umbraco.cms.businesslogic.member;

namespace Vega.USiteBuilder
{
    internal class MemberTypeResolver
    {
        private static MemberTypeResolver _instance;

        public static MemberTypeResolver Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MemberTypeResolver();

                return _instance;
            }
        }

        public MemberTypeActivator Activator { get; set; }

        private MemberTypeResolver()
        {
            Activator = new MemberTypeActivator();
        }

        public virtual T GetTyped<T>(Member member) where T : MemberTypeBase
        {
            return Activator.CreateAndPopulateTypedInstance<T>(member);
        }
    }
}
