using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DofusMaster
{
    public static class SaveManager
    {
        public static string Path { get; private set; }
        public static Save Save { get; private set; }

        static SaveManager()
        {
            Path = Environment.CurrentDirectory + @"\save.json";
            if (!HasSave())
                CreateDefaultSave();
            LoadSave();
        }

        public static bool HasSave()
        {
            return File.Exists(Path);
        }

        public static void CreateDefaultSave()
        {
            Save save = new Save();
            save.Accounts = new Dictionary<string, DofusAccount>();
            save.MinMovementDelay = 120;
            save.MaxMovementDelay = 400;
            save.NextKey = VirtualKeys.Right;
            save.PreviewKey = VirtualKeys.Left;
            save.instantKey = VirtualKeys.LeftControl;
            save.smoothKey = VirtualKeys.RightControl;
            string json = JsonConvert.SerializeObject(save);
            File.WriteAllText(Path, json);
        }

        public static void LoadSave()
        {
            string json = File.ReadAllText(Path);
            Save = JsonConvert.DeserializeObject<Save>(json);
        }

        public static void SaveAccounts()
        {
            Save.Accounts = new Dictionary<string, DofusAccount>();
            foreach (var account in Manager.ConnectedAccounts)
                Save.Accounts.Add(account.Name, account);
            SaveSave();
        }

        public static void SaveSave()
        {
            string json = JsonConvert.SerializeObject(Save, Formatting.Indented);
            File.WriteAllText(Path, json);
        }
    }

    [Serializable]
    public class Save
    {
        public Dictionary<string, DofusAccount> Accounts { get; set; }
        public int MinMovementDelay { get; set; }
        public int MaxMovementDelay { get; set; }
        public bool AccountShortcutCtrl { get; set; }
        public bool NextPreviexCtrl { get; set; }
        public bool NextPreviexShift { get; set; }
        public VirtualKeys NextKey { get; set; }
        public VirtualKeys PreviewKey { get; set; }
        public VirtualKeys instantKey { get; set; }
        public VirtualKeys smoothKey { get; set; }

        public Save() { }
    }
}