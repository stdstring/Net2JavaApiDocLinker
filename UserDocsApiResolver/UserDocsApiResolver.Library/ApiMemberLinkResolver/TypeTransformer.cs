using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using UserDocsApiResolver.Library.Utils;
using log4net;

namespace UserDocsApiResolver.Library.ApiMemberLinkResolver
{
    internal class TypeTransformer
    {
        public String Transform(Type type)
        {
            Logger.DebugFormat("Transform {0}", type.Name);
            if (type.IsArray)
                return GetJavaArrayType(type);
            if (type.IsEnum)
                return JavaEnumType;
            return GetJavaType(type);
        }

        public String TransformAsposeType(String typeName)
        {
            Logger.DebugFormat("TransformAsposeType {0}", typeName);
            if (ApiMemberUtils.IsAsposeClass(typeName))
                return String.Format(AsposeFullNameTemplate, MemberPartDef.GetLastNamePart(typeName));
            return null;
        }

        private String GetJavaType(Type netType)
        {
            // TODO (std_string) : think about FullName
            if (String.IsNullOrEmpty(netType.FullName))
            {
                Logger.WarnFormat("Can't get full name for {0}", netType.Name);
                return null;
            }
            if (_simpleTypeLinks.ContainsKey(netType.FullName))
                return _simpleTypeLinks[netType.FullName];
            if (ApiMemberUtils.IsAsposeClass(netType.FullName))
                return String.Format(AsposeFullNameTemplate, netType.Name);
            Logger.WarnFormat("Can't find corresponding java type for {0}", netType.FullName);
            return null;
        }

        private String GetJavaArrayType(Type netType)
        {
            Type elementType = netType.GetElementType();
            String javaElementType = GetJavaType(elementType);
            return String.IsNullOrEmpty(javaElementType) ? null : String.Format(ArrayTemplate, javaElementType);
        }

        private readonly IDictionary<String, String> _simpleTypeLinks =
            new Dictionary<String, String>
                {
                    // TODO (std_string) : may be sbyte ???
                    {typeof (Byte).FullName, "byte"},
                    {typeof (Boolean).FullName, "boolean"},
                    {typeof (Int32).FullName, "int"},
                    {typeof (Single).FullName, "float"},
                    {typeof (Double).FullName, "double"},
                    {typeof (Guid).FullName, "java.lang.UUID"},
                    {typeof (String).FullName, "java.lang.String"},
                    {typeof (Object).FullName, "java.lang.Object"},
                    {typeof (Type).FullName, "java.lang.Class"},
                    {typeof (DateTime).FullName, "java.util.Date"},
                    {typeof (Color).FullName, "java.awt.Color"},
                    {typeof (PrinterSettings).FullName, "javax.print.attribute.AttributeSet"},
                    {typeof (Graphics).FullName, "java.awt.Graphics2D"},
                    {typeof (Image).FullName, "java.awt.image.BufferedImage"},
                    {typeof (PointF).FullName, "java.awt.geom.Point2D.Float"},
                    {typeof (Regex).FullName, "java.util.regex.Pattern"}
                };

        private const String JavaEnumType = "int";
        private const String ArrayTemplate = "{0}[]";
        private const String AsposeFullNameTemplate = CommonDefs.JavaAsposePrefix + "{0}";

        private static readonly ILog Logger = LogManager.GetLogger("TypeTransformer");
    }
}
