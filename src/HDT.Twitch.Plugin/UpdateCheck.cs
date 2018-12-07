using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace HDT.Twitch.Plugin
{
    public class UpdateCheck
    {
        public static async void Run(Version current)
        {
            Log.Info("Checking for updates...", "Twitch");

            const string versionXmlUrl = @"https://raw.githubusercontent.com/Prom3theu5/HDT-Data/master/Plugins/twitch-version";
            try
            {
                Log.Info("Current version: " + current, "Twitch");
                string xml;
                using (WebClient wc = new WebClient())
                    xml = await wc.DownloadStringTaskAsync(versionXmlUrl);

                Version newVersion = new Version(XmlManager<SerializableVersion>.LoadFromString(xml).ToString());
                Log.Info("Latest version: " + newVersion, "Twitch");

                if (newVersion > current)
                {
                    await Task.Delay(5000);
                    MessageDialogResult result =
                        await
                        Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowMessageAsync("Twitch update available!", "(Plugins can not be updated automatically)",
                                                           MessageDialogStyle.AffirmativeAndNegative,
                                                           new MetroDialogSettings { AffirmativeButtonText = "download", NegativeButtonText = "not now" });
                    if (result == MessageDialogResult.Affirmative)
                        Process.Start(@"https://github.com/Prom3theu5/HDT-Twitch/releases");
                }
            }
            catch (Exception e)
            {
                Log.Error("Error checking for new version.\n\n" + e, "Twitch");
            }
        }
    }
}