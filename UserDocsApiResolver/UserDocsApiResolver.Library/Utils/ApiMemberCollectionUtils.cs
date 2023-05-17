using System.Collections.Generic;

namespace UserDocsApiResolver.Library.Utils
{
    internal static class ApiMemberCollectionUtils
    {
        public static void CopyApiMembersFromBaseClass(List<ApiMember> apiMembers)
        {
            List<ApiMember> addList = new List<ApiMember>();
            List<ApiMember> cloneList = new List<ApiMember>();
            CopyList(cloneList, apiMembers);

            foreach (ApiMember apiMember in apiMembers)
            {
                IList<ApiMember> retList = CopyApiMembersFromBaseClass(cloneList, apiMember);
                CopyList(cloneList, retList);
                CopyList(addList, retList);
            }
            CopyList(apiMembers, addList);
        }

        private static IList<ApiMember> CopyApiMembersFromBaseClass(IList<ApiMember> apiMembers, ApiMember apiMember)
        {
            List<ApiMember> addList = new List<ApiMember>();
            List<ApiMember> cloneList = new List<ApiMember>();
            if ((apiMember.Type == ApiMemberType.Type) &&
                (!string.IsNullOrEmpty(apiMember.BaseType)) &&
                (ApiMemberUtils.IsAsposeClass(apiMember.BaseType)))
            {
                ApiMember baseClass = null;
                foreach (ApiMember member in apiMembers)
                {
                    if ((member.Key.Equals(apiMember.BaseType)) && (member.Type == ApiMemberType.Type))
                    {
                        baseClass = member;
                        break;
                    }
                }
                CopyList(cloneList, apiMembers);
                if ((baseClass != null) &&
                    (!string.IsNullOrEmpty(baseClass.BaseType)) &&
                    (ApiMemberUtils.IsAsposeClass(baseClass.BaseType)))
                {
                    IList<ApiMember> retList = CopyApiMembersFromBaseClass(cloneList, baseClass);
                    CopyList(addList, retList);
                    CopyList(cloneList, retList);
                }
                if (baseClass != null)
                {
                    IList<ApiMember> retList = CopyApiMembersFromBaseClass(cloneList, baseClass, apiMember);
                    CopyList(addList, retList);
                    CopyList(cloneList, retList);
                }
            }
            return addList;
        }

        private static IList<ApiMember> CopyApiMembersFromBaseClass(List<ApiMember> apiMembers, ApiMember srcClass, ApiMember dstClass)
        {
            IList<ApiMember> addList = new List<ApiMember>();
            foreach (ApiMember member in apiMembers)
            {
                if (ApiMemberUtils.IsMemberBelongToClass(member, srcClass))
                {
                    ApiMember modMember = member.Clone();
                    ChangeBaseClassNameByParent(dstClass, modMember);
                    if (apiMembers.Find(m => ApiMemberUtils.IsEquilMembers(m, modMember)) == null)
                        addList.Add(modMember);
                }
            }
            return addList;
        }

        private static void ChangeBaseClassNameByParent(ApiMember parentMember, ApiMember changedMember)
        {
            changedMember.Key = ReplaceClassName(parentMember.Key, changedMember.Key);
            changedMember.FriendlyName = ReplaceClassName(parentMember.FriendlyName, changedMember.FriendlyName);

        }

        private static string ReplaceClassName(string parentKey, string changedKey)
        {
            string[] parentArr = parentKey.Split(MemberPartDef.ClassNameDelimiterChar);
            string[] changedArr = changedKey.Split(MemberPartDef.ClassNameDelimiterChar);
            if (parentArr.Length >= changedArr.Length)
                return parentKey;
            for (int i = 0; i < parentArr.Length; i++)
                changedArr[i] = parentArr[i];
            return string.Join(MemberPartDef.ClassNameDelimiter, changedArr);
        }

        private static void CopyList(List<ApiMember> mainList, IEnumerable<ApiMember> addList)
        {
            foreach (ApiMember addMember in addList)
            {
                if (mainList.Find(member => ApiMemberUtils.IsEquilMembers(member, addMember)) == null)
                    mainList.Add(addMember);
            }
        }
    }
}
