using GpsTools.Data;
using GpsTools.Utils;
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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GpsTools.Views
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
                // Application now has read/write access to the picked file(s)
                foreach (StorageFile file in files)
                {
                    this.FilesList.Items.Add(file.Name);
                    this.DataManager.AddToSession(file.ToGpsFileInfo());
                }
            }
            else
            {
                this.FilesList.Items.Clear();
            }
        }

        private void MergerPage_BulkDelete(object sender, RoutedEventArgs e)
        {

        }

        private void MergerPage_Next(object sender, RoutedEventArgs e)
        {
            if (!this.DataManager.IsLoadComplete)
            {
                return;
            }
        }

        private void MergerPage_Clear(object sender, RoutedEventArgs e)
        {
            this.FilesList.Items.Clear();
            this.DataManager.Clear();
        }
    }
}
