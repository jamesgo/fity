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

            this.Items = new ObservableCollection<LibraryItem>()
            {
                new LibraryItem
                {
                    Width = 123,
                    Height = 123
                }
            };
            this.ListView.ItemsSource = this.Items;
            this.ListView.ItemClick += ItemControl_ItemClick;
        }

        public FolderMonitor FolderMonitor { get; }

        public ObservableCollection<LibraryItem> Items { get; }

        private void ItemControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as LibraryItem;
            if (item == null)
            {
                return;
            }

            //_wrapPanelCollection.Remove(item);
        }
    }

    public class LibraryItem
    {
        public double Width { get; set; }

        public double Height { get; set; }

    }

}
