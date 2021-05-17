using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KDSVideo.Infrastructure
{
    /// <summary>Helper class for platform detection.</summary>
    internal static class DesignerLibrary
    {
        private static DesignerPlatformLibrary? _detectedDesignerPlatformLibrary;
        private static bool? _isInDesignMode;

        private static DesignerPlatformLibrary DetectedDesignerLibrary
        {
            get
            {
                if (!DesignerLibrary._detectedDesignerPlatformLibrary.HasValue)
                    DesignerLibrary._detectedDesignerPlatformLibrary = new DesignerPlatformLibrary?(DesignerLibrary.GetCurrentPlatform());
                return DesignerLibrary._detectedDesignerPlatformLibrary.Value;
            }
        }

        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            if (Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e") != null)
                return DesignerPlatformLibrary.Silverlight;
            if (Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") != null)
                return DesignerPlatformLibrary.Net;
            return Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime") != null ? DesignerPlatformLibrary.WinRt : DesignerPlatformLibrary.Unknown;
        }

        public static bool IsInDesignMode
        {
            get
            {
                if (!DesignerLibrary._isInDesignMode.HasValue)
                    DesignerLibrary._isInDesignMode = new bool?(DesignerLibrary.IsInDesignModePortable());
                return DesignerLibrary._isInDesignMode.Value;
            }
        }

        private static bool IsInDesignModePortable()
        {
            switch (DesignerLibrary.DetectedDesignerLibrary)
            {
                case DesignerPlatformLibrary.Net:
                    return DesignerLibrary.IsInDesignModeNet();
                case DesignerPlatformLibrary.WinRt:
                    return DesignerLibrary.IsInDesignModeMetro();
                case DesignerPlatformLibrary.Silverlight:
                    bool flag = DesignerLibrary.IsInDesignModeSilverlight();
                    if (!flag)
                        flag = DesignerLibrary.IsInDesignModeNet();
                    return flag;
                default:
                    return false;
            }
        }

        private static bool IsInDesignModeSilverlight()
        {
            try
            {
                Type type = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
                if (type == null)
                    return false;
                PropertyInfo declaredProperty = type.GetTypeInfo().GetDeclaredProperty("IsInDesignTool");
                return declaredProperty != null && (bool)declaredProperty.GetValue((object)null, (object[])null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeMetro()
        {
            try
            {
                return (bool)Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime").GetTypeInfo().GetDeclaredProperty("DesignModeEnabled").GetValue((object)null, (object[])null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeNet()
        {
            try
            {
                Type type1 = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (type1 == null)
                    return false;
                object obj1 = type1.GetTypeInfo().GetDeclaredField("IsInDesignModeProperty").GetValue((object)null);
                Type type2 = Type.GetType("System.ComponentModel.DependencyPropertyDescriptor, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                Type type3 = Type.GetType("System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (type2 == null || type3 == null)
                    return false;
                List<MethodInfo> list = type2.GetTypeInfo().GetDeclaredMethods("FromProperty").ToList<MethodInfo>();
                if (list == null || list.Count == 0)
                    return false;
                MethodInfo methodInfo = list.FirstOrDefault<MethodInfo>((Func<MethodInfo, bool>)(mi => mi.IsPublic && mi.IsStatic && mi.GetParameters().Length == 2));
                if (methodInfo == null)
                    return false;
                object obj2 = methodInfo.Invoke((object)null, new object[2]
                {
          obj1,
          (object) type3
                });
                if (obj2 == null)
                    return false;
                PropertyInfo declaredProperty1 = type2.GetTypeInfo().GetDeclaredProperty("Metadata");
                if (declaredProperty1 == null)
                    return false;
                object obj3 = declaredProperty1.GetValue(obj2, (object[])null);
                Type type4 = Type.GetType("System.Windows.PropertyMetadata, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                if (obj3 == null || type4 == null)
                    return false;
                PropertyInfo declaredProperty2 = type4.GetTypeInfo().GetDeclaredProperty("DefaultValue");
                return declaredProperty2 != null && (bool)declaredProperty2.GetValue(obj3, (object[])null);
            }
            catch
            {
                return false;
            }
        }

        private enum DesignerPlatformLibrary
        {
            Unknown,
            Net,
            WinRt,
            Silverlight,
        }
    }
}
