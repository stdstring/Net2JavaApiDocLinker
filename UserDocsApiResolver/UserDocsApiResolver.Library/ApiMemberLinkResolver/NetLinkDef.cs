using System;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal static class NetLinkDef
    {
        public const String HLinkTemplate = "{0}{1}.html";
        public const String OverloadPropertyHLinkTemplate = "{0}{1}{2}.html";
        public const String OverloadCtorHLinkTemplate = "{0}{1}constructor{2}.html";
        public const String OverloadMethodHLinkTemplate = "{0}{1}_overload_{2}.html";
        public const String MethodSummaryHLinkTemplate = "{0}{1}_overloads.html";
        public const String CtorSummaryHLinkTemplate = "{0}{1}constructor.html";
    }
}
