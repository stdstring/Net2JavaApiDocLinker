using System;
using UserDocsApiResolver.Library;
using UserDocsApiResolver.Library.ApiMemberStorage;

namespace UserDocsApiResolver
{
    internal class ApiMemberRequestProcessor
    {
        public ApiMemberRequestProcessor(ApiMemberStorageManager apiMemberStorageManager)
        {
            if (apiMemberStorageManager == null)
                throw new ArgumentNullException("apiMemberStorageManager");
            _apiMemberStorageManager = apiMemberStorageManager;
        }

        public String Process(String member, ApiMemberPlatform platform)
        {
            ApiMember result = _apiMemberStorageManager.Search(member, platform);
            return result == null
                       ? String.Format(UnrecognizedResponseTemplate, member)
                       : String.Format(RecognizedResponseTemplate, member, result.Url);
        }

        private readonly ApiMemberStorageManager _apiMemberStorageManager;

        private const String RecognizedResponseTemplate = "[{0}|{1}]";
        private const String UnrecognizedResponseTemplate = "*{0}*";
    }
}
