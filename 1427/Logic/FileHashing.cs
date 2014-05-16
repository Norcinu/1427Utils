using System.IO;
using System.Security.Cryptography;

namespace PDTUtils
{
	static class FileHashing
	{
		public static string GetFileHash(string filename)
		{
			string result = "";
			
			try
			{
				FileStream stream = File.Open(filename, FileMode.Open);
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				byte[] byteHashValue = md5.ComputeHash(stream);
				stream.Close();
				
				string hashData = System.BitConverter.ToString(byteHashValue);
				hashData = hashData.Replace("-", "");
				result = hashData;
			}
			catch (System.Exception)
			{
				result = "Could not find hash of filename: " + filename;
			}

			return (result);
		}
	}
}
