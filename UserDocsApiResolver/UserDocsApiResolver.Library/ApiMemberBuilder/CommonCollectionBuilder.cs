using System;
using System.Collections.Generic;
using System.Reflection;
using UserDocsApiResolver.Library.ApiMemberLinkResolver;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberBuilder
{
    internal class CommonCollectionBuilder
    {
        public CommonCollectionBuilder(ApiMemberDocProcessor memberDocProcessor, String sourceAssemblyName, ILinkResolver linkResolver)
        {
            if (memberDocProcessor == null)
                throw new ArgumentNullException("memberDocProcessor");
            if (sourceAssemblyName == null)
                throw new ArgumentNullException("sourceAssemblyName");
            if (linkResolver == null)
                throw new ArgumentNullException("linkResolver");
            _memberDocProcessor = memberDocProcessor;
            _sourceAssemblyName = sourceAssemblyName;
            _linkResolver = linkResolver;
            _processMemberActions = new Dictionary<ApiMemberType, Action<Type, ApiMember>>
                                        {
                                            {ApiMemberType.Type, ProcessTypeMember},
                                            {ApiMemberType.Event, ProcessEventMember},
                                            {ApiMemberType.Field, ProcessFieldMember},
                                            {ApiMemberType.Property, ProcessPropertyMember},
                                            {ApiMemberType.Method, ProcessMethodMember}
                                        };
        }

        public List<ApiMember> Build()
        {
            Logger.Debug("Build() enter");
            List<ApiMember> members = _memberDocProcessor.Build();
            ProcessApiMemberFromAssembly(members);
            AppendSummaryApiMembers(members);
            ApiMemberCollectionUtils.CopyApiMembersFromBaseClass(members);
            Logger.DebugFormat("Build {0} members", members.Count);
            Logger.Debug("Build() exit");
            return members;
        }

        private void ProcessApiMemberFromAssembly(IList<ApiMember> apiMembers)
        {
            Assembly searchAssembly = Assembly.Load(AssemblyName.GetAssemblyName(_sourceAssemblyName));
            Type[] assemblyTypes = searchAssembly.GetTypes();
            foreach (Type assemblyType in assemblyTypes)
            {
                foreach (ApiMember member in apiMembers)
                {
                    _processMemberActions[member.Type](assemblyType, member);
                }
            }
        }

        private void AppendSummaryApiMembers(IList<ApiMember> apiMembers)
        {
            List<ApiMember> summaryMemberList = new List<ApiMember>();
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
                    if (summaryMemberList.Find(summaryMember => summaryMember.Key.Equals(apiMember.Key)) == null)
                        summaryMemberList.Add(apiMember);
                }
            }
            foreach (ApiMember member in summaryMemberList)
            {
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                Boolean isConstructor = ApiMemberUtils.GetMemberName(member).Equals(CommonDefs.Ctor);
                apiMembers.Add(isConstructor
                                   ? _linkResolver.FillConstructorSummary(member)
                                   : _linkResolver.FillMethodSummary(member));
            }
        }

        private void ProcessTypeMember(Type source, ApiMember member)
        {
            if (Equals(source.FullName, member.Key))
            {
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                _linkResolver.FillType(member, source);
            }
        }

        private void ProcessEventMember(Type source, ApiMember member)
        {
            foreach (EventInfo ei in source.GetEvents())
            {
                String eventFullName = ApiMemberUtils.GetKey(ei);
                if (Equals(eventFullName, member.Key))
                {
                    Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                    _linkResolver.FillEvent(member, ei);
                    return;
                }
            }
        }

        private void ProcessFieldMember(Type source, ApiMember member)
        {
            foreach (FieldInfo fi in source.GetFields())
            {
                String fieldFullName = ApiMemberUtils.GetKey(fi);
                if (Equals(fieldFullName, member.Key))
                {
                    Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                    _linkResolver.FillField(member, fi);
                    return;
                }
            }
        }

        private void ProcessPropertyMember(Type source, ApiMember member)
        {
            List<PropertyInfo> propertyList = new List<PropertyInfo>();
            foreach (PropertyInfo property in source.GetProperties())
            {
                if (Equals(ApiMemberUtils.GetKey(property), member.Key))
                    propertyList.Add(property);
            }
            if (propertyList.Count > 0)
            {
                Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                _linkResolver.FillProperty(member, propertyList);
            }
        }

        private void ProcessMethodMember(Type source, ApiMember member)
        {
            if (ApiMemberUtils.GetMemberName(member).Equals(CommonDefs.Ctor))
            {
                List<ConstructorInfo> constructorList = new List<ConstructorInfo>();
                foreach (ConstructorInfo constructor in source.GetConstructors())
                {
                    if (Equals(ApiMemberUtils.GetKey(constructor), member.Key))
                        constructorList.Add(constructor);
                }
                if (constructorList.Count > 0)
                {
                    Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                    _linkResolver.FillConstructor(member, constructorList);
                }
            }
            else
            {
                List<MethodInfo> methodList = new List<MethodInfo>();
                foreach (MethodInfo method in source.GetMethods())
                {
                    if (Equals(ApiMemberUtils.GetKey(method), member.Key))
                        methodList.Add(method);
                }
                if (methodList.Count > 0)
                {
                    Logger.DebugFormat("Process {0}", ApiMemberUtils.GetString(member));
                    _linkResolver.FillMethod(member, methodList);
                }
            }
        }

        private readonly ApiMemberDocProcessor _memberDocProcessor;
        private readonly String _sourceAssemblyName;
        private readonly ILinkResolver _linkResolver;
        private readonly IDictionary<ApiMemberType, Action<Type, ApiMember>> _processMemberActions;

        private static readonly ILog Logger = LogManager.GetLogger("CommonCollectionBuilder");
    }
}