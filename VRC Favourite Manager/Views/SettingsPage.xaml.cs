using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Documents;
using VRC_Favourite_Manager.Common;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            string languageCode = Application.Current.Resources["languageCode"] as string;

            RefreshPage(languageCode);
        }

        private void Language_Checked(object sender, RoutedEventArgs e)
        {
            var _configManager = new ConfigManager();
            if (sender is RadioButton radioButton)
            {
                string languageCode = string.Empty;

                switch (radioButton.Tag.ToString())
                {
                    case "Japanese":
                        languageCode = "ja";
                        break;
                    case "English":
                        languageCode = "en";
                        break;
                }

                if (!string.IsNullOrEmpty(languageCode))
                {
                    ChangeApplicationLanguage(languageCode);
                }
            }

        }

        private async void RefreshPage(string languageCode)
        {
            if (this.SettingsTitle == null)
            {
                await Task.Delay(100);
            }
            if (languageCode == "ja")
            {
                this.SettingsTitle.Text = "�ݒ�";
                this.LanguageTitle.Text = "����";
                this.JapaneseRadioButton.Content = "���{��";
                this.EnglishRadioButton.Content = "�p��";
                this.JapaneseRadioButton.IsChecked = true;
                this.EnglishRadioButton.IsChecked = false;
                this.LookingForTranslators.Text = "������̑Ή��͌���ǉ��\��ł��B�Ή�����̗v�]��";
                this.HyperlinkText.Text = "������";
                this.WorldManagementTitle.Text = "���[���h�Ǘ�";
                this.HiddenFolder.Content = "��\���t�H���_";
                this.ResetButton.Content = "���Z�b�g";
            }
            else
            {
                this.SettingsTitle.Text = "Settings";
                this.LanguageTitle.Text = "Language";
                this.JapaneseRadioButton.Content = "Japanese";
                this.EnglishRadioButton.Content = "English";
                this.EnglishRadioButton.IsChecked = true;
                this.JapaneseRadioButton.IsChecked = false;
                this.LookingForTranslators.Text = "Support for other languages will be added later. Requests for supported languages can be made";
                this.HyperlinkText.Text = "here.";
                this.WorldManagementTitle.Text = "Manage Worlds";
                this.HiddenFolder.Content = "Hidden Folder"; 
                this.ResetButton.Content = "Reset";
            }
        }

        private void ChangeApplicationLanguage(string languageCode)
        { 
            Application.Current.Resources["languageCode"] = languageCode;
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
            RefreshPage(languageCode);
        }

        private void HiddenFolder_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HiddenFolderPage));
        }

        private void ResetButton_Clicked(object sender, RoutedEventArgs e)
        {
            var resetPopup = new ResetPopup();
            resetPopup.XamlRoot = this.Content.XamlRoot;
            resetPopup.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Unregister the message subscription
            WeakReferenceMessenger.Default.Unregister<LanguageChangedMessage>(this);
        }

        private void Hyperlink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(new Uri("https://forms.gle/vDGEFjz9PQJaHbxk8"));
        }
    }
}
