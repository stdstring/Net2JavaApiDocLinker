using System;
using System.Configuration;
using System.IO;
using UserDocsApiResolver.Library.ApiMemberStorage;

namespace UserDocsApiResolver
{
    public class ApiMemberStorageConfigReader
    {
        public ApiMemberStorageConfig Read()
        {
            return new ApiMemberStorageConfig
                       {
                           NetDocumentedMembersFile = ConfigurationManager.AppSettings[NetDocumentedMembersFile],
                           NetSourceAssemblyFile = ConfigurationManager.AppSettings[NetSourceAssemblyFile],
                           NetBaseAddress = ConfigurationManager.AppSettings[NetBaseAddress],
                           JavaDocumentedMembersFile = ConfigurationManager.AppSettings[JavaDocumentedMembersFile],
                           JavaSourceAssemblyFile = ConfigurationManager.AppSettings[JavaSourceAssemblyFile],
                           JavaBaseAddress = ConfigurationManager.AppSettings[JavaBaseAddress],
                           JavaClassPath = ConfigurationManager.AppSettings[JavaAsposePath] +
                                           JavaClassPathDelimiter +
                                           ConfigurationManager.AppSettings[JavaReflectionAppPath],
                           JavaSourceJarName = Path.GetFileName(ConfigurationManager.AppSettings[JavaAsposePath])
                       };
        }

        // for NET
        private const String NetDocumentedMembersFile = "NetDocumentedMembersFile";
        private const String NetSourceAssemblyFile = "NetSourceAssemblyFile";
        private const String NetBaseAddress = "NetBaseAddress";
        // for Java
        private const String JavaDocumentedMembersFile = "JavaDocumentedMembersFile";
        private const String JavaSourceAssemblyFile = "JavaSourceAssemblyFile";
        private const String JavaBaseAddress = "JavaBaseAddress";
        private const String JavaAsposePath = "JavaAsposePath";
        private const String JavaReflectionAppPath = "JavaReflectionAppPath";
        private const String JavaClassPathDelimiter = ";";
    }
}
