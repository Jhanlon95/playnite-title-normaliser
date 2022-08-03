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
            if (!settings.Settings.NormaliseOnUpdate)
            {
                settings.Settings.LastRefreshOnLibUpdate = DateTime.Now;
                SavePluginSettings(settings.Settings);
                return;
            }

            List<Game> updateGames = new List<Game>();

            GlobalProgressOptions progressOptions = new GlobalProgressOptions("Normalising on library update", true)
            {
                IsIndeterminate = false
            };
            PlayniteApi.Dialogs.ActivateGlobalProgress((a) =>
            {
                var games = PlayniteApi.Database.Games;
                a.ProgressMaxValue = games.Count();

                using (PlayniteApi.Database.BufferedUpdate())
                {
                    foreach (var game in games)
                    {
                        a.CurrentProgressValue++;
                        if (a.CancelToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (game.Added != null && game.Added > settings.Settings.LastRefreshOnLibUpdate)
                        {
                            if (!settings.Settings.NormaliseOnUpdate)
                            {
                                continue;
                            }
                            updateGames.Add(game);
                        }

                    }

                    TitleNormaliserHelper.NormaliseTitle(updateGames, settings.Settings);
                } 
            }, progressOptions);

            settings.Settings.LastRefreshOnLibUpdate = DateTime.Now;
            SavePluginSettings(settings.Settings);
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
                        ) { IsIndeterminate = false };

                        PlayniteApi.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                        {
                            try
                            {
                                List<Game> list = API.Instance.Database.Games.ToList();

                                TitleNormaliserHelper.NormaliseTitle(list, settings.Settings);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.StackTrace);
                            }
                        }, globalProgressOptions);


                    }
                },

                new MainMenuItem
                {
                    MenuSection = "@TitleNormalisation",
                    Description = "Normalise Selected Titles",
                    Action = (mainMenuItem) =>
                    {
                        GlobalProgressOptions globalProgressOptions = new GlobalProgressOptions(
                            "TitleNormaliser - Normalising Game Titles",
                            true
                        ) { IsIndeterminate = false };

                        PlayniteApi.Dialogs.ActivateGlobalProgress((activateGlobalProgress) =>
                        {
                            try
                            {
                              List<Game> list = API.Instance.MainView.SelectedGames.ToList();
                              TitleNormaliserHelper.NormaliseTitle(list, settings.Settings);
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

        public override IEnumerable<GameMenuItem> GetGameMenuItems(GetGameMenuItemsArgs args)
        {
            if (settings.Settings.ShowRightClickMenu)
            {
                yield return new GameMenuItem
                {
                    MenuSection = "Title Normaliser",
                    Description = "Normalise Selected Titles",

                    Action = (mainMenuItem) =>
                    {
                        var games = args.Games.Distinct().ToList();
                        TitleNormaliserHelper.NormaliseTitle(games, settings.Settings);
                    }
                };
            }
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