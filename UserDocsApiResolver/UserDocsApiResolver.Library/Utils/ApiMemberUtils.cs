using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UserDocsApiResolver.Library.Utils
{
    internal static class ApiMemberUtils
    {
        public static String GetString(ApiMember member)
        {
            IList<String> parameters = member.Parameters.ConvertAll(parameter => parameter.Key);
            String parameterList = MemberPartDef.BuildParamList(parameters, member.Type == ApiMemberType.Method);
            return String.Format("ApiMember: Type = {0}, Key = {1}{2}", member.Type, member.Key, parameterList);
        }

        public static String GetKey(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
                return ((Type)memberInfo).FullName;
            if (memberInfo is ConstructorInfo)
                return String.Format(CompKeyTemplate, memberInfo.ReflectedType.FullName, CommonDefs.Ctor);
            return String.Format(CompKeyTemplate, memberInfo.ReflectedType.FullName, memberInfo.Name);
        }

        public static String GetContainedType(ApiMember member)
        {
            if (member.Type == ApiMemberType.Type || member.Type == ApiMemberType.Unknown)
                return member.Key;
            Int32 lastDotIndex = member.Key.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            return lastDotIndex == -1 ? member.Key : member.Key.Substring(0, lastDotIndex);
        }

        public static String GetMemberName(ApiMember member)
        {
            if (member.Type == ApiMemberType.Type || member.Type == ApiMemberType.Unknown)
                return String.Empty;
            return MemberPartDef.GetLastNamePart(member.Key);
        }

        /// <summary>
        /// Finds member matches between document and xml.
        /// </summary>
        /// <param name="member">String member. Like Aspose.Words.License(System.Int32, System.Int32).</param>
        /// <param name="apiMembers">Collection of ApiMembers</param>
        /// <returns>Returns same member if does not match with the collection of members, or return found member.</returns>
        public static ApiMember ReplaceApiMember(string member, IList<ApiMember> apiMembers)
        {
            ApiMember am = FindApiMember(member, apiMembers);
            if (am == null)
                return new ApiMember
                            {
                                FriendlyName = member,
                                Type = ApiMemberType.Unknown
                            };
            return am;
        }

        /// <summary>
        /// Implements finding diference between two API members.
        /// </summary>
        /// <param name="friendlyMember">Member from xml file</param>
        /// <param name="docMember">Member from document</param>
        /// <param name="count">Count of the same members (only name, without comparing parameters)</param>
        public static bool CompareMembers(ApiMember friendlyMember, ApiMember docMember, int count)
        {
            if (!CompareDotJoingString(friendlyMember, docMember.Key)) 
                return false;
            // if we have only one member with the same name we should ignore one parameter
            if ((count == 1) && (docMember.Parameters.Count == 0))
                return true;
            if (friendlyMember.Parameters.Count != docMember.Parameters.Count) 
                return false;
            for (int i = 0; i < friendlyMember.Parameters.Count; i++)
                if (!CompareDotJoingString(friendlyMember.Parameters[i].Key, docMember.Parameters[i].Key))
                    return false;
            return true;
        }

        /// <summary>
        /// Splits the word like PageBreak into string like PAGE_BREAK.
        /// </summary>
        /// <param name="inputstr">Input string for spliting.</param>
        /// <returns>Splited word.</returns>
        public static string CapitalSplit(string inputstr)
        {
            Regex reg = new Regex(@"([a-z])[A-Z]");
            MatchCollection col = reg.Matches(inputstr);
            int iStart = 0;
            string final = string.Empty;
            foreach (Match m in col)
            {
                int i = m.Index;
                final += inputstr.Substring(iStart, i - iStart + 1) + "_";
                iStart = i + 1;
            }
            string last = inputstr.Substring(iStart, inputstr.Length - iStart);
            final += last;
            return final;
        }

        /// <summary>
        /// Check ApiMembers equialent.
        /// </summary>
        /// <param name="firstMember">First member to check.</param>
        /// <param name="secondMember">Second member to check.</param>
        /// <returns>If equil then return true, false otherwise.</returns>
        public static bool IsEquilMembers(ApiMember firstMember, ApiMember secondMember)
        {
            if (firstMember.Type != secondMember.Type)
                return false;
            if (firstMember.BaseType != secondMember.BaseType)
                return false;
            if (firstMember.Key != secondMember.Key)
                return false;
            if (firstMember.FriendlyName != secondMember.FriendlyName)
                return false;
            if (firstMember.Url != secondMember.Url)
                return false;
            if (firstMember.Parameters.Count != secondMember.Parameters.Count)
                return false;
            for(int i = 0; i< firstMember.Parameters.Count; i++)
                if(!IsEquilParamMember(firstMember.Parameters[i], secondMember.Parameters[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if the specified string contains letters only. (A4 - false, PageBreak - true.)
        /// </summary>
        public static bool IsOnlyLetters(String strToCheck)
        {
            Regex reg = new Regex(@"[^a-zA-Z]");
            return !reg.IsMatch(strToCheck);
        }

        /// <summary>
        /// Same as <see cref="CapitalSplit"/> but splits a string by number character. Page3x4 - PAGE_3_X_4
        /// </summary>
        /// <param name="inputstr">Input string for spliting.</param>
        /// <returns>Splited word.</returns>
        public static string CapitalNumberSplit(string inputstr)
        {
            Regex reg = new Regex(@"([a-z])[0-9]");
            MatchCollection col = reg.Matches(inputstr);
            int iStart = 0;
            string final = string.Empty;
            foreach (Match m in col)
            {
                int i = m.Index;
                final += CapitalNumberSplitNeg(inputstr.Substring(iStart, i - iStart + 1)) + "_";
                iStart = i + 1;
            }
            string last = inputstr.Substring(iStart, inputstr.Length - iStart);
            final += last;
            return final;
        }

        /// <summary>
        /// Checks if specified string has a special prefix.
        /// </summary>
        public static bool IsContainSpecialPrefix(string inputstr)
        {
            List<string> specialPrefixes = new List<string>
                                               {
                                                   "Is",
                                                   "Has",
                                                   "Can",
                                                   "Should",
                                                   "Could",
                                                   "Will",
                                                   "Shall",
                                                   "Contains",
                                                   "StartsWith",
                                                   "EndsWith"
                                               };
            foreach (string prefix in specialPrefixes)
                if (inputstr.Contains(prefix))
                    return true;
            return false;
        }

        /// <summary>
        /// Method test name of class to know is that class a Aspose.Words class.
        /// </summary>
        /// <param name="className">Name of class</param>
        /// <returns>If Aspose.Words class return true, false otherwise.</returns>
        public static bool IsAsposeClass(String className)
        {
            String[] arr = className.Split('.');
            if (arr.Length < 2)
                return false;
            if ((arr[0] == "Aspose") && (arr[1] == "Words"))
                return true;
            return false;
        }

        public static bool IsMemberBelongToClass(ApiMember apiMember, ApiMember apiClass)
        {
            if (apiClass.Type != ApiMemberType.Type)
                return false;
            if ((apiMember.Type == ApiMemberType.Type) || (apiMember.Type == ApiMemberType.Unknown))
                return false;
            string[] attrMember = apiMember.Key.Split('.');
            string[] classMember = apiClass.Key.Split('.');
            if (attrMember.Length <= classMember.Length) 
                return false;
            for (int i = 0; i < classMember.Length; i++)
                if (attrMember[i].ToUpper() != classMember[i].ToUpper()) return false;
            return true;
        }

        public static bool IsOverloadsWithNoEmptyParametersMember(ApiMember member, IList<ApiMember> apiMembers)
        {
            if (CountApiMember(member.Key, apiMembers) > 1)
            {
                if (member.Parameters.Count == 0)
                    return false;
                foreach (ApiMember apiMember in apiMembers)
                {
                    if (CompareDotJoingString(member.Key, apiMember.Key) && (apiMember.Parameters.Count == 0))
                        return false;
                }
                return true;
            }
            return false;
        }

        public static ApiMember FindApiMember(string member, IList<ApiMember> apiMembers)
        {
            int count = CountApiMember(member, apiMembers);
            foreach (ApiMember am in apiMembers)
                if (CompareMembers(am, member, count))
                    return am;
            return null;
        }

        public static Boolean CheckExistenceByName(string member, IList<ApiMember> apiMembers)
        {
            return CountApiMember(member, apiMembers) > 0;
        }

        private static bool IsEquilParamMember(ApiMemberParameter firstParam, ApiMemberParameter secondParam)
        {
            if (firstParam.Key != secondParam.Key)
                return false;

            return true;
        }

        private static int CountApiMember(string member, IEnumerable<ApiMember> apiMembers)
        {
            int cnt = 0;
            foreach (ApiMember apiMember in apiMembers)
            {
                if (CompareDotJoingString(apiMember, member))
                {
                    cnt++;
                }
            }
            return cnt;
        }

        private static bool CompareMembers(ApiMember friendlyMember, string docMember, int count)
        {
            string fullSyntax = docMember.Trim();
            ApiMember am = new ApiMember();
            // Aspose.Words.License(System.Int32, System.Int32)  - have parameters.
            if (docMember.Contains("("))
            {
                int idx = docMember.IndexOf('(');
                fullSyntax = docMember.Substring(0, idx).Trim();
                string parameters = docMember.Substring(idx + 1, docMember.Length - idx - 2);

                if (!string.IsNullOrEmpty(parameters))
                    foreach (string amp in parameters.Split(','))
                        am.Parameters.Add(new ApiMemberParameter(amp.Trim()));
            }
            am.Key = fullSyntax;
            am.Type = ApiMemberType.Unknown;
            am.FriendlyName = string.Empty;
            am.BaseType = string.Empty;

            return CompareMembers(friendlyMember, am, count);
        }

        private static bool CompareDotJoingString(ApiMember friendlyMember, string docMember)
        {
            string[] fMember = friendlyMember.Key.Split('.');
            string[] dMember = docMember.Split('.');
            int dMemberLength = dMember.Length;
            int fMemberLength = fMember.Length;
            if (dMemberLength == 1 && friendlyMember.Type != ApiMemberType.Type)
                return false;
            if (dMemberLength > fMemberLength) 
                return false;
            for (int i = 0; i < dMemberLength; i++)
                if (dMember[dMemberLength - i - 1].ToUpper() != fMember[fMemberLength - i - 1].ToUpper())
                    return false;
            return true;
        }

        private static bool CompareDotJoingString(string friendlyMember, string docMember)
        {
            string[] fMember = friendlyMember.Split('.');
            string[] dMember = docMember.Split('.');
            int dMemberLength = dMember.Length;
            int fMemberLength = fMember.Length;
            if (dMemberLength > fMemberLength) return false;
            for (int i = 0; i < dMemberLength; i++)
                if (dMember[dMemberLength - i - 1].ToUpper() != fMember[fMemberLength - i - 1].ToUpper())
                    return false;
            return true;
        }

        private static string CapitalNumberSplitNeg(string inputstr)
        {
            Regex reg = new Regex(@"([0-9])[a-z]");
            MatchCollection col = reg.Matches(inputstr);
            int iStart = 0;
            string final = string.Empty;
            foreach (Match m in col)
            {
                int i = m.Index;
                final += inputstr.Substring(iStart, i - iStart + 1) + "_";
                iStart = i + 1;
            }
            string last = inputstr.Substring(iStart, inputstr.Length - iStart);
            final += last;
            return final;
        }

        private const String CompKeyTemplate = "{0}.{1}";
    }
}
