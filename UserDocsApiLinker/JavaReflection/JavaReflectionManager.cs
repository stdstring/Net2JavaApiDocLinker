using System;
using UserDocsApiLinker.Utils;

namespace UserDocsApiLinker.JavaReflection
{
    internal class JavaReflectionManager : IDisposable
    {
        public JavaReflectionManager(Func<IJavaReflectionAdapter> adapterFactory)
        {
            this.adapterFactory = adapterFactory;
            adapter = adapterFactory();
        }

        public String[] Process(JavaReflectionRequest request)
        {
            try
            {
                return adapter.ProcessRequest(request.GetRequestString());
            }
            catch (Exception)
            {
                adapter.Dispose();
                adapter = adapterFactory();
                return new String[0];
            }
        }

        public void Close()
        {
            adapter.Close();
        }

        public void Dispose()
        {
            adapter.Dispose();
        }

        private readonly Func<IJavaReflectionAdapter> adapterFactory;
        private IJavaReflectionAdapter adapter;
    }
}
