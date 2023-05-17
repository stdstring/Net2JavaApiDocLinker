using Aspose.Tests.GoldComparers;
using UserDocsApiLinker;

namespace UserDoscApiLinker
{
    /// <summary>
    /// Utilities functions for tests.
    /// </summary>
    internal static class TestUtils
    {
        public static void RunTest(string testName, ApiMemberPlatform platform)
        {
            string testTestData = BuildTestFileName(testName, "docx");
            string testGoldData = BuildGoldFileName(testName, "docx");
            string testOutData = BuildOutFileName(testName, "docx");

            ApiMemberLinker apiMemberLinker = new ApiMemberLinker(platform, ReturnUrl(), ReturnAssemblyFile(platform), ReturnXmlFile(platform), "log.txt", 3);
            apiMemberLinker.ProcessDocument(testTestData, testOutData);
            ZipFileComparer.Execute(testName, testTestData, testOutData, testGoldData, testOutData);
        }

        public static string ReturnXmlFile(ApiMemberPlatform apiMemberPlatform)
        {
            switch (apiMemberPlatform)
            {
                case ApiMemberPlatform.Net:
                    return string.Format("{0}/{1}", ReturnXmlPath(), "Aspose.Words.DotNet.xml");
                case ApiMemberPlatform.Java:
                    return string.Format("{0}/{1}", ReturnXmlPath(), "Aspose.Words.Java.xml");
                default:
                    return string.Format("{0}/{1}", ReturnXmlPath(), "Aspose.Words.DotNet.xml");
            }
        }

        public static string ReturnAssemblyFile(ApiMemberPlatform apiMemberPlatform)
        {
            switch (apiMemberPlatform)
            {
                case ApiMemberPlatform.Net:
                    return string.Format("{0}/{1}", ReturnAssemblyPath(), "Aspose.Words.dll");
                case ApiMemberPlatform.Java:
                    return string.Format("{0}/{1}", ReturnAssemblyPath(), "Aspose.Words.jdk15.jar");
                default:
                    return string.Format("{0}/{1}", ReturnAssemblyPath(), "Aspose.Words.dll");
            }
        }

        public static string BuildOutFileName(string testName, string extension)
        {
            return BuildFileName(testName, "Out", extension);
        }

        public static string BuildTestFileName(string testName, string extension)
        {
            return BuildFileName(testName, "Test", extension);
        }
        
        public static string BuildGoldFileName(string testName, string extension)
        {
            return BuildFileName(testName, "Gold", extension);
        }
        
        internal static string BuildFileName(string testName, string prefix, string extension)
        {
            return string.Format("X:\\Aspose\\www.Aspose.Shared\\Docs\\UserDocsApiLinker\\TestData\\{0} {1}.{2}", testName, prefix, extension);
        }
        
        internal static string ReturnXmlPath()
        {
            return @"X:\Aspose\Aspose.Words\UserDocs\Assembly\";
        }

        internal static string ReturnAssemblyPath()
        {
            return @"X:\Aspose\Aspose.Words\UserDocs\Assembly\";
        }

        internal static string ReturnUrl()
        {
            return @"http://www.aspose.com/documentation/.net-components/aspose.words-for-.net-and-java/";
        }
    }
}
