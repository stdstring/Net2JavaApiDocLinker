using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Drawing;
using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using UserDocsApiLinker.ApiMemberLinkResolver;
using UserDocsApiLinker.JavaMetadata;
using UserDocsApiLinker.JavaReflection;

namespace UserDocsApiLinker
{
    /// <summary>
    /// Main class for replace all finding entrance of friendly syntax in the document.
    /// </summary>
    public class ApiMemberLinker
    {
        public ApiMemberLinker(
            ApiMemberPlatform platform, 
            string url, 
            string assemblyFile,
            string xmlFile, 
            string logFile, 
            int logLevel)
        {
            FindAndApplyLicense();

            mPlatform = platform;
            mAbsoluteUrl = url;
            mAssemblyFile = assemblyFile;
            mXmlFile = xmlFile;

            mLogger = new ApiMemberLogging();
            mLogger.LogFileName = logFile;
            mLogger.LogLevel = logLevel;

        }

        /// <summary>
        /// Process document. Find all API member and replace theme with hyperlink friendly syntax text.
        /// </summary>
        public void ProcessDocument(string srcFileName, string dstFileName)
        {
            Document doc = new Document(srcFileName);

            ApiMemberCollectionJava apiMemberCollectionJava = new ApiMemberCollectionJava();
            ApiMemberCollectionNet apiMemberCollectionNet = new ApiMemberCollectionNet();

            switch (mPlatform)
            {
                case ApiMemberPlatform.Java:
                    apiMemberCollectionJava = FillApiMembersJava();
                    break;
                case ApiMemberPlatform.Net:
                    apiMemberCollectionNet = FillApiMembersNet();
                    break;
                default:
                    throw new ArgumentException("There is incorrect platform type.");
            }

            // R1.2
            doc.Accept(new ApiMemberDocVisitor());
            List<ApiMember> amCurrent = new List<ApiMember>();
            foreach (Section section in doc.Sections)
            {
                foreach(Node node in section.Body.ChildNodes)
                {
                    switch (node.NodeType)
                    {
                        case NodeType.Paragraph:
                            ProcessParagraph(doc, (Paragraph)node, apiMemberCollectionJava, apiMemberCollectionNet, amCurrent);
                            break;

                        case NodeType.Table:
                            ProcessTable(doc, (Table)node, apiMemberCollectionJava, apiMemberCollectionNet, amCurrent);
                            break;
                    }
                }
            }
            doc.Styles["ApiLink"].Font.Shading.BackgroundPatternColor = Color.Empty;
            doc.Save(dstFileName);
        }

        private void ProcessTable(Document doc, Table table,
            ApiMemberCollectionJava amcJava, ApiMemberCollectionNet amcNet, List<ApiMember> amCurrent)
        {
            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    foreach (Paragraph paragraph in cell.Paragraphs)
                    {
                        ProcessParagraph(doc, paragraph, amcJava, amcNet, amCurrent);
                    }
                }
            }
        }

        private void ProcessParagraph(Document document, Paragraph paragraph,
            ApiMemberCollectionJava amcJava, ApiMemberCollectionNet amcNet, List<ApiMember> amCurrent)
        {
            int i = 0;
            Run run;
            
            while (i < paragraph.Runs.Count)
            {
                run = paragraph.Runs[i];
                
                //R3.1 R3.2
                List<string> parStyles = new List<string> {"Heading 1", "Heading 2", "Heading 3", "Heading 4"};
                foreach (string parStyle in parStyles)
                {
                    if(paragraph.ParagraphFormat.Style.Name == parStyle)
                    {
                        amCurrent.Clear();
                        break;
                    }
                }

                if (run.Font.Style.Name == "ApiLink")
                {
                    ApiMember am;
                    switch (mPlatform)
                    {
                        case ApiMemberPlatform.Java:
                            am = amcJava.ReplaceApiMember(run.Text);
                            break;
                        case ApiMemberPlatform.Net:
                            am = amcNet.ReplaceApiMember(run.Text);
                            break;
                        default:
                            throw new ArgumentException("There is incorrect platform type.");
                    }
                    //R3.4.1
                    if(am.Type == ApiMemberType.Unknown)
                    {
                        mLogger.WriteToLog(string.Format("There is no Api member with given name or parameters found - \"{0}\"", am.FriendlyName));
                    }
                    else
                    {
                        //R3.2
                        if (!amCurrent.Contains(am))
                        {
                            amCurrent.Add(am);
                            ReplaceRunWithHyperlink(document, run, am);
                            run.Remove();
                            continue;
                        }
                        run.Text = am.FriendlyName;
                    }
                }
                i++;
            }
        }

        private static void ReplaceRunWithHyperlink(Document document, Run run, ApiMember apiMember)
        {
            DocumentBuilder documentBuilder = new DocumentBuilder(document);
            Field field = documentBuilder.InsertHyperlink(apiMember.FriendlyName, apiMember.Url, false);
            
            FieldStart fieldStart = field.Start;
            fieldStart.Font.StyleName = "Hyperlink";

            Run hlUrl = (Run)fieldStart.NextSibling;
            hlUrl.Font.StyleName = "Hyperlink";

            FieldSeparator fieldSeparator = (FieldSeparator)hlUrl.NextSibling;
            fieldSeparator.Font.StyleName = "Hyperlink";

            Run hlName = (Run)fieldSeparator.NextSibling;
            hlName.Font.StyleName = "Hyperlink";
            hlName.Font.HighlightColor = run.Font.HighlightColor;

            FieldEnd fieldEnd = (FieldEnd)hlName.NextSibling;
            fieldEnd.Font.StyleName = "Hyperlink";

            Paragraph pr = run.ParentParagraph;
            pr.InsertBefore(fieldStart, run);
            pr.InsertBefore(hlUrl, run);
            pr.InsertBefore(fieldSeparator, run);
            pr.InsertBefore(hlName, run);
            pr.InsertBefore(fieldEnd, run);
        }

        private ApiMemberCollectionJava FillApiMembersJava()
        {
            XmlTextReader textReader = new XmlTextReader(mXmlFile);
            ApiMemberCollectionJava apiMemberCollection = new ApiMemberCollectionJava();

            while (textReader.Read())
            {
                if ((textReader.LocalName == "member") && (textReader.AttributeCount > 0))
                {
                    try
                    {
                        apiMemberCollection.AddApiMember(textReader.GetAttribute("name"));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            const String classPath = "X:\\awuex\\lib\\Aspose.Words.jdk15.jar";
            const String asposeJar = "Aspose.Words.jdk15.jar";
            String baseAddress = /*mAbsoluteUrl + */"com/aspose/words/";
            JavaReflectionManager reflectionManager = new JavaReflectionManager(() => new JavaReflectionAdapter(classPath));
            JavaMetadataStorage storage = new JavaMetadataStorageBuilder().Build(reflectionManager, asposeJar);
            ILinkResolver linkResolver = new JavaLinkResolver(baseAddress, storage);
            LoadApiMemberUrlNetAssembly(apiMemberCollection.ApiMembers, linkResolver);
            AddBaseApiMemberUrl(apiMemberCollection.ApiMembers, linkResolver);
            CopyMemberAttributesFromBaseClass(apiMemberCollection.ApiMembers);
            // Fill with java specific metadata
            LoadJavaSpecificMemberUrl(baseAddress, apiMemberCollection.ApiMembers, storage);
            return apiMemberCollection;
        }

        private static void LoadJavaSpecificMemberUrl(String baseAddress, List<ApiMember> apiMembers, JavaMetadataStorage javaMetadata)
        {
            List<ApiMember> javaSpecificMembers = new List<ApiMember>();
            JavaApiMemberBuilder apiMemberBuilder = new JavaApiMemberBuilder(baseAddress);
            // Load types
            foreach (JavaClass javaClass in javaMetadata.GetAllClasses())
            {
                IList<ApiMember> portion = apiMemberBuilder.Build(javaClass, apiMembers);
                javaSpecificMembers.AddRange(portion);
            }
            apiMembers.AddRange(javaSpecificMembers);
        }

        private static void AddBaseApiMemberUrl(IList<ApiMember> apiMembers, ILinkResolver linkResolver)
        {
            IList<ApiMember> addList = new List<ApiMember>();
            foreach (ApiMember member in apiMembers)
            {
                ApiMember apiMember = new ApiMember
                                          {
                                              Type = member.Type,
                                              Key = member.Key,
                                              FriendlyName = member.FriendlyName,
                                              BaseType = member.BaseType
                                          };


                if ((member.Type == ApiMemberType.Method) &&
                    (ApiMemberUtils.IsOverloadsWithNoEmptyParametersMember(member, apiMembers)))
                {
                    bool isContain = false;
                    foreach(ApiMember addMember in addList)
                    {
                        if(addMember.Key == apiMember.Key)
                        {
                            isContain = true;
                            break;
                        }
                    }
                    if(!isContain)
                        addList.Add(apiMember);
                }
            }

            foreach (ApiMember apiMember in addList)
            {
                Boolean isConstructor = ApiMemberKeyManager.GetMemberName(apiMember).Equals(ApiMemberKeyManager.ConstructorName);
                apiMembers.Add(isConstructor ? linkResolver.FillConstructorSummary(apiMember) : linkResolver.FillMethodSummary(apiMember));
            }
        }

        private static void CopyMemberAttributesFromBaseClass(IList<ApiMember> apiMembers)
        {
            IList<ApiMember> addList = new List<ApiMember>();
            IList<ApiMember> cloneList = new List<ApiMember>();
            CopyList(cloneList, apiMembers);

            foreach(ApiMember apiMember in apiMembers)
            {
                IList<ApiMember> retList = CopyMemberAttributesFromBaseClass(cloneList, apiMember);
                CopyList(cloneList, retList);
                CopyList(addList, retList);
            }
            CopyList(apiMembers, addList);
        }

        private static IList<ApiMember> CopyMemberAttributesFromBaseClass(IList<ApiMember> apiMembers, ApiMember apiMember)
        {
            IList<ApiMember> addList = new List<ApiMember>();
            IList<ApiMember> cloneList = new List<ApiMember>();
            if ((apiMember.Type == ApiMemberType.Type)
                && (!string.IsNullOrEmpty(apiMember.BaseType))
                && (ApiMemberUtils.IsAsposeClass(apiMember.BaseType)))
            {
                ApiMember baseClass = null;
                foreach (ApiMember member in apiMembers)
                {
                    if ( (member.Key == apiMember.BaseType) && (member.Type == ApiMemberType.Type) )
                    {
                        baseClass = member;
                        break;
                    }
                }
                CopyList(cloneList, apiMembers);
                if( (baseClass != null)
                    && (!string.IsNullOrEmpty(baseClass.BaseType))
                    && (ApiMemberUtils.IsAsposeClass(baseClass.BaseType)))
                {
                    IList<ApiMember> retList = CopyMemberAttributesFromBaseClass(cloneList, baseClass);
                    CopyList(addList, retList);
                    CopyList(cloneList, retList);
                }
                if(baseClass != null)
                {
                    IList<ApiMember> retList = CopyMemberAttributesFromBaseClass(cloneList, baseClass, apiMember);
                    CopyList(addList, retList);
                    CopyList(cloneList, retList);
                }
            }
            return addList;
        }

        private static IList<ApiMember> CopyMemberAttributesFromBaseClass(IList<ApiMember> apiMembers, ApiMember srcClass, ApiMember dstClass)
        {
            IList<ApiMember> addList = new List<ApiMember>();

            foreach(ApiMember member in apiMembers)
            {
                if(ApiMemberUtils.IsMemberBelongToClass(member, srcClass))
                {
                    ApiMember modMember = (ApiMember)member.Clone();

                    ChangeBaseClassNameByParent(dstClass, modMember);

                    if (!IsMemberInList(apiMembers, modMember))
                        addList.Add(modMember);

                }
            }
            return addList;
        }

        private static int CopyList(IList<ApiMember> mainList, IList<ApiMember> addList)
        {
            int ret = 0;
            foreach (ApiMember addMember in addList)
            {
                if(!IsMemberInList(mainList, addMember))
                {
                    mainList.Add(addMember);
                    ret++;
                }
            }
            return ret;
        }

        private static bool IsMemberInList(IList<ApiMember> mainList, ApiMember member)
        {
            foreach (ApiMember curMember in mainList)
            {
                if (ApiMemberUtils.IsEquilMembers(curMember, member))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ChangeBaseClassNameByParent(ApiMember parentMember, ApiMember changedMember)
        {
            changedMember.Key = ReplaceClassName(parentMember.Key, changedMember.Key);
            changedMember.FriendlyName = ReplaceClassName(parentMember.FriendlyName, changedMember.FriendlyName);

        }

        private static string ReplaceClassName(string parentKey, string changedKey)
        {
            string[] parentArr = parentKey.Split('.');
            string[] changedArr = changedKey.Split('.');

            if (parentArr.Length >= changedArr.Length)
                return parentKey;

            for (int i = 0; i < parentArr.Length; i++)
                changedArr[i] = parentArr[i];

            return string.Join(".", changedArr);
        }

        private ApiMemberCollectionNet FillApiMembersNet()
        {
            XmlTextReader textReader = new XmlTextReader(mXmlFile);
            ApiMemberCollectionNet apiMemberCollection = new ApiMemberCollectionNet();

            while (textReader.Read())
            {
                if ((textReader.LocalName == "member") && (textReader.AttributeCount > 0))
                {
                    try
                    {
                        apiMemberCollection.AddApiMember(textReader.GetAttribute("name"));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            ILinkResolver linkResolver = new NetLinkResolver(mAbsoluteUrl);
            LoadApiMemberUrlNetAssembly(apiMemberCollection.ApiMembers, linkResolver);
            AddBaseApiMemberUrl(apiMemberCollection.ApiMembers, linkResolver);
            CopyMemberAttributesFromBaseClass(apiMemberCollection.ApiMembers);
            return apiMemberCollection;
        }

        private void LoadApiMemberUrlNetAssembly(IList<ApiMember> apiMembers, ILinkResolver resolver)
        {
            Assembly searchAssembly = Assembly.Load(AssemblyName.GetAssemblyName(mAssemblyFile));
            Type[] assemblyTypes = searchAssembly.GetTypes();

            foreach (Type assemblyType in assemblyTypes)
            {
                foreach (ApiMember member in apiMembers)
                {
                    switch (member.Type)
                    {
                        case ApiMemberType.Type:
                        {
                            if (assemblyType.FullName == member.Key)
                            {
                                resolver.FillType(member, assemblyType);
                            }
                            break;
                        }
                        case ApiMemberType.Event:
                            foreach (EventInfo ei in assemblyType.GetEvents())
                            {
                                String eventFullName = ApiMemberKeyManager.GetKey(ei);
                                if (eventFullName == member.Key)
                                {
                                    resolver.FillEvent(member, ei);
                                    break;
                                }
                            }
                            break;

                        case ApiMemberType.Field:
                            foreach (FieldInfo fi in assemblyType.GetFields())
                            {
                                String fieldFullName = ApiMemberKeyManager.GetKey(fi);
                                if (fieldFullName == member.Key)
                                {
                                    resolver.FillField(member, fi);
                                    break;
                                }
                            }
                            break;

                        case ApiMemberType.Property:
                            List<PropertyInfo> propertyList = new List<PropertyInfo>();
                            foreach (PropertyInfo property in assemblyType.GetProperties())
                            {
                                if (ApiMemberKeyManager.GetKey(property).Equals(member.Key))
                                    propertyList.Add(property);
                            }
                            resolver.FillProperty(member, propertyList);
                            break;
                        case ApiMemberType.Method:
                            if (ApiMemberKeyManager.GetMemberName(member).Equals(ApiMemberKeyManager.ConstructorName))
                            {
                                List<ConstructorInfo> constructorList = new List<ConstructorInfo>();
                                foreach (ConstructorInfo constructor in assemblyType.GetConstructors())
                                {
                                    if(ApiMemberKeyManager.GetKey(constructor).Equals(member.Key))
                                        constructorList.Add(constructor);
                                }
                                
                                resolver.FillConstructor(member, constructorList);
                            }
                            else
                            {
                                List<MethodInfo> methodList = new List<MethodInfo>();
                                foreach (MethodInfo method in assemblyType.GetMethods())
                                {
                                    if(ApiMemberKeyManager.GetKey(method).Equals(member.Key))
                                        methodList.Add(method);
                                }
                                resolver.FillMethod(member, methodList);
                            }
                            break;
                    }
                }
            }
        }

        private static void FindAndApplyLicense()
        {
            License licenseWords = new License();
            licenseWords.SetLicense("Aspose.Total.lic");
        }


        private readonly ApiMemberPlatform mPlatform;
        private readonly string mAbsoluteUrl;
        private readonly string mAssemblyFile;
        private readonly string mXmlFile;
        private readonly ApiMemberLogging mLogger;
    }
}
