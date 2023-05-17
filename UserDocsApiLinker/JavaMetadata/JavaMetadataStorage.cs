using System;
using System.Collections.Generic;

namespace UserDocsApiLinker.JavaMetadata
{
    internal class JavaMetadataStorage
    {
        public JavaMetadataStorage(IEnumerable<JavaClass> source)
        {
            javaClasses = new List<JavaClass>(source);
        }

        public List<JavaClass> GetAllClasses()
        {
            return javaClasses;
        }

        public JavaClass FindClass(String className)
        {
            return javaClasses.Find(cl => cl.FullName.Equals(className));
        }

        public JavaField FindField(String className, String fieldName)
        {
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
                return null;
            return javaClass.FieldList.Find(field => field.Name.Equals(fieldName));
        }

        public List<JavaConstructor> FindConstructors(String className)
        {
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
                return new List<JavaConstructor>();
            return new List<JavaConstructor>(javaClass.ConstructorList);
        }

        public List<JavaMethod> FindMethods(String className, String methodName)
        {
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
                return new List<JavaMethod>();
            List<JavaMethod> methods = new List<JavaMethod>();
            foreach (JavaMethod method in javaClass.MethodList)
            {
                if(method.Name.Equals(methodName))
                    methods.Add(method);
            }
            return methods;
        }

        private readonly List<JavaClass> javaClasses;
    }
}
