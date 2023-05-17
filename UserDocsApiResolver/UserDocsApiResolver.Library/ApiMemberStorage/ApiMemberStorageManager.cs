using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.ApiMemberBuilder;
using UserDocsApiResolver.Library.ApiMemberLinkResolver;
using UserDocsApiResolver.Library.JavaMetadata;
using UserDocsApiResolver.Library.JavaReflection;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberStorage
{
    public class ApiMemberStorageManager
    {
        public ApiMember Search(String member, ApiMemberPlatform platform)
        {
            Logger.DebugFormat("Search({0}, {1}) enter", member, platform);
            ApiMember apiMember = _storageMap[platform].Search(member);
            Logger.DebugFormat("Search({0}, {1}) exit", member, platform);
            return apiMember;
        }

        public static ApiMemberStorageManager Create(ApiMemberStorageConfig config)
        {
            Logger.Debug("Create() enter");
            if (config == null)
                throw new ArgumentNullException("config");
            IDictionary<ApiMemberPlatform, IApiMemberStorage> storageMap = new Dictionary<ApiMemberPlatform, IApiMemberStorage>();
            // .NET collection
            NetApiMemberStorage netStorage = CreateNetStorage(config.NetDocumentedMembersFile,
                                                              config.NetSourceAssemblyFile,
                                                              config.NetBaseAddress);
            storageMap.Add(ApiMemberPlatform.Net, netStorage);
            // Java collection
            JavaApiMemberStorage javaStorage = CreateJavaStorage(config.JavaDocumentedMembersFile,
                                                                 config.JavaSourceAssemblyFile,
                                                                 config.JavaClassPath,
                                                                 config.JavaSourceJarName,
                                                                 config.JavaBaseAddress,
                                                                 netStorage.CommonMembers);
            storageMap.Add(ApiMemberPlatform.Java, javaStorage);
            Logger.Debug("Create() exit");
            return new ApiMemberStorageManager(storageMap);
        }

        private static NetApiMemberStorage CreateNetStorage(String documentedMembersFile,
                                                            String sourceAssemblyFile,
                                                            String netBaseAddress)
        {
            CommonNetMemberBuilder netMemberBuilder = new CommonNetMemberBuilder();
            ApiMemberDocProcessor netMemberDocProcessor = new ApiMemberDocProcessor(documentedMembersFile,
                                                                                    netMemberBuilder.BuildMember);
            ILinkResolver netLinkResolver = new NetLinkResolver(netBaseAddress);
            CommonCollectionBuilder netCollectionBuilder = new CommonCollectionBuilder(netMemberDocProcessor,
                                                                                       sourceAssemblyFile,
                                                                                       netLinkResolver);
            List<ApiMember> netMembers = netCollectionBuilder.Build();
            return new NetApiMemberStorage(netMembers);
        }

        private static JavaApiMemberStorage CreateJavaStorage(String documentedMembersFile,
                                                              String sourceAssemblyFile,
                                                              String javaClassPath,
                                                              String sourceJarName,
                                                              String javaBaseAddress,
                                                              List<ApiMember> netMembers)
        {
            CommonJavaMemberBuilder javaMemberBuilder = new CommonJavaMemberBuilder();
            ApiMemberDocProcessor javaMemberDocProcessor = new ApiMemberDocProcessor(documentedMembersFile,
                                                                                     javaMemberBuilder.BuildMember);
            JavaMetadataStorage javaMetadataStorage;
            using (JavaReflectionManager javaReflectionManager = new JavaReflectionManager(() => new JavaReflectionAdapter(javaClassPath)))
            {
                JavaMetadataStorageBuilder javaMetadataBuilder = new JavaMetadataStorageBuilder();
                javaMetadataStorage = javaMetadataBuilder.Build(javaReflectionManager, sourceJarName);
            }
            ILinkResolver javaLinkResolver = new JavaLinkResolver(javaBaseAddress, javaMetadataStorage);
            CommonCollectionBuilder javaCollectionBuilder = new CommonCollectionBuilder(javaMemberDocProcessor,
                                                                                        sourceAssemblyFile,
                                                                                        javaLinkResolver);
            List<ApiMember> javaCommonMembers = javaCollectionBuilder.Build();
            List<ApiMember> javaSpecificMembers = new JavaSpecificCollectionBuilder(javaBaseAddress)
                .Build(javaMetadataStorage, netMembers);
            return new JavaApiMemberStorage(javaCommonMembers, javaSpecificMembers);
        }

        private ApiMemberStorageManager(IDictionary<ApiMemberPlatform, IApiMemberStorage> storageMap)
        {
            _storageMap = storageMap;
        }

        private readonly IDictionary<ApiMemberPlatform, IApiMemberStorage> _storageMap;

        private static readonly ILog Logger = LogManager.GetLogger("ApiMemberStorageManager");
    }
}
