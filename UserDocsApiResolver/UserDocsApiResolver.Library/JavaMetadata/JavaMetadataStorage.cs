using System;
using System.Collections.Generic;
using log4net;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class JavaMetadataStorage
    {
        public JavaMetadataStorage(IEnumerable<JavaClass> source)
        {
            _javaClasses = new List<JavaClass>(source);
        }

        public List<JavaClass> GetAllClasses()
        {
            Logger.Debug("GetAllClasses() enter");
            List<JavaClass> classes = _javaClasses;
            Logger.Debug("GetAllClasses() exit");
            return classes;
        }

        public JavaClass FindClass(String className)
        {
            Logger.DebugFormat("FindClass({0}) enter", className);
            JavaClass result = _javaClasses.Find(cl => cl.FullName.Equals(className));
            Logger.DebugFormat("FindClass({0}) exit", className);
            return result;
        }

        public JavaField FindField(String className, String fieldName)
        {
            Logger.DebugFormat("FindField({0}, {1}) enter", className, fieldName);
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
            {
                Logger.DebugFormat("FindField({0}, {1}) exit", className, fieldName);
                return null;
            }
            JavaField result = javaClass.FieldList.Find(field => field.Name.Equals(fieldName));
            Logger.DebugFormat("FindField({0}, {1}) exit", className, fieldName);
            return result;
        }

        public List<JavaConstructor> FindConstructors(String className)
        {
            Logger.DebugFormat("FindConstructors({0}) enter", className);
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
            {
                Logger.DebugFormat("FindConstructors({0}) exit", className);
                return new List<JavaConstructor>();
            }
            List<JavaConstructor> result = new List<JavaConstructor>(javaClass.ConstructorList);
            Logger.DebugFormat("FindConstructors({0}) exit", className);
            return result;
        }

        public List<JavaMethod> FindMethods(String className, String methodName)
        {
            Logger.DebugFormat("FindMethods({0}, {1}) enter", className, methodName);
            JavaClass javaClass = FindClass(className);
            if (javaClass == null)
            {
                Logger.DebugFormat("FindMethods({0}, {1}) exit", className, methodName);
                return new List<JavaMethod>();
            }
            List<JavaMethod> methods = new List<JavaMethod>();
            foreach (JavaMethod method in javaClass.MethodList)
            {
                if(method.Name.Equals(methodName))
                    methods.Add(method);
            }
            Logger.DebugFormat("FindMethods({0}, {1}) exit", className, methodName);
            return methods;
        }

        private readonly List<JavaClass> _javaClasses;

        private static readonly ILog Logger = LogManager.GetLogger("JavaMetadataStorage");
    }
}
