using System;
using UserDocsApiResolver.Library.Utils;

namespace UserDocsApiResolver.Library.ApiMemberBuilder
{
    internal class CommonNetMemberBuilder
    {
        public ApiMember BuildMember(String apiMember)
        {
            string memberType = apiMember.Substring(0, 1);
            switch (memberType)
            {
                case Type:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Type);
                case Method:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Method);
                case Property:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Property);
                case Field:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Field);
                case Event:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Event);
                default:
                    throw new ArgumentException("There is incorrect input string.");
            }
        }

        private ApiMember BuildMember(String apiMember, ApiMemberType apiMemberType)
        {
            String fullSyntax = apiMember;
            ApiMember am = new ApiMember();
            // M:Aspose.Words.License(System.Int32, System.Int32)  - have parameters.
            if (apiMember.Contains(MemberPartDef.ParameterBlockStart))
            {
                int idx = apiMember.IndexOf(MemberPartDef.ParameterBlockStart);
                fullSyntax = apiMember.Substring(0, idx);
                string parameters = apiMember.Substring(idx + 1, apiMember.Length - idx - 2);

                if (!string.IsNullOrEmpty(parameters))
                    foreach (string amp in parameters.Split(MemberPartDef.ParametersDelimiterChar))
                        am.Parameters.Add(new ApiMemberParameter(amp));
            }
            am.Key = fullSyntax;
            am.Type = apiMemberType;
            switch (apiMemberType)
            {
                case ApiMemberType.Type:
                    {
                        //R2.1.1
                        //T:Aspose.Words.Reporting.MergeFieldEventArgs
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Method:
                    if (!fullSyntax.Contains(CommonDefs.Ctor))
                    {
                        //R2.1.2
                        //M:Aspose.Words.License
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    else
                    {
                        //R2.1.3
                        //M:Aspose.Words.License.#ctor
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        string range = fullSyntax.Substring(0, dotpos);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = range.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Property:
                    {
                        //R2.1.4
                        //P:Aspose.Words.BookmarkStart.Bookmark
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Field:
                    {
                        //R2.1.5
                        //F:Aspose.Words.ImportFormatMode.UseDestinationStyles
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Event:
                    {
                        //R2.1.6
                        //E:Aspose.Words.Viewer.DocumentRenderer.EndDocument
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
            }
            return am;
        }

        private const String Type = "T";
        private const String Method = "M";
        private const String Property = "P";
        private const String Field = "F";
        private const String Event = "E";
    }
}
