using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class ResetPopup : ContentDialog
    {
        
        public ResetPopup()
        {
            this.InitializeComponent();
            this.DataContext = new SettingsPageViewModel();

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "���Z�b�g";
                this.DeleteWorlds.Text = "�t�H���_�����Z�b�g";
                this.ConfirmMessage.Text = "���ׂẴf�[�^�����Z�b�g���܂����H";
                this.ConfirmMessage2.Text = "���̑���͎������܂���B";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Reset Folders";
                this.ConfirmMessage.Text = "Are you sure you want to reset?";
                this.ConfirmMessage2.Text = "This action cannot be undone.";
            }
        }


        private void CloseButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (SettingsPageViewModel)this.DataContext;
            viewModel.Reset();
            this.Hide();
        }
    }
}