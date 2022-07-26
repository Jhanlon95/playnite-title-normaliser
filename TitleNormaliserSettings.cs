﻿using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleNormaliser
{
    public class TitleNormaliserSettings : ObservableObject
    {

        public DateTime LastRefreshOnLibUpdate = DateTime.Now;

        private string option1 = string.Empty;
        private string option2 = string.Empty;
        private bool normaliseOnUpdate = false;
        private bool showRightClickMenu = false;
        private bool optionThatWontBeSaved = false;
        private bool turnDashIntoColon = false;

        public string Option1 { get => option1; set => SetValue(ref option1, value); }
        public string Option2 { get => option2; set => SetValue(ref option2, value); }
        public bool NormaliseOnUpdate { get => normaliseOnUpdate; set => SetValue(ref normaliseOnUpdate, value); }
        public bool ShowRightClickMenu { get => showRightClickMenu; set => SetValue(ref showRightClickMenu, value); }
        public bool TurnDashIntoColon { get => turnDashIntoColon; set => SetValue(ref turnDashIntoColon, value); }

        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonDontSerialize` ignore attribute.
        [DontSerialize]
        public bool OptionThatWontBeSaved { get => optionThatWontBeSaved; set => SetValue(ref optionThatWontBeSaved, value); }
    }

    public class TitleNormaliserSettingsViewModel : ObservableObject, ISettings
    {
        private readonly TitleNormaliser plugin;
        private TitleNormaliserSettings editingClone { get; set; }

        private TitleNormaliserSettings settings;
        public TitleNormaliserSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public TitleNormaliserSettingsViewModel(TitleNormaliser plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<TitleNormaliserSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new TitleNormaliserSettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}