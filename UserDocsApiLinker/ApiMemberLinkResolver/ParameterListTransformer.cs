using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UserDocsApiLinker.JavaMetadata;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.ApiMemberLinkResolver
{
    internal class ParameterListTransformer
    {
        public ParameterListTransformer(JavaMetadataStorage javaMetadataStorage)
        {
            this.javaMetadataStorage = javaMetadataStorage;
            typeTransformer = new TypeTransformer();
            methodNameTransformer = new MethodNameTransformer();
            javaMetadataSearch = new JavaMetadataSearch();
        }

        public String[] Transform(MethodInfo method)
        {
            String methodName = methodNameTransformer.Transform(method);
            ParameterInfo[] parameters = method.GetParameters();
            Func<String, String[]> paramSearcher = typeName => SearchMethodParameters(typeName, methodName, parameters);
            return Transform(method.ReflectedType, parameters, paramSearcher);
        }

        public String[] Transform(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            Func<String, String[]> paramSearcher = typeName => SearchConstructorParameters(typeName, parameters);
            return Transform(constructor.ReflectedType, constructor.GetParameters(), paramSearcher);
        }

        public String[] Transform(PropertyInfo property)
        {
            String methodName = methodNameTransformer.Transform2Java(property);
            ParameterInfo[] parameters = property.GetIndexParameters();
            Func<String, String[]> paramSearcher = typeName => SearchMethodParameters(typeName, methodName, parameters);
            return Transform(property.ReflectedType, parameters, paramSearcher);
        }

        private String[] Transform(Type containedType, ParameterInfo[] parameters, Func<String, String[]> paramSearcher)
        {
            List<String> javaParameters = new List<String>();
            Array.ForEach(parameters, parameter => javaParameters.Add(typeTransformer.Transform(parameter.ParameterType)));
            // if we can transfotm parameters without using Java Reflection API
            if (javaParameters.TrueForAll(parameter => !String.IsNullOrEmpty(parameter)))
                return javaParameters.ToArray();
            // if we have to using Java Reflection API for transfotm parameters
            String typeName = typeTransformer.Transform(containedType);
            return paramSearcher(typeName);
        }

        private String[] SearchMethodParameters(String typeName, String methodName, ParameterInfo[] parameters)
        {
            if (String.IsNullOrEmpty(typeName) || String.IsNullOrEmpty(methodName))
            {
                // log(type name and/or method name is null or empty);
                return null;
            }
            IList<ISearchCondition> paramConditions = CreateSearchConditions(parameters);
            IEnumerable<JavaMethod> source = javaMetadataStorage.FindMethods(typeName, methodName);
            IList<JavaMethod> searchResult = javaMetadataSearch.Search(source, methodName, paramConditions);
            if (searchResult.Count == 0)
            {
                // log(not_found);
                return null;
            }
            if (searchResult.Count > 1)
            {
                // log(ambiguous_found);
                return null;
            }
            IList<String> transformedParameters = searchResult[0].Parameters;
            return new List<String>(transformedParameters).ToArray();
        }

        private String[] SearchConstructorParameters(String typeName, ParameterInfo[] parameters)
        {
            if (String.IsNullOrEmpty(typeName))
            {
                // log(type name and/or method name is null or empty);
                return null;
            }
            IList<ISearchCondition> paramConditions = CreateSearchConditions(parameters);
            IEnumerable<JavaConstructor> source = javaMetadataStorage.FindConstructors(typeName);
            IList<JavaConstructor> searchResult = javaMetadataSearch.Search(source, paramConditions);
            if (searchResult.Count == 0)
            {
                // log(not_found);
                return null;
            }
            if (searchResult.Count > 1)
            {
                // log(ambiguous_found);
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
                String javaType = typeTransformer.Transform(parameter.ParameterType);
                if(!String.IsNullOrEmpty(javaType))
                {
                    searchConditions.Add(new SimpleValueSearchCondition(javaType));
                    continue;
                }
                // TODO (std_string) : think about FullName
                if (String.IsNullOrEmpty(parameter.ParameterType.FullName))
                    continue;
                if(ambiguousTypeLinks.ContainsKey(parameter.ParameterType.FullName))
                {
                    searchConditions.Add(new ArraySearchCondition(ambiguousTypeLinks[parameter.ParameterType.FullName]));
                    continue;
                }
                searchConditions.Add(new FalseSearchCondition());
            }
            return searchConditions;
        }

        private readonly JavaMetadataStorage javaMetadataStorage;
        private readonly TypeTransformer typeTransformer;
        private readonly MethodNameTransformer methodNameTransformer;
        private readonly JavaMetadataSearch javaMetadataSearch;

        private readonly IDictionary<String, String[]> ambiguousTypeLinks =
            new Dictionary<String, String[]>
                {
                    {typeof (Stream).FullName, new[] {"java.io.InputStream", "java.io.OutputStream"}}
                };
    }
}
