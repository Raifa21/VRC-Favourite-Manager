// FolderPage.xaml.cs

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        public FolderPage()
        {
            this.InitializeComponent();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedWorld = e.ClickedItem as WorldModel;
            if (selectedWorld != null)
            {
                var dialog = new WorldDetailsPopup(selectedWorld);
                dialog.XamlRoot = this.XamlRoot;
                await dialog.ShowAsync();
            }
        }

        private void ViewDetails_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var selectedWorld = (sender as FrameworkElement).DataContext as WorldModel;
            if (selectedWorld != null)
            {
                // Handle viewing details for the selected world
            }
        }

        private void MoveToAnotherFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var selectedWorld = (sender as FrameworkElement).DataContext as WorldModel;
            if (selectedWorld != null)
            {
                // Handle moving the selected world to another folder
            }
        }

        private void Remove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var selectedWorld = (sender as FrameworkElement).DataContext as WorldModel;
            if (selectedWorld != null)
            {
                var viewModel = (MainViewModel)this.DataContext;
                viewModel.SelectedFolder.Worlds.Remove(selectedWorld);
            }
        }
    }
}