using System;
using System.Collections.Generic;
using System.Xml;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberBuilder
{
    internal class ApiMemberDocProcessor
    {
        public ApiMemberDocProcessor(String documentedMembersFile, Func<String, ApiMember> apiMemberBuilder)
        {
            if (String.IsNullOrEmpty(documentedMembersFile))
                throw new ArgumentException("documentedMembersFile");
            if (apiMemberBuilder == null)
                throw new ArgumentNullException("apiMemberBuilder");
            _documentedMembersFile = documentedMembersFile;
            _apiMemberBuilder = apiMemberBuilder;
        }

        public List<ApiMember> Build()
        {
            Logger.Debug("Build() enter");
            List<ApiMember> commonApiMembers = new List<ApiMember>();
            XmlTextReader textReader = new XmlTextReader(_documentedMembersFile);
            while (textReader.Read())
            {
                if ((textReader.LocalName.Equals(MemberNodeName)) && (textReader.AttributeCount > 0))
                {
                    try
                    {
                        ApiMember member = _apiMemberBuilder(textReader.GetAttribute(MemberAttrName));
                        Logger.DebugFormat("Build {0}", ApiMemberUtils.GetString(member));
                        commonApiMembers.Add(member);
                    }
                    catch (Exception ex)
                    {
                        Logger.WarnFormat("Exception in Build(): {0}", ex);
                        continue;
                    }
                }
            }
            Logger.Debug("Build() exit");
            return commonApiMembers;
        }

        private readonly String _documentedMembersFile;
        private readonly Func<String, ApiMember> _apiMemberBuilder;

        private static readonly ILog Logger = LogManager.GetLogger("ApiMemberDocProcessor");

        private const String MemberNodeName = "member";
        private const String MemberAttrName = "name";
    }
}
