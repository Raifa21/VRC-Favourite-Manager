using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class RemovePopup : ContentDialog
    {

        public RemovePopup(WorldModel selectedWorld, string folderName)
        {
            this.InitializeComponent();
            this.DataContext = new RemovePopupViewModel(selectedWorld, folderName);

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "�폜";
                this.DeleteWorlds.Text = "���[���h���폜";
                this.SelectedWorld.Text = "�I�����ꂽ���[���h�F " + selectedWorld.WorldName;
                this.ConfirmMessage.Text = "�I���������[���h���t�H���_����폜���܂����H";
                this.ConfirmExplanation.Text = "�폜���ꂽ���[���h�́u�����ށv�t�H���_�Ɉړ�����܂��B�����ރt�H���_����폜���ꂽ���[���h�͐ݒ聄����A�N�Z�X�ł��܂��B";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Delete Worlds";
                this.SelectedWorld.Text = "Selected World: " + selectedWorld.WorldName;
                this.ConfirmMessage.Text = "Do you want to remove the selected world from the folder?";
                this.ConfirmExplanation.Text = "Removed worlds will be moved to the 'Unclassified' folder. Worlds removed from the 'Unclassified' folder can be accessed from Settings > Trash Bin.";
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