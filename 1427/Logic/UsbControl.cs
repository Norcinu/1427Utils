using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace PDTUtils.Logic
{
	/// <summary>
	/// Don't bother with this shizzle just, export the C++ routines into dll.
	/// </summary>
	class UsbControl : IDisposable
	{
		ManagementEventWatcher attachWatcher;
		ManagementEventWatcher detachWatcher;

		public UsbControl()
		{
			
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// http://msdn.microsoft.com/en-us/library/fs2xkftw(v=vs.85).aspx
			// Check to see if Dispose has already been called.
			//if (!this.disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				//if (disposing)
				//{
					// Dispose managed resources.
				//	Components.Dispose();
				//}
				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
				//CloseHandle(handle);
				//handle = IntPtr.Zero;
				// Note that this is not thread safe.
				// Another thread could start disposing the object
				// after the managed resources are disposed,
				// but before the disposed flag is set to true.
				// If thread safety is necessary, it must be
				// implemented by the client.

			}
			//disposed = true;
		}

		void Attaching(object sender, EventArrivedEventArgs e)
		{
			if (sender != attachWatcher)
				return;
			Console.WriteLine("Attaching");
		}

		~UsbControl()
		{
			this.Dispose();
		}
	}
}
