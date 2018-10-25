using System;
using Xamarin.Forms;

namespace MapBox
{
    public static class PinImageDescriptorFactory
    {
        public static PinImageDescriptor FromBundle(string fileName)
        {
            return PinImageDescriptor.FromBundle(fileName);
        }

        public static PinImageDescriptor FromPath(string folderPath, string fileName, double height, double width)
        {
            return PinImageDescriptor.FromPath(folderPath, fileName, height, width);
        }

        public static string GetStringImage(this Pin pin)
        {
            switch (pin.icon.Type)
            {
                case PinImageDescriptorType.Bundle:
                    return pin.icon.FileName;
                case PinImageDescriptorType.Path:
                    return pin.icon.FilePath;
                default:
                    return pin.icon.FileName;
            }
        }
    }
}
