using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class RemovePopup : ContentDialog
    {
        
        public RemovePopup(List<WorldModel> selectedWorlds, string folderName)
        {
            this.InitializeComponent();
            this.DataContext = new RemovePopupViewModel(selectedWorlds, folderName);
            var text = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "�I�����ꂽ���[���h�F " : "Selected Worlds: ";
            foreach (var selectedWorld in selectedWorlds)
            {
                text += selectedWorld.WorldName + ", ";
            }
            text = text.Substring(0, text.Length - 2);

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "�폜";
                this.DeleteWorlds.Text = "���[���h���폜";
                this.SelectedWorld.Text = text;
                this.ConfirmMessage.Text = "�I���������[���h���폜���܂����H";
                this.ConfirmExplanation.Text = "�I�����ꂽ���[���h�͂��ׂẴt�H���_����폜����܂��B�폜���ꂽ���[���h�͐ݒ聄��\������A�N�Z�X�ł��܂��B";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Delete Worlds";
                this.SelectedWorld.Text = text;
                this.ConfirmMessage.Text = "Do you want to remove the selected world(s)?";
                this.ConfirmExplanation.Text = "The selected world(s) will be removed from all folders. Removed worlds can be accessed from Settings > Hidden Worlds.";
            }
        }


        private void CloseButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (RemovePopupViewModel)this.DataContext;
            viewModel.RemoveFromFolder();
            this.Hide();
        }
    }
}