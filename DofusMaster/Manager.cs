using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DofusMaster
{
    public static class Manager
    {
        public static VirtualKeys NextKey { get; private set; }
        public static VirtualKeys PreviewKey { get; private set; }
        public static event Action NextKeyPressed;
        public static event Action PreviewKeyPressed;
        public static event Action<int> OnSelectAccount;
        public static List<DofusAccount> ConnectedAccounts { get; private set; }
        public static bool HasAccounts { get { return ConnectedAccounts != null && ConnectedAccounts.Count > 0; } }
        private static int currentAccountIndex = 0;

        static Manager()
        {
            currentAccountIndex = 0;
            ConnectedAccounts = new List<DofusAccount>();
            NextKey = VirtualKeys.Right;
            PreviewKey = VirtualKeys.Left;
            Keys.KeyDown += Keys_KeyDown;
            NextKeyPressed += Manager_NextKeyPressed;
            PreviewKeyPressed += Manager_PreviewKeyPressed;
        }

        private static void Manager_PreviewKeyPressed()
        {
            if (!HasAccounts)
                return;
            currentAccountIndex--;
            if (currentAccountIndex < 0)
                currentAccountIndex = ConnectedAccounts.Count - 1;
            ConnectedAccounts[currentAccountIndex].ShowWindow();
            OnSelectAccount?.Invoke(currentAccountIndex);
        }

        private static void Manager_NextKeyPressed()
        {
            if (!HasAccounts)
                return;
            currentAccountIndex++;
            if (currentAccountIndex >= ConnectedAccounts.Count)
                currentAccountIndex = 0;
            ConnectedAccounts[currentAccountIndex].ShowWindow();
            OnSelectAccount?.Invoke(currentAccountIndex);
        }

        private static void Keys_KeyDown(VirtualKeys key)
        {
            if (key == NextKey)
                NextKeyPressed?.Invoke();
            else if (key == PreviewKey)
                PreviewKeyPressed?.Invoke();
        }

        public static void RefreshConnectedAccounts()
        {
            ConnectedAccounts = new List<DofusAccount>();
            foreach (var process in Process.GetProcessesByName("Dofus"))
                if (process.ProcessName == "Dofus")
                    ConnectedAccounts.Add(new DofusAccount(process));
        }
    }
}