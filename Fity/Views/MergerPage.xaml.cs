using Fity.Data;
using Fity.Data.TCX;
using Fity.Models;
using Fity.Utils;
using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MergerPage : Page
    {
        private ActivityManager activityManager;
        private Session mergedActivity;

        public GpsDataManager DataManager { get; private set; }

        public MergerPage()
        {
            this.InitializeComponent();
            this.Loaded += MergerPage_Loaded;
        }

        private void MergerPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataManager = new GpsDataManager();
            this.FilesMap.MapServiceToken = Constants.BingMapsKey;
            this.MergedMap.MapServiceToken = Constants.BingMapsKey;
        }

        private async void MergerPage_AddFile(object sender, RoutedEventArgs e)
        {
            // Clear any previously returned files between iterations of this scenario

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".gprx");
            openPicker.FileTypeFilter.Add("*");
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                var defaultLocationInputs = new List<Tuple<double, double, int>>();

                // Application now has read/write access to the picked file(s)
                foreach (StorageFile file in files)
                {
                    this.FilesList.Items.Add(file.Name);
                    var gpsFileInfo = file.ToGpsFileInfo();
                    var loader = this.DataManager.AddToSession(gpsFileInfo);
                }

                this.activityManager = new ActivityManager(this.DataManager);
                foreach (var activityExtended in await this.activityManager.GetSessions())
                {
                    foreach (var mapEle in activityExtended.GetMapElements())
                    {
                        this.FilesMap.MapElements.Add(mapEle);
                    }

                    if (activityExtended.HasGps)
                    {
                        defaultLocationInputs.Add(activityExtended.GetDefaultLocationWithWeights());
                    }
                }

                this.mergedActivity = await this.activityManager.GetMerged();
                foreach (var mapEle in this.mergedActivity.GetMapElements())
                {
                    this.MergedMap.MapElements.Add(mapEle);
                }

                var latitude = defaultLocationInputs.Sum(dp => dp.Item1 * dp.Item3) / defaultLocationInputs.Sum(dp => dp.Item3);
                var longitude = defaultLocationInputs.Sum(dp => dp.Item2 * dp.Item3) / defaultLocationInputs.Sum(dp => dp.Item3);
                
                // TODO: Set default map location based on inputs. Currently not working
                //MapControl.SetLocation(this.FilesMap, new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
                //{
                //    Latitude = latitude,
                //    Longitude = longitude
                //}));
                //this.FilesMap.ZoomLevel = 14;
            }
        }

        private void MergerPage_List(object sender, RoutedEventArgs e)
        {
            this.FilesListPanel.Visibility = Visibility.Visible;
            this.FilesMap.Visibility = Visibility.Collapsed;
            this.MergedMap.Visibility = Visibility.Collapsed;
            this.NextAppBarButton.Visibility = Visibility.Visible;
            this.SaveAppBarButton.Visibility = Visibility.Collapsed;
        }

        private void MergerPage_Map(object sender, RoutedEventArgs e)
        {
            this.FilesListPanel.Visibility = Visibility.Collapsed;
            this.FilesMap.Visibility = Visibility.Visible;
            this.MergedMap.Visibility = Visibility.Collapsed;
            this.NextAppBarButton.Visibility = Visibility.Visible;
            this.SaveAppBarButton.Visibility = Visibility.Collapsed;
        }

        private void MergerPage_Next(object sender, RoutedEventArgs e)
        {
            this.FilesListPanel.Visibility = Visibility.Collapsed;
            this.FilesMap.Visibility = Visibility.Collapsed;
            this.MergedMap.Visibility = Visibility.Visible;
            this.NextAppBarButton.Visibility = Visibility.Collapsed;
            this.SaveAppBarButton.Visibility = Visibility.Visible;
        }

        private async void MergerPage_Save(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation =
                PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("TCX file", new List<string>() { ".tcx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = $"{this.DataManager.GetKeys().OrderBy(k => k.FileName.Length).First().FileName}_Merged";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file


                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TrainingCenterDatabase));
                    serializer.Serialize(stream, this.mergedActivity.ToContract().TrainingCenterDatabase);
                };

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                var status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Show(new ToastContent
                    {
                        Visual = new ToastVisual
                        {
                            TitleText = new ToastText { Text = $"File {file.Name} was saved." }
                        }
                    });
                }
                else
                {
                    Show(new ToastContent
                    {
                        Visual = new ToastVisual
                        {
                            TitleText = new ToastText { Text = $"File {file.Name} couldn't be saved." }
                        }
                    });
                }
            }
        }

        private void Show(ToastContent content)
        {
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

        private void MergerPage_Clear(object sender, RoutedEventArgs e)
        {
            this.FilesList.Items.Clear();
            this.DataManager.Clear();
            this.FilesMap.MapElements.Clear();
        }
    }
}
