using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UserDocsApiResolver.Library.JavaMetadata;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal class ParameterListTransformer
    {
        public ParameterListTransformer(JavaMetadataStorage javaMetadataStorage)
        {
            _javaMetadataStorage = javaMetadataStorage;
            _typeTransformer = new TypeTransformer();
            _methodNameTransformer = new MethodNameTransformer();
            _javaMetadataSearch = new JavaMetadataSearch();
        }

        public String[] Transform(MethodInfo method)
        {
            Logger.DebugFormat("TransformMethod {0}", MemberInfoPresentation.GetString(method));
            String methodName = _methodNameTransformer.Transform(method);
            ParameterInfo[] parameters = method.GetParameters();
            Func<String, String[]> paramSearcher = typeName => SearchMethodParameters(typeName, methodName, parameters);
            return Transform(method.ReflectedType, parameters, paramSearcher);
        }

        public String[] Transform(ConstructorInfo constructor)
        {
            Logger.DebugFormat("TransformConstructor {0}", MemberInfoPresentation.GetString(constructor));
            ParameterInfo[] parameters = constructor.GetParameters();
            Func<String, String[]> paramSearcher = typeName => SearchConstructorParameters(typeName, parameters);
            return Transform(constructor.ReflectedType, constructor.GetParameters(), paramSearcher);
        }

        public String[] Transform(PropertyInfo property)
        {
            Logger.DebugFormat("TransformProperty {0}", MemberInfoPresentation.GetString(property));
            String methodName = _methodNameTransformer.Transform2Java(property);
            ParameterInfo[] parameters = property.GetIndexParameters();
            Func<String, String[]> paramSearcher = typeName => SearchMethodParameters(typeName, methodName, parameters);
            return Transform(property.ReflectedType, parameters, paramSearcher);
        }

        private String[] Transform(Type containedType, ParameterInfo[] parameters, Func<String, String[]> paramSearcher)
        {
            List<String> javaParameters = new List<String>();
            Array.ForEach(parameters, parameter => javaParameters.Add(_typeTransformer.Transform(parameter.ParameterType)));
            // if we can transfotm parameters without using Java Reflection API
            if (javaParameters.TrueForAll(parameter => !String.IsNullOrEmpty(parameter)))
                return javaParameters.ToArray();
            // if we have to using Java Reflection API for transfotm parameters
            String typeName = _typeTransformer.Transform(containedType);
            return paramSearcher(typeName);
        }

        private String[] SearchMethodParameters(String typeName, String methodName, ParameterInfo[] parameters)
        {
            if (String.IsNullOrEmpty(typeName) || String.IsNullOrEmpty(methodName))
            {
                Logger.Warn("Search failed");
                return null;
            }
            IList<ISearchCondition> paramConditions = CreateSearchConditions(parameters);
            IEnumerable<JavaMethod> source = _javaMetadataStorage.FindMethods(typeName, methodName);
            IList<JavaMethod> searchResult = _javaMetadataSearch.Search(source, methodName, paramConditions);
            if (searchResult.Count == 0)
            {
                Logger.WarnFormat("Empty search result for type = {0} method = {1}", typeName, methodName);
                return null;
            }
            if (searchResult.Count > 1)
            {
                Logger.WarnFormat("Ambiguous search result for type = {0} method = {1}", typeName, methodName);
                return null;
            }
            IList<String> transformedParameters = searchResult[0].Parameters;
            return new List<String>(transformedParameters).ToArray();
        }

        private String[] SearchConstructorParameters(String typeName, ParameterInfo[] parameters)
        {
            if (String.IsNullOrEmpty(typeName))
            {
                Logger.Warn("Search failed");
                return null;
            }
            IList<ISearchCondition> paramConditions = CreateSearchConditions(parameters);
            IEnumerable<JavaConstructor> source = _javaMetadataStorage.FindConstructors(typeName);
            IList<JavaConstructor> searchResult = _javaMetadataSearch.Search(source, paramConditions);
            if (searchResult.Count == 0)
            {
                Logger.WarnFormat("Empty search result for type = {0}", typeName);
                return null;
            }
            if (searchResult.Count > 1)
            {
                Logger.WarnFormat("Ambiguous search result for type = {0}", typeName);
                return null;
            }
            IList<String> transformedParameters = searchResult[0].Parameters;
            return new List<String>(transformedParameters).ToArray();
        }

        private IList<ISearchCondition> CreateSearchConditions(ParameterInfo[] parameters)
        {
            IList<ISearchCondition> searchConditions = new List<ISearchCondition>(parameters.Length);
            foreach (ParameterInfo parameter in parameters)
            {
                String javaType = _typeTransformer.Transform(parameter.ParameterType);
                if(!String.IsNullOrEmpty(javaType))
                {
                    searchConditions.Add(new SimpleValueSearchCondition(javaType));
                    continue;
                }
                // TODO (std_string) : think about FullName
                if (String.IsNullOrEmpty(parameter.ParameterType.FullName))
                    continue;
                if(_ambiguousTypeLinks.ContainsKey(parameter.ParameterType.FullName))
                {
                    searchConditions.Add(new ArraySearchCondition(_ambiguousTypeLinks[parameter.ParameterType.FullName]));
                    continue;
                }
                searchConditions.Add(new FalseSearchCondition());
            }
            return searchConditions;
        }

        private readonly JavaMetadataStorage _javaMetadataStorage;
        private readonly TypeTransformer _typeTransformer;
        private readonly MethodNameTransformer _methodNameTransformer;
        private readonly JavaMetadataSearch _javaMetadataSearch;
        private readonly IDictionary<String, String[]> _ambiguousTypeLinks =
            new Dictionary<String, String[]>
                {
                    {typeof (Stream).FullName, new[] {"java.io.InputStream", "java.io.OutputStream"}}
                };

        private static readonly ILog Logger = LogManager.GetLogger("ParameterListTransformer");
    }
}
