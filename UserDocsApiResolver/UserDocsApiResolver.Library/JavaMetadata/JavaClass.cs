﻿using System;
using System.Collections.Generic;
using UserDocsApiResolver.Library.Utils;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class JavaClass
    {
        public JavaClass(String className,
            IEnumerable<JavaField> fields,
            IEnumerable<JavaConstructor> ctors,
            IEnumerable<JavaMethod> methods)
        {
            _className = className;
            List<JavaField> fieldList = new List<JavaField>(fields);
            fieldList.Sort((field1, field2) => String.Compare(field1.Name, field2.Name));
            List<JavaConstructor> ctorList = new List<JavaConstructor>(ctors);
            ctorList.Sort((ctor1, ctor2) => String.Compare(ctor1.Source, ctor2.Source));
            List<JavaMethod> methodList = new List<JavaMethod>(methods);
            methodList.Sort((method1, method2) => String.Compare(method1.Source, method2.Source));
            FieldList = fieldList;
            ConstructorList = ctorList;
            MethodList = methodList;
        }

        public String FullName { get { return _className; } }
        public String ShortName
        {
            get
            {
                Int32 dotIndex = _className.LastIndexOf(MemberPartDef.ClassNameDelimiter);
                return dotIndex == -1 ? _className : _className.Substring(dotIndex + 1);
            }
        }

        public List<JavaField> FieldList { get; private set; }
        public List<JavaConstructor> ConstructorList { get; private set; }
        public List<JavaMethod> MethodList { get; private set; }

        private readonly String _className;
    }
}
