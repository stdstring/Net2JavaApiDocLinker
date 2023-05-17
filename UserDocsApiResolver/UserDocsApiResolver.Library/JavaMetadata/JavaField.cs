using System;
using UserDocsApiResolver.Library.Utils;

namespace UserDocsApiResolver.Library.JavaMetadata
{
    internal class JavaField
    {
        public JavaField(String name)
        {
            Int32 fieldDelimiter = name.LastIndexOf(MemberPartDef.ClassNameDelimiter);
            Name = fieldDelimiter == -1 ? name : name.Substring(fieldDelimiter + 1);
        }

        public String Name { get; private set; }
    }
}
