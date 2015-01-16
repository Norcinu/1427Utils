using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System;

namespace System.Runtime.CompilerServices
{
	public class ExtensionAttribute : Attribute { }
}

namespace PDTUtils
{
	public static class Extension
	{
		/// <summary>
		/// http://stackoverflow.com/questions/10279092/how-to-get-child-by-type-in-wpf-container
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="depObj"></param>
		/// <returns></returns>
		public static List<T> GetChildOfType<T>(this DependencyObject depObj)
			where T : DependencyObject
		{
			if (depObj == null) return null;
			List<T> elementList = new List<T>();

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = (child as T) ?? null;//GetChildOfType<T>(child);
				if (result != null) //return result;
					elementList.Add(result);
			}
			//			if (elementList.Count > 0)
			//		return null;
			return elementList;
		}

		public static void RemoveAll<T>(this ObservableCollection<T> coll)
		{
			for (int i = coll.Count - 1; i >= 0; i--)
			{
				if (coll[i] != null)
					coll.RemoveAt(i);
			}
		}

        public static void BubbleSort<T>(this ObservableCollection<T> o)
        {
            for (int i = o.Count - 1; i >= 0; i--) 
            {
                for (int j = 1; j <= i; j++) 
                {
                    object o1 = o[j - 1];
                    object o2 = o[j];
                    if (((IComparable)o1).CompareTo(o2) > 0) 
                    {
                    //    o.Remove(o1);
                    //    o.Insert(j, o1);
                    }
                }
            }
        }
	}
}