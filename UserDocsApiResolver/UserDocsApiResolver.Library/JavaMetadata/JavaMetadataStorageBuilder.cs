using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.JavaReflection;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class JavaMetadataStorageBuilder
    {
        public JavaMetadataStorage Build(JavaReflectionManager reflectionManager, String jarName)
        {
            Logger.Debug("Build{) enter");
            JavaReflectionRequest classesRequest = new JavaReflectionRequest(JavaReflectionCommands.GetClassesFromJar, jarName);
            String[] javaClasses = reflectionManager.Process(classesRequest);
            IList<JavaClass> classes = new List<JavaClass>();
            foreach (String javaClass in javaClasses)
            {
                if (IsSuitableClass(javaClass))
                {
                    Logger.DebugFormat("Recognize java class : {0}",javaClass);
                    classes.Add(BuildClass(reflectionManager, javaClass));
                }
            }
            Logger.Debug("Build{) exit");
            return new JavaMetadataStorage(classes);
        }

        private JavaClass BuildClass(JavaReflectionManager reflectionManager, String className)
        {
            JavaReflectionRequest memberRequest = new JavaReflectionRequest(JavaReflectionCommands.GetMembersForClass, className);
            String[] members = reflectionManager.Process(memberRequest) ?? new String[0];
            IList<JavaField> fields = new List<JavaField>();
            IList<JavaConstructor> ctors = new List<JavaConstructor>();
            IList<JavaMethod> methods = new List<JavaMethod>();
            foreach (String member in members)
            {
                if (member.StartsWith(FieldPrefix))
                {
                    Logger.DebugFormat("Recognize java field : {0}", member);
                    fields.Add(new JavaField(member.Substring(FieldPrefix.Length)));
                }
                if (member.StartsWith(ConstructorPrefix))
                {
                    Logger.DebugFormat("Recognize java constructor : {0}", member);
                    ctors.Add(new JavaConstructor(member.Substring(ConstructorPrefix.Length)));
                }
                if (member.StartsWith(MethodPrefix))
                {
                    Logger.DebugFormat("Recognize java method : {0}", member);
                    methods.Add(new JavaMethod(member.Substring(MethodPrefix.Length)));
                }
            }
            return new JavaClass(className, fields, ctors, methods);
        }

        private Boolean IsSuitableClass(String className)
        {
            if (!className.StartsWith(CommonDefs.JavaAsposePrefix))
                return false;
            if (Char.IsLower(className, CommonDefs.JavaAsposePrefix.Length))
                return false;
            return true;
        }

        private const String FieldPrefix = "F:";
        private const String ConstructorPrefix = "C:";
        private const String MethodPrefix = "M:";

        private static readonly ILog Logger = LogManager.GetLogger("JavaMetadataStorageBuilder");
    }
}
