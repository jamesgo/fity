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
    public sealed partial class ActivityDetailPage : Page
    {
        private ActivityManager activityManager;
        private Session mergedActivity;

        public MapDataManager DataManager { get; private set; }

        public ActivityDetailPage()
        {
            this.InitializeComponent();
            this.Loaded += ActivityDetailPage_Loaded;
        }

        private void ActivityDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.FilesMap.MapServiceToken = Constants.BingMapsKey;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var fileLaunchParams = (FileLaunchParameters)e.Parameter;

            this.DataManager = new MapDataManager(fileLaunchParams.Files);
            await this.DataManager.LoadCompleteAsync();
            this.FilesMap.Center = this.DataManager.Center;
            this.FilesMap.ZoomLevel = this.DataManager.GetZoomLevel(this.ContentContainer.ActualWidth, this.ContentContainer.ActualHeight);
            foreach (var mapEle in this.DataManager.MapElements)
            {
                this.FilesMap.MapElements.Add(mapEle);
            }
        }
    }
}
