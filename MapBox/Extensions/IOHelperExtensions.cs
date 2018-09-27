using System;
using System.IO;
using System.Reflection;
using MapBox.Helpers;

namespace MapBox.Extensions
{
	public static class IOHelperExtensions
	{
		public static byte[] toByteArray(this Stream stream)
		{
			if (stream is MemoryStream)
				return (stream as MemoryStream).ToArray();

			using (var memoryStream = new MemoryStream()) {
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		public static Stream toStream(this byte[] byteArray)
		{
			return new MemoryStream(byteArray);
		}

		public static Stream getRawStremFromEmbeddedResource(this string fileName, Assembly assembly, double width, double height)
		{
			byte[] buffer = null;
			var nativeScale = DisplayMetricsHelper.instance.nativeScale;
			var address = $"{assembly.GetName().Name}.{fileName.Trim()}";
			using (var stream = assembly.GetManifestResourceStream(address)) {
				if (stream == null)
					throw new FileNotFoundException("File not found, make sure that the file extension is included and the directory separator is a dot (.).", fileName);
				buffer = new byte[stream.Length];
				stream.Read(buffer, 0, (int)stream.Length);
				using (var editableImage = Plugin.ImageEdit.CrossImageEdit.Current.CreateImage(buffer)) {
					var modified = editableImage.Resize((int)(width * nativeScale), (int)(height * nativeScale)).ToPng();
					return new MemoryStream(modified);
				}
			}
		}
	}
}
