using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDTUtils.Logic
{
	public static class MyDebug<T>
	{
		public static void WriteToFile(string filename, T theOutput)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				writer.WriteLine(theOutput.ToString());
			}
		}

		public static void WriteCollectionToFile(string filename, T[] theOutput)
		{
			using (StreamWriter writer = new StreamWriter(filename, true))
			{
				foreach (var s in theOutput)
				{
					writer.WriteLine(s);
				}
			}
		}
	}
}
