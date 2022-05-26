using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DofusMaster
{
    /// <summary>
    /// Logique d'interaction pour KeyCode.xaml
    /// </summary>
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
