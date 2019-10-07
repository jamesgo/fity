using Fity.Utils;
using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fity.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageLibraryPage : Page
    {
        public ManageLibraryPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await Singletons.FolderMonitor.InitializeAsync();
            foreach (var folder in Singletons.FolderMonitor.Folders)
            {
                this.FoldersList.Items.Add(folder.Path);
            }
        }

        private async void AddFolder(object sender, RoutedEventArgs e)
        {
            // Clear any previously returned files between iterations of this scenario
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                this.FoldersList.Items.Add(folder.Path);
                await Singletons.FolderMonitor.AddPath(folder.Path);
            }
        }

        private async void Clear(object sender, RoutedEventArgs e)
        {
            this.FoldersList.Items.Clear();
            await Singletons.FolderMonitor.Clear();
        }
    }
}
