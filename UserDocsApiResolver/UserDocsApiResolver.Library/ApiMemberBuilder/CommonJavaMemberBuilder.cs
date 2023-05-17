using System;
using UserDocsApiResolver.Library.Utils;

namespace UserDocsApiResolver.Library.ApiMemberBuilder
{
    internal class CommonJavaMemberBuilder
    {
        public ApiMember BuildMember(String apiMember)
        {
            string memberType = apiMember.Substring(0, 1);
            switch (memberType)
            {
                case Type:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Type, null);
                case Method:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Method, null);
                case Property:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Property, null);
                case Field:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Field, null);
                default:
                    throw new ArgumentException("There is incorrect input string.");
            }
        }

        public ApiMember BuildMember(String apiMember, Boolean? isReadOnly)
        {
            string memberType = apiMember.Substring(0, 1);
            switch (memberType)
            {
                case Type:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Type, isReadOnly);
                case Method:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Method, isReadOnly);
                case Property:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Property, isReadOnly);
                case Field:
                    return BuildMember(apiMember.Substring(2), ApiMemberType.Field, isReadOnly);
                default:
                    throw new ArgumentException("There is incorrect input string.");
            }
        }

        private ApiMember BuildMember(String apiMember, ApiMemberType apiMemberType, Boolean? isReadOnly)
        {
            string fullSyntax = apiMember;
            ApiMember am = new ApiMember();
            // Aspose.Words.License(System.Int32, System.Int32) - have parameters.
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
                        //R2.2.1
                        //T:Aspose.Words.Reporting.MergeFieldEventArgs
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Method:
                    if (!fullSyntax.Contains(CommonDefs.Ctor))
                    {
                        //R2.2.2
                        //M:Aspose.Words.License
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split(MemberPartDef.ClassNameDelimiterChar);
                        arRange[1] = arRange[1].Substring(0, 1).ToLower() + arRange[1].Substring(1);
                        am.FriendlyName = string.Join(MemberPartDef.ClassNameDelimiter, arRange);
                    }
                    else
                    {
                        //R2.2.3
                        //M:Aspose.Words.License.#ctor
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        string range = fullSyntax.Substring(0, dotpos);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        am.FriendlyName = range.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Property:
                    {
                        //Need to determine how to understand that the property is read only!
                        bool isReadOnlyProperty = isReadOnly.HasValue && isReadOnly.Value;
                        //R2.2.4.x
                        //P:Aspose.Words.BookmarkStart.Bookmark
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split(MemberPartDef.ClassNameDelimiterChar);
                        //R2.2.4.4
                        if (ApiMemberUtils.IsContainSpecialPrefix(arRange[1]))
                            arRange[1] = arRange[1].Substring(0, 1).ToLower() + arRange[1].Substring(1);
                        else if (arRange[1].Contains(ItemName))
                            if (am.Parameters.Count > 0)
                            {
                                //R2.2.4.3
                                if (am.Parameters[0].Key == typeof(Int32).FullName)
                                    arRange[1] = isReadOnlyProperty ? ReadonlyItemProperty : ItemProperty;
                                //R2.2.4.5
                                else
                                {
                                    string[] arParam = am.Parameters[0].Key.Split(MemberPartDef.ClassNameDelimiterChar);
                                    arRange[1] = isReadOnlyProperty
                                                     ? string.Format(ReadonlyItemByPropertyTemplate, arParam[arParam.Length - 1])
                                                     : string.Format(ItemByPropertyTemplate, arParam[arParam.Length - 1]);
                                }
                            }
                            else
                                arRange[1] = isReadOnlyProperty ? ReadonlyItemProperty : ItemProperty;
                        //R2.2.4.1 - R2.2.4.2
                        else
                            arRange[1] = isReadOnlyProperty
                                             ? string.Format(ReadonlyPropertyTemplate, arRange[1])
                                             : string.Format(PropertyTemplate, arRange[1]);
                        am.FriendlyName = string.Join(MemberPartDef.ClassNameDelimiter, arRange);
                    }
                    break;
                case ApiMemberType.Field:
                    {
                        //R2.2.5
                        int dotpos = fullSyntax.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf(MemberPartDef.ClassNameDelimiter);
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split(MemberPartDef.ClassNameDelimiterChar);
                        arRange[1] = ApiMemberUtils.IsOnlyLetters(arRange[1])
                            ? ApiMemberUtils.CapitalSplit(arRange[1]).ToUpper()
                            : ApiMemberUtils.CapitalNumberSplit(arRange[1]).ToUpper();
                        am.FriendlyName = string.Join(MemberPartDef.ClassNameDelimiter, arRange);
                    }
                    break;
            }
            return am;
        }

        private const String ItemName = "Item";
        private const String ReadonlyItemProperty = "get";
        private const String ItemProperty = "get/set";
        private const String ReadonlyPropertyTemplate = "get{0}";
        private const String PropertyTemplate = "get{0}/set{0}";
        private const String ReadonlyItemByPropertyTemplate = "getBy{0}";
        private const String ItemByPropertyTemplate = "getBy{0}/setBy{0}";

        private const String Type = "T";
        private const String Method = "M";
        private const String Property = "P";
        private const String Field = "F";
    }
}
