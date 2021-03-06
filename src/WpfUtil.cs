﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Internal.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Color = System.Windows.Media.Color;

namespace IgnoreFiles
{
    public static class WpfUtil
    {
        public static BitmapSource GetIconForImageMoniker(ImageMoniker? imageMoniker, int sizeX, int sizeY)
        {
            if (imageMoniker == null)
            {
                return null;
            }

            IVsImageService2 vsIconService = ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) as IVsImageService2;

            if (vsIconService == null)
            {
                return null;
            }

            ImageAttributes imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                LogicalHeight = sizeY,
                LogicalWidth = sizeX,
                StructSize = Marshal.SizeOf(typeof(ImageAttributes))
            };

            IVsUIObject result = vsIconService.GetImage(imageMoniker.Value, imageAttributes);

            object data;
            result.get_Data(out data);
            BitmapSource glyph = data as BitmapSource;

            if (glyph != null)
            {
                glyph.Freeze();
            }

            return glyph;
        }

        public static ImageSource GetIconForFile(DependencyObject owner, string file, out bool themeIcon)
        {
            return GetImage(owner, file, __VSUIDATAFORMAT.VSDF_WPF, out themeIcon);
        }

        private static ImageSource GetImage(DependencyObject owner, string file, __VSUIDATAFORMAT format, out bool themeIcon)
        {
            IVsImageService imageService = ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) as IVsImageService;
            BitmapSource result = null;
            uint iconSource = (uint)__VSIconSource.IS_Unknown;

            if (imageService != null && !string.IsNullOrWhiteSpace(file))
            {
                IVsUIObject image = imageService.GetIconForFileEx(file, format, out iconSource);
                if (image != null)
                {
                    object imageData = GetObjectData(image);
                    result = imageData as BitmapSource;
                }
            }

            themeIcon = (iconSource == (uint)__VSIconSource.IS_VisualStudio);

            if (themeIcon && result != null && owner != null)
            {
                return ThemeImage(owner, result);
            }

            return result;
        }

        public static ImageSource ThemeImage(DependencyObject owner, BitmapSource source)
        {
            Color background = ImageThemingUtilities.GetImageBackgroundColor(owner);
            return ImageThemingUtilities.GetOrCreateThemedBitmapSource(source, background, true, Colors.Black, false);
        }

        private static object GetObjectData(IVsUIObject obj)
        {
            Validate.IsNotNull(obj, "obj");

            object value;
            int result = obj.get_Data(out value);

            if (result != VSConstants.S_OK)
            {
                throw new COMException("Could not get object data", result);
            }

            return (value);
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
