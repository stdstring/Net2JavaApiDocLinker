using System;
using System.Collections.Generic;

namespace UserDocsApiLinker
{
    /// <summary>
    /// Implements processing for Java documentation.
    /// </summary>
    public class ApiMemberCollectionJava
    {
        public List<ApiMember> ApiMembers { get; set; }

        public ApiMemberCollectionJava()
        {
            ApiMembers = new List<ApiMember>();
        }

        /// <summary>
        /// Replaces an API member from document with API member found in xml.
        /// </summary>
        public ApiMember ReplaceApiMember(string member)
        {
            return ApiMemberUtils.ReplaceApiMember(member, ApiMembers);
        }

        /// <summary>
        /// Adds a new API member with specified type.
        /// </summary>
        public void AddApiMember(string apiMember)
        {
            string memberType = apiMember.Substring(0, 1);
            switch (memberType)
            {
                case "T":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Type, null);
                    break;
                case "M":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Method, null);
                    break;
                case "P":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Property, null);
                    break;
                case "F":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Field, null);
                    break;

                default:
                    throw new ArgumentException("There is incorrect input string.");
            }
        }

        /// <summary>
        /// Adds a new API member with specified type.
        /// </summary>
        public void AddApiMember(string apiMember, bool? isReadOnly)
        {
            string memberType = apiMember.Substring(0, 1);
            switch (memberType)
            {
                case "T":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Type, isReadOnly);
                    break;
                case "M":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Method, isReadOnly);
                    break;
                case "P":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Property, isReadOnly);
                    break;
                case "F":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Field, isReadOnly);
                    break;
                default:
                    throw new ArgumentException("There is incorrect input string.");
            }
        }

        /// <summary>
        /// Adds a new API member with specified type.
        /// </summary>
        /// <param name="apiMember">String like Aspose.Words.License(System.Int32, System.Int32)</param>
        /// <param name="apiMemberType">Type of member (Method, Event, etc..</param>
        /// <param name="isReadOnly">Only for Properties!</param>
        private void AddApiMember(string apiMember, ApiMemberType apiMemberType, bool? isReadOnly)
        {

            string fullSyntax = apiMember;
            ApiMember am = new ApiMember();

            // Aspose.Words.License(System.Int32, System.Int32)  - have parameters.
            if (apiMember.Contains("("))
            {
                int idx = apiMember.IndexOf('(');
                fullSyntax = apiMember.Substring(0, idx);
                string parameters = apiMember.Substring(idx + 1, apiMember.Length - idx - 2);

                if (!string.IsNullOrEmpty(parameters))
                    foreach (string amp in parameters.Split(','))
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
                        int dotpos = fullSyntax.LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;

                case ApiMemberType.Method:
                    if (!fullSyntax.Contains("#ctor"))
                    {
                        //R2.2.2
                        //M:Aspose.Words.License
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split('.');
                        arRange[1] = arRange[1].Substring(0, 1).ToLower() + arRange[1].Substring(1);
                        am.FriendlyName = string.Join(".", arRange);
                    }
                    else
                    {
                        //R2.2.3
                        //M:Aspose.Words.License.#ctor
                        int dotpos = fullSyntax.LastIndexOf('.');
                        string range = fullSyntax.Substring(0, dotpos);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = range.Substring(dotpos + 1);
                    }
                    break;

                case ApiMemberType.Property:
                    {
                        //Need to determine how to understand that the property is read only!
                        bool isReadOnlyProperty = isReadOnly.HasValue && isReadOnly.Value;

                        //R2.2.4.x

                        //P:Aspose.Words.BookmarkStart.Bookmark
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split('.');

                        //R2.2.4.4
                        if (ApiMemberUtils.IsContainSpecialPrefix(arRange[1]))
                            arRange[1] = arRange[1].Substring(0, 1).ToLower() + arRange[1].Substring(1);
                        else if (arRange[1].Contains("Item"))
                            if (am.Parameters.Count > 0)
                            {
                                //R2.2.4.3
                                if (am.Parameters[0].Key == "System.Int32")
                                    arRange[1] = isReadOnlyProperty ? "get" : "get/set";
                                //R2.2.4.5
                                else
                                {
                                    string[] arParam = am.Parameters[0].Key.Split('.');
                                    arRange[1] = isReadOnlyProperty ? string.Format("getBy{0}", arParam[arParam.Length - 1]) : string.Format("getBy{0}/setBy{0}", arParam[arParam.Length - 1]);
                                }
                            }
                            else
                                arRange[1] = isReadOnlyProperty ? "get" : "get/set";
                        //R2.2.4.1 - R2.2.4.2
                        else
                            arRange[1] = isReadOnlyProperty ? string.Format("get{0}", arRange[1]) : string.Format("get{0}/set{0}", arRange[1]);
                        am.FriendlyName = string.Join(".", arRange);
                    }
                    break;

                case ApiMemberType.Field:
                    {
                        //R2.2.5
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        string[] arRange = fullSyntax.Substring(dotpos + 1).Split('.');
                        arRange[1] = ApiMemberUtils.IsOnlyLetters(arRange[1]) 
                            ? ApiMemberUtils.CapitalSplit(arRange[1]).ToUpper() 
                            : ApiMemberUtils.CapitalNumberSplit(arRange[1]).ToUpper();
                        am.FriendlyName = string.Join(".", arRange);
                    }
                    break;
            }
            ApiMembers.Add(am);
        }
    }
}
