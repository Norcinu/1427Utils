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
#if DEBUG
			using (StreamWriter writer = new StreamWriter(filename, true))
			{
				writer.WriteLine(theOutput.ToString());
			}
#endif
		}
		
		public static void WriteCollectionToFile(string filename, T[] theOutput)
		{
#if DEBUG
			using (StreamWriter writer = new StreamWriter(filename))
			{
				foreach (var s in theOutput)
				{
					writer.WriteLine(s);
				}
			}
#endif
		}
	}
}
