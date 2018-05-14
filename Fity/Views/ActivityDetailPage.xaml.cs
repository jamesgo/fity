using Fity.Models;
using Fity.Models.Navigation;
using Fity.Utils;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private async void ActivityDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.FilesMap.MapServiceToken = Constants.BingMapsKey;

            await this.DataManager.LoadCompleteAsync();
            this.FilesMap.Center = this.DataManager.Center;
            this.FilesMap.ZoomLevel = this.DataManager.GetZoomLevel(this.ContentContainer.ActualWidth, this.ContentContainer.ActualHeight);
            foreach (var mapEle in this.DataManager.MapElements)
            {
                this.FilesMap.MapElements.Add(mapEle);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is FileLaunchParameters fileLaunchParams)
            {
                this.DataManager = new MapDataManager(fileLaunchParams.Files.Select(f => ((StorageFile)f).ToGpsFileInfo()));
            }
            else if (e.Parameter is DetailsLaunchParameters detailsLaunchParams)
            {
                this.DataManager = new MapDataManager(detailsLaunchParams.Files);
            }
        }
    }
}
