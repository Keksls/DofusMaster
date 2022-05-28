using System.Windows;

namespace DofusMaster
{
    public partial class KeyCode : Window
    {
        public KeyCode()
        {
            InitializeComponent();
            IO.KeyDown += IO_KeyDown;
        }

        private void IO_KeyDown(VirtualKeys key)
        {
            lbKeyCode.Content = (int)key;
        }
    }
}