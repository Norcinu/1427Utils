using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class ErrorStatusViewModel
    {
        public string ErrorMessage { get; set; }

        public ErrorStatusViewModel()
        {
            ErrorMessage = "";
        }

        public ICommand GetError
        {
            get { return new DelegateCommand(o => GetErrorMessage()); }
        }
        void GetErrorMessage()
        {
            var errorCode = BoLib.getError();
            if (errorCode > 0)
            {
                ErrorMessage = "Current Error : " + errorCode + "\nPress Button to clear Errors";
            }
            else
                ErrorMessage = "No Current Error";
        }
    }
}
