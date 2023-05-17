using System;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.JavaReflection
{
    internal class JavaReflectionManager : IDisposable
    {
        public JavaReflectionManager(Func<IJavaReflectionAdapter> adapterFactory)
        {
            _adapterFactory = adapterFactory;
            _adapter = adapterFactory();
        }

        public String[] Process(JavaReflectionRequest request)
        {
            Logger.DebugFormat("Process({0}) enter", request);
            try
            {
                String[] result = _adapter.ProcessRequest(request.GetRequestString());
                Logger.DebugFormat("Process({0}) exit", request);
                return result;
            }
            catch (Exception exc)
            {
                _adapter.Dispose();
                _adapter = _adapterFactory();
                Logger.WarnFormat("Exception occur {0}", exc);
                Logger.DebugFormat("Process({0}) exit", request);
                return new String[0];
            }
        }

        public void Close()
        {
            _adapter.Close();
        }

        public void Dispose()
        {
            _adapter.Dispose();
        }

        private readonly Func<IJavaReflectionAdapter> _adapterFactory;
        private IJavaReflectionAdapter _adapter;

        private static readonly ILog Logger = LogManager.GetLogger("JavaMetadataStorageBuilder");
  }
}
