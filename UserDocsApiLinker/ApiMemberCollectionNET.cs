using System;
using System.Collections.Generic;


namespace UserDocsApiLinker
{
    /// <summary>
    /// Class implements processing for .Net documentation.
    /// </summary>
    public class ApiMemberCollectionNet
    {
        public IList<ApiMember> ApiMembers { get; set; }

        public ApiMemberCollectionNet()
        {
            ApiMembers = new List<ApiMember>();
        }

        /// <summary>
        /// Replace an API member from document with finding API member from xml.
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
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Type);
                    break;
                case "M":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Method);
                    break;
                case "P":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Property);
                    break;
                case "F":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Field);
                    break;
                case "E":
                    AddApiMember(apiMember.Substring(2), ApiMemberType.Event);
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
        private void AddApiMember(string apiMember, ApiMemberType apiMemberType)
        {
            string fullSyntax = apiMember;
            ApiMember am = new ApiMember();

            // M:Aspose.Words.License(System.Int32, System.Int32)  - have parameters.
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
                        //R2.1.1
                        //T:Aspose.Words.Reporting.MergeFieldEventArgs
                        int dotpos = fullSyntax.LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
                case ApiMemberType.Method:
                    if (!fullSyntax.Contains("#ctor"))
                    {
                        //R2.1.2
                        //M:Aspose.Words.License
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    else
                    {
                        //R2.1.3
                        //M:Aspose.Words.License.#ctor
                        int dotpos = fullSyntax.LastIndexOf('.');
                        string range = fullSyntax.Substring(0, dotpos);
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = range.Substring(dotpos + 1);
                    }
                    break;

                case ApiMemberType.Property:
                    {
                        //R2.1.4
                        //P:Aspose.Words.BookmarkStart.Bookmark
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;

                case ApiMemberType.Field:
                    {
                        //R2.1.5
                        //F:Aspose.Words.ImportFormatMode.UseDestinationStyles
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;

                case ApiMemberType.Event:
                    {
                        //R2.1.6
                        //E:Aspose.Words.Viewer.DocumentRenderer.EndDocument
                        int dotpos = fullSyntax.LastIndexOf('.');
                        dotpos = fullSyntax.Substring(0, dotpos).LastIndexOf('.');
                        am.FriendlyName = fullSyntax.Substring(dotpos + 1);
                    }
                    break;
            }

            ApiMembers.Add(am);
        }
    }
}
