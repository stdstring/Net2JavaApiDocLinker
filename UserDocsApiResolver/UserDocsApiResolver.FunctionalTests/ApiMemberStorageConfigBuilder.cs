using UserDocsApiResolver.Library.ApiMemberStorage;

namespace UserDocsApiResolver.FunctionalTests
{
    internal class ApiMemberStorageConfigBuilder
    {
        public ApiMemberStorageConfig Build()
        {
            return new ApiMemberStorageConfig
            {
                NetDocumentedMembersFile = "../../../external/Aspose.NET/Aspose.Words.xml",
                NetSourceAssemblyFile = "../../../external/Aspose.NET/Aspose.Words.dll",
                NetBaseAddress = "http://aspose.words/net/",
                JavaDocumentedMembersFile = "../../../external/Aspose.Java/Aspose.Words.xml",
                JavaSourceAssemblyFile = "../../../external/Aspose.NET/Aspose.Words.dll",
                JavaBaseAddress = "http://aspose.words/java/",
                JavaClassPath = "../../../external/Aspose.Java/Aspose.Words.jdk15.jar;../../../external/JavaReflectionApp/JavaReflectionApp.jar",
                JavaSourceJarName = "Aspose.Words.jdk15.jar"
            };
        }
    }
}
