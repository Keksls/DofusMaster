using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DofusMaster
{
    public partial class MainWindow : Window
    {
        int selectedAccountIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            Manager.OnSelectAccount += Manager_OnSelectAccount;
            Manager.OnSetReplicationMode += Manager_OnSetReplicationMode;
            IO.LeftClick += IO_LeftClick;
            refreshButton_Click(null, null);
        }

        private void Manager_OnSetReplicationMode(ReplicationMode obj)
        {
            replicationMode.Content = "Mouse click forward : " + obj.ToString();
        }

        private void IO_LeftClick(int x, int y)
        {
            if (chkEnableReplication.IsChecked.HasValue && chkEnableReplication.IsChecked.Value)
                Manager.OrderReplication(x, y);
        }

        private void Manager_OnSelectAccount(int accountIndex)
        {
            Dispatcher.Invoke(() =>
            {
                lbAccounts.SelectedIndex = accountIndex;
            });
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            lbAccounts.Items.Clear();
            Manager.RefreshConnectedAccounts();
            foreach (var account in Manager.ConnectedAccounts)
            {
                lbAccounts.Items.Add(account);
            }
            rightPanel.IsEnabled = Manager.HasAccounts;
        }

        private void lbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbAccounts.SelectedIndex >= 0)
            {
                BindSelectionPanel(lbAccounts.SelectedIndex);
            }
            else
            {
                selectAccountName.Content = "No account selected";
                selectionPanel.IsEnabled = false;
            }
        }

        public void BindSelectionPanel(int order)
        {
            selectionPanel.Visibility = Visibility.Visible;
            selectionPanel.IsEnabled = true;
            selectedAccountIndex = order;
            DofusAccount account = Manager.GetAccount(order);
            selectAccountName.Content = "[" + account.SelectionKey.ToString() + "] : " + account.Name;
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            selectedAccountIndex--;
            if (selectedAccountIndex < 0)
                selectedAccountIndex++;
            else
            {
                int downOrder = selectedAccountIndex;
                int startOrder = selectedAccountIndex + 1;

                Manager.GetAccount(downOrder).Order = startOrder;
                Manager.GetAccount(startOrder).Order = downOrder;

                refreshButton_Click(null, null);
                BindSelectionPanel(selectedAccountIndex);
                lbAccounts.SelectedIndex = selectedAccountIndex;
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            selectedAccountIndex++;
            if (selectedAccountIndex >= Manager.ConnectedAccounts.Count)
                selectedAccountIndex--;
            else
            {
                int upOrder = selectedAccountIndex;
                int startOrder = selectedAccountIndex - 1;

                Manager.GetAccount(upOrder).Order = startOrder;
                Manager.GetAccount(startOrder).Order = upOrder;

                refreshButton_Click(null, null);
                BindSelectionPanel(selectedAccountIndex);
                lbAccounts.SelectedIndex = selectedAccountIndex;
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string text = tbConsoleText.Text.Trim();
            tbConsoleText.Text = "";
            if (text.Length > 0)
            {
                Thread t = new Thread(() =>
                {
                    foreach (var account in Manager.ConnectedAccounts)
                    {
                        if (!account.Selected)
                            continue;
                        account.WriteIntoConsole(text);
                        Thread.Sleep(10);
                    }
                });
                t.Start();
            }
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("-[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnTravel_Click(object sender, RoutedEventArgs e)
        {
            int x;
            if (int.TryParse(tbX.Text, out x))
            {
                int y;
                if (int.TryParse(tbY.Text, out y))
                {
                    Thread t = new Thread(() =>
                    {
                        foreach (var account in Manager.ConnectedAccounts)
                        {
                            if (!account.Selected)
                                continue;
                            account.WriteIntoConsole("/travel " + x + " " + y);
                        }

                        Thread.Sleep(1500);

                        foreach (var account in Manager.ConnectedAccounts)
                        {
                            if (!account.Selected)
                                continue;
                            account.ShowWindow();
                            Thread.Sleep(100);
                            account.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                            Thread.Sleep(100);
                        }
                    });
                    t.Start();
                }
            }
        }

        private void btnCloseAll_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                foreach (var account in Manager.ConnectedAccounts)
                {
                    if (!account.Selected)
                        continue;
                    account.Process.Kill();
                    Thread.Sleep(100);
                }
                Thread.Sleep(100);
                refreshButton_Click(null, null);
            });
            t.Start();
        }

        private void btnOpenConfig_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(SaveManager.Path);
        }

        private void btnKeyCode_Click(object sender, RoutedEventArgs e)
        {
            KeyCode kc = new KeyCode();
            kc.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SaveManager.SaveAccounts();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SaveManager.SaveAccounts();
        }
    }
}