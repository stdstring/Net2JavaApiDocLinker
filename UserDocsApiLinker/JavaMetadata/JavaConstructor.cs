using System;
using System.Collections.Generic;

namespace UserDocsApiLinker.JavaMetadata
{
    internal class JavaConstructor
    {
        public JavaConstructor(String source)
        {
            Signature signature = Signature.Parse(source);
            if(signature == null)
                throw new ArgumentException();
            Source = source;
            Name = signature.Name;
            Parameters = signature.Parameters;
        }

        public String Source { get; private set; }
        public String Name { get; private set; }
        public List<String> Parameters { get; private set; }
    }
}
