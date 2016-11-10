using Fity.Data;
using Fity.Data.TCX;
using Fity.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
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

                    var gprx = await loader.Task;
                    var activityExtended = new GprxExtended(gprx);

                    foreach (var mapEle in activityExtended.GetMapElements())
                    {
                        this.FilesMap.MapElements.Add(mapEle);
                    }

                    if (activityExtended.HasGps())
                    {
                        defaultLocationInputs.Add(activityExtended.GetDefaultLocationWithWeights());
                    }
                }

                var latitude = defaultLocationInputs.Sum(dp => dp.Item1 * dp.Item3) / defaultLocationInputs.Sum(dp => dp.Item3);
                var longitude = defaultLocationInputs.Sum(dp => dp.Item2 * dp.Item3) / defaultLocationInputs.Sum(dp => dp.Item3);
                MapControl.SetLocation(this.FilesMap, new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition
                {
                    Latitude = latitude,
                    Longitude = longitude
                }));
                this.FilesMap.ZoomLevel = 14;
            }
        }

        private void MergerPage_List(object sender, RoutedEventArgs e)
        {
            this.FilesList.Visibility = Visibility.Visible;
            this.FilesMap.Visibility = Visibility.Collapsed;
        }

        private void MergerPage_Map(object sender, RoutedEventArgs e)
        {
            this.FilesList.Visibility = Visibility.Collapsed;
            this.FilesMap.Visibility = Visibility.Visible;
        }

        private void MergerPage_Clear(object sender, RoutedEventArgs e)
        {
            this.FilesList.Items.Clear();
            this.DataManager.Clear();
            this.FilesMap.MapElements.Clear();
        }
    }
}
