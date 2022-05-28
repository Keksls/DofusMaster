using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DofusMaster
{
    public static class Manager
    {

        public static event Action NextKeyPressed;
        public static event Action PreviewKeyPressed;
        public static event Action<int> OnSelectAccount;
        public static event Action<ReplicationMode> OnSetReplicationMode;

        public static List<DofusAccount> ConnectedAccounts { get; private set; }
        public static bool HasAccounts { get { return ConnectedAccounts != null && ConnectedAccounts.Count > 0; } }
        private static int currentAccountIndex = 0;
        private static Random rnd = new Random();
        private static ReplicationMode replicationMode;
        public static ReplicationMode ReplicationMode { get { return replicationMode; } set { replicationMode = value; OnSetReplicationMode?.Invoke(replicationMode); } }
        private static Queue<replicationData> replicationQueue;
        private static bool canReplicate = true;

        static Manager()
        {
            replicationQueue = new Queue<replicationData>();
            ConnectedAccounts = new List<DofusAccount>();
            currentAccountIndex = 0;
            IO.KeyDown += Keys_KeyDown;
            IO.KeyUp += IO_KeyUp; ;
            NextKeyPressed += Manager_NextKeyPressed;
            PreviewKeyPressed += Manager_PreviewKeyPressed;
            ThreadPool.QueueUserWorkItem(ReplicationLoop);
        }

        public static DofusAccount GetAccount(int order)
        {
            return ConnectedAccounts[order];
        }

        public static void SelectAccount(int index)
        {
            currentAccountIndex = index;
            OnSelectAccount?.Invoke(currentAccountIndex);
        }

        private static void IO_KeyUp(VirtualKeys key)
        {
            if (key == SaveManager.Save.instantKey && ReplicationMode == ReplicationMode.Instent)
                ReplicationMode = ReplicationMode.None;

            else if (key == SaveManager.Save.smoothKey && ReplicationMode == ReplicationMode.Smooth)
                ReplicationMode = ReplicationMode.None;
        }

        private static void Manager_PreviewKeyPressed()
        {
            if (!HasAccounts || !HasEnabledAccount())
                return;

            int i = 0;
            while (i < ConnectedAccounts.Count)
            {
                currentAccountIndex--;
                if (currentAccountIndex < 0)
                    currentAccountIndex = ConnectedAccounts.Count - 1;
                if (ConnectedAccounts[currentAccountIndex].Selected)
                {
                    ConnectedAccounts[currentAccountIndex].ShowWindow();
                    break;
                }
                i++;
            }
        }

        private static void Manager_NextKeyPressed()
        {
            if (!HasAccounts || !HasEnabledAccount())
                return;

            int i = 0;
            while (i < ConnectedAccounts.Count)
            {
                currentAccountIndex++;
                if (currentAccountIndex >= ConnectedAccounts.Count)
                    currentAccountIndex = 0;
                if (ConnectedAccounts[currentAccountIndex].Selected)
                {
                    ConnectedAccounts[currentAccountIndex].ShowWindow();
                    break;
                }
                i++;
            }
        }

        public static bool HasEnabledAccount()
        {
            foreach (var account in ConnectedAccounts)
                if (account.Selected)
                    return true;
            return false;
        }

        private static void Keys_KeyDown(VirtualKeys key)
        {
            if (key == SaveManager.Save.NextKey)
            {
                if (!SaveManager.Save.NextPreviexCtrl || SaveManager.Save.NextPreviexCtrl && IO.isCtrl)
                    if (!SaveManager.Save.NextPreviexShift || SaveManager.Save.NextPreviexShift && IO.isShift)
                        NextKeyPressed?.Invoke();
            }

            else if (key == SaveManager.Save.PreviewKey)
            {
                if (!SaveManager.Save.NextPreviexCtrl || SaveManager.Save.NextPreviexCtrl && IO.isCtrl)
                    if (!SaveManager.Save.NextPreviexShift || SaveManager.Save.NextPreviexShift && IO.isShift)
                        PreviewKeyPressed?.Invoke();
            }

            else if (key == SaveManager.Save.instantKey)
                ReplicationMode = ReplicationMode.Instent;

            else if (key == SaveManager.Save.smoothKey)
                ReplicationMode = ReplicationMode.Smooth;
        }

        public static void RefreshConnectedAccounts()
        {
            DeregisterAllAccountsShortcuts();
            List<DofusAccount> accounts = new List<DofusAccount>();

            foreach (var process in Process.GetProcessesByName("Dofus"))
                if (process.ProcessName == "Dofus" && !process.MainWindowTitle.StartsWith("Dofus"))
                    accounts.Add(new DofusAccount(process));

            int maxOrder = 0;
            if (SaveManager.Save.Accounts.Count > 0)
                maxOrder = SaveManager.Save.Accounts.Max(a => a.Value.Order);

            foreach (DofusAccount account in accounts)
            {
                if (SaveManager.Save.Accounts.ContainsKey(account.Name))
                    account.Order = SaveManager.Save.Accounts[account.Name].Order;
                else
                    account.Order = maxOrder++;
            }

            ConnectedAccounts = new List<DofusAccount>();

            int i = 0;
            foreach (DofusAccount account in accounts.OrderBy(a => a.Order))
            {
                account.SetOrder(i);
                ConnectedAccounts.Add(account);
                i++;
            }
            SaveManager.SaveAccounts();
        }

        public static void DeregisterAllAccountsShortcuts()
        {
            if (HasAccounts)
                foreach (var account in ConnectedAccounts)
                    account.DeregisterShortCut();
        }

        private static void doReplication(replicationData data)
        {
            switch (data.Mode)
            {
                default:
                case ReplicationMode.None:
                    break;

                case ReplicationMode.Instent:
                    foreach (var account in ConnectedAccounts)
                    {
                        if (!account.Selected)
                            continue;
                        account.InstentMouseClick(data.X, data.Y);
                        Thread.Sleep(rnd.Next(SaveManager.Save.MinMovementDelay, SaveManager.Save.MaxMovementDelay));
                    }
                    break;

                case ReplicationMode.Smooth:
                    foreach (var account in ConnectedAccounts)
                    {
                        if (!account.Selected)
                            continue;
                        account.ShowWindow();
                        Thread.Sleep(rnd.Next(SaveManager.Save.MinMovementDelay, SaveManager.Save.MaxMovementDelay));
                        account.SmoothMouseClick(data.X, data.Y);
                        Thread.Sleep(rnd.Next(SaveManager.Save.MinMovementDelay, SaveManager.Save.MaxMovementDelay));
                    }
                    break;
            }
        }

        private static void ReplicationLoop(object sender)
        {
            while (true)
            {
                if (replicationQueue.Count > 0)
                {
                    replicationData data = replicationQueue.Dequeue();
                    doReplication(data);
                    canReplicate = true;
                }
                Thread.Sleep(1);
            }
        }

        public static void OrderReplication(int X, int Y)
        {
            if (replicationMode != ReplicationMode.None && canReplicate)
            {
                replicationQueue.Enqueue(new replicationData(X, Y, replicationMode));
                canReplicate = false;
            }
        }
    }

    public struct replicationData
    {
        public int X;
        public int Y;
        public ReplicationMode Mode;

        public replicationData(int x, int y, ReplicationMode mode)
        {
            X = x;
            Y = y;
            Mode = mode;
        }
    }

    public enum ReplicationMode
    {
        None,
        Instent,
        Smooth
    }
}