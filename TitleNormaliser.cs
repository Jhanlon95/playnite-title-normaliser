using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TitleNormaliser
{
    public class TitleNormaliser : GenericPlugin
    {
        private readonly ILogger logger = LogManager.GetLogger();

        private TitleNormaliserSettingsViewModel settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("2a4b8ac6-f29c-4dc5-97dc-aa89504a0979");

        public TitleNormaliser(IPlayniteAPI api) : base(api)
        {
            

            settings = new TitleNormaliserSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {

            List<MainMenuItem> mainMenuItems = new List<MainMenuItem>
            {
                new MainMenuItem
                {
                    MenuSection = "@TitleNormalisation",
                    Description = "Normalise All Titles",
                    Action = (mainMenuItem) =>
                    {
                        GlobalProgressOptions globalProgressOptions = new GlobalProgressOptions(
                            "TitleNormaliser - Normalising Game Titles",
                            true
                        );
                        globalProgressOptions.IsIndeterminate = false;

                        PlayniteApi.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                        {
                            try
                            {
                                TitleNormaliserHelper.NormaliseTitle(PlayniteApi.Database.Games, settings.Settings);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.StackTrace);
                            }
                        }, globalProgressOptions);


                    }
                }
            };

            return mainMenuItems;
        }

        

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new TitleNormaliserSettingsView();
        }
    }


}