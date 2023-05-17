using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberStorage
{
    internal class JavaApiMemberStorage : IApiMemberStorage
    {
        public JavaApiMemberStorage(List<ApiMember> commonMembers, List<ApiMember> specificMembers)
        {
            CommonMembers = commonMembers;
            SpecificMembers = specificMembers;
        }

        public ApiMember Search(String member)
        {
            Logger.DebugFormat("Search({0}) enter", member);
            ApiMember result = ApiMemberUtils.FindApiMember(member, CommonMembers) ??
                               ApiMemberUtils.FindApiMember(member, SpecificMembers);
            Logger.DebugFormat("Search({0}) exit", member);
            return result;
        }

        public List<ApiMember> CommonMembers { get; private set; }
        public List<ApiMember> SpecificMembers { get; private set; }

        private static readonly ILog Logger = LogManager.GetLogger("JavaApiMemberStorage");
    }
}
