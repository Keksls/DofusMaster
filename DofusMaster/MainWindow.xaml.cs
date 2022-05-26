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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DofusMaster
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Manager.OnSelectAccount += Manager_OnSelectAccount;
            Manager.RefreshConnectedAccounts();
            foreach(var account in Manager.ConnectedAccounts)
                lbAccounts.Items.Add(account);
        }

        private void Manager_OnSelectAccount(int accountIndex)
        {
            selectedAccountLabel.Content = Manager.ConnectedAccounts[accountIndex].Name;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            lbAccounts.Items.Clear();
            Manager.RefreshConnectedAccounts();
            foreach (var account in Manager.ConnectedAccounts)
                lbAccounts.Items.Add(account);
        }

        private void lbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbAccounts.SelectedIndex >= 0)
            {
                Manager.ConnectedAccounts[lbAccounts.SelectedIndex].ShowWindow();
            }
        }
    }
}