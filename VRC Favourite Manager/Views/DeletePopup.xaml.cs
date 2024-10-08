﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class DeletePopup : ContentDialog
    {
        public string _folderName;
        public DeletePopup(string folderName)
        {
            this.InitializeComponent();
            _folderName = folderName;

            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                this.DeleteWorlds.Text = "フォルダを削除";
                this.ConfirmButton.Content = "削除";
                this.ConfirmMessage.Text = "選択したフォルダを削除しますか？";
                this.ConfirmExplanation.Text = "選択したフォルダは削除されます。この操作は元に戻すことができません。";
            }
            else
            {
                this.DeleteWorlds.Text = "Delete Folder";
                this.ConfirmButton.Content = "Delete";
                this.ConfirmMessage.Text = "Do you want to remove the selected folder?";
                this.ConfirmExplanation.Text = "The selected folder will be removed. This action cannot be undone.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new DeletePopupViewModel();
            viewModel.Delete_Click(_folderName);
            this.Hide();
        }

    }
}