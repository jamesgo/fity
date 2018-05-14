using Fity.Data;
using Fity.Utils.FolderMonitor;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace Fity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            this.InitializeComponent();
            this.FolderMonitor = new FolderMonitor(new string[] { @"C:\users\jamesg\Desktop\fd" });
            this.FolderMonitor.Files_Changed += FolderMonitor_Changed;

            this.ListView.ItemClick += ItemControl_ItemClick;
        }

        public FolderMonitor FolderMonitor { get; }

        private void FolderMonitor_Changed(object sender)
        {
            this.ListView.ItemsSource = this.FolderMonitor.GpsFiles.Values;
        }

        private void ItemControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Tcx;
            if (item == null)
            {
                return;
            }

            //_wrapPanelCollection.Remove(item);
        }
    }
}
