using System;
using System.IO;
using System.Reflection;

using Sce.PlayStation.Core;


namespace Tutorial.Utility
{
	public class Utility
	{
		static public Byte[] ReadEmbeddedFile(string embeddedFileName)
		{
			Assembly resourceAssembly = Assembly.GetExecutingAssembly();
			if (resourceAssembly.GetManifestResourceInfo(embeddedFileName) == null)
			{
				throw new FileNotFoundException("File not found.", embeddedFileName);
			}
	
			Stream fileStream = resourceAssembly.GetManifestResourceStream(embeddedFileName);
			Byte[] dataBuffer = new Byte[fileStream.Length];
			
			fileStream.Read(dataBuffer, 0, dataBuffer.Length);
			
			return dataBuffer;
		}
		
		/// <summary>
		/// Creates the vector4 from color sample.
		/// </summary>
		/// <returns>
		/// The vector4 from color sample.
		/// </returns>
		/// <param name='color'>
		/// Color sample. __RRGGBB
		/// </param>
		static public Vector4 CreateVector4FromColorSample(Int32 colorSample)
		{
			return new Vector4( 
			                   ((colorSample & 0xFF0000)>>16)/255.0f,
			                   ((colorSample & 0xFF00)>>8)/255.0f,
			                   (colorSample & 0xFF)/255.0f,
			                   1.0f);
		}
		
		
	}
}

