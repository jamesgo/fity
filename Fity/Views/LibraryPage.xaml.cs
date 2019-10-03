using Fity.Data;
using Fity.Models;
using Fity.Models.Navigation;
using Fity.Utils.FolderMonitor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fity.Views
{
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            this.InitializeComponent();
            this.FolderMonitor = new FolderMonitor(new string[] { @"C:\Users\james\OneDrive\Desktop\fd" });

            this.FolderMonitor.Files_Changed += FolderMonitor_Changed;

            this.ListView.ItemClick += ItemControl_ItemClick;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await this.FolderMonitor.InitializeAsync();
        }

        public FolderMonitor FolderMonitor { get; }

        private void Manage(object sender, object e)
        {
            //Frame rootFrame = Window.Current.Content as Frame;
            //var appShell = rootFrame.Content as AppShell;
            //appShell.AppFrame.Navigate(typeof(ManageLibraryPage));
        }

        private async void Refresh(object sender, object e)
        {
            await this.FolderMonitor.RefreshFilesAsync();
        }

        private void FolderMonitor_Changed(object sender)
        {
            this.ListView.ItemsSource = this.FolderMonitor.GpsFiles.Values;
        }

        private void ItemControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Session;
            if (item == null)
            {
                return;
            }

            Frame rootFrame = Window.Current.Content as Frame;
            var appShell = rootFrame.Content as AppShell;
            appShell.AppFrame.Navigate(typeof(ActivityDetailPage), new DetailsLaunchParameters
            {
                Files = new List<IGpsFileInfo> { this.FolderMonitor.SessionFiles[item] }
            });
        }

        //private async void rc_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        //{
        //    //Do some work to show new Content! Once the work is done, call RefreshCompletionDeferral.Complete()
        //    this.RefreshCompletionDeferral = args.GetDeferral();
        //    await this.FolderMonitor.RefreshFilesAsync();
        //}
    }
}
