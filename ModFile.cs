using System;

namespace ModPlayer
{
	internal static class ModFile
	{
		internal static byte[] Data;

		internal static void Load(string path)
		{
			Data = System.IO.File.ReadAllBytes(path);
		}
	}
}