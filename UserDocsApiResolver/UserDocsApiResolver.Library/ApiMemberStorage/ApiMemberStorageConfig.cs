using System;

namespace UserDocsApiResolver.Library.ApiMemberStorage
{
    public class ApiMemberStorageConfig
    {
        // for .NET
        public String NetDocumentedMembersFile { get; set; }
        public String NetSourceAssemblyFile { get; set; }
        public String NetBaseAddress { get; set; }
        // for Java
        public String JavaDocumentedMembersFile { get; set; }
        public String JavaSourceAssemblyFile { get; set; }
        public String JavaBaseAddress { get; set; }
        public String JavaClassPath { get; set; }
        public String JavaSourceJarName { get; set; }
    }
}
