using System.Windows.Forms;

namespace PDTUtils.MVVM
{
    class WPFMessageBoxService : IMessageBoxService
    {
        public bool ShowMessage(string text, string caption)
        {
            if (MessageBox.Show(text, caption) == DialogResult.OK)
                return true;
            else
                return false;
        }
    }
}
