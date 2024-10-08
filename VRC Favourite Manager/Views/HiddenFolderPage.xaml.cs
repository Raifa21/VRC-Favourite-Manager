// FolderPage.xaml.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using Serilog;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class HiddenFolderPage : Page
    {
        private HiddenFolderPageViewModel _viewModel => (HiddenFolderPageViewModel)this.DataContext;
        private List<WorldModel> selectedItems;


        public HiddenFolderPage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel;
            
            selectedItems = new List<WorldModel>();
            MultiClickGrid.Visibility = Visibility.Collapsed;


            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                this.MultiSelectButton.Content = "選択";
                this.MultiSelectButton_Cancel.Content = "キャンセル";
                this.FolderNameTextBlock.Text = "非表示ワールド";
            }
            else
            {
                this.MultiSelectButton.Content = "Select";
                this.MultiSelectButton_Cancel.Content = "Cancel";
                this.FolderNameTextBlock.Text = "Hidden Worlds";
            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is WorldModel selectedWorld)
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItems = MultiClickGrid.SelectedItems.Cast<WorldModel>().ToList();
        }

        private void GridView_ClearSelection(object sender, RoutedEventArgs e)
        {
            MultiClickGrid.SelectedItems.Clear();
            selectedItems.Clear();
        }


        private void ViewDetails_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Log.Information("ViewDetails_Click");

            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private void RestoreWorld_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("RestoreWorld_Click");
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                _viewModel.RestoreWorld(selectedWorld);
            }
        }

        private void MultiRestoreWorld_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("MultiRestoreWorld_Click");
            foreach (var item in selectedItems)
            {
                _viewModel.RestoreWorld(item);
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.Dispose();
        }

    }
}