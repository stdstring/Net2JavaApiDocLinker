using System;

namespace UserDocsApiResolver.Library.ApiMemberStorage
{
    public interface IApiMemberStorage
    {
        ApiMember Search(String member);
    }
}
