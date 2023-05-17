using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberStorage
{
    internal class NetApiMemberStorage : IApiMemberStorage
    {
        public NetApiMemberStorage(List<ApiMember> commonMembers)
        {
            CommonMembers = commonMembers;
        }

        public ApiMember Search(String member)
        {
            Logger.DebugFormat("Search({0}) enter", member);
            ApiMember result = ApiMemberUtils.FindApiMember(member, CommonMembers);
            Logger.DebugFormat("Search({0}) exit", member);
            return result;
        }

        public List<ApiMember> CommonMembers { get; private set; }

        private static readonly ILog Logger = LogManager.GetLogger("NetApiMemberStorage");
    }
}
