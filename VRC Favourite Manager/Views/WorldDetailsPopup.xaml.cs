// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class WorldDetailsPopup : ContentDialog
    {
        public WorldModel World { get; set; }
        private string _selectedInstanceType;
        private string _selectedRegion;
        private WorldDetailsPopupViewModel _viewmodel;

        public WorldDetailsPopup(WorldModel world)
        {
            this.InitializeComponent();
            _viewmodel = new WorldDetailsPopupViewModel(world);
            this.World = world;
            this.DataContext = _viewmodel;
            
            _selectedInstanceType = "Public";
            _selectedRegion = "JP";

            

            string languageCode = Application.Current.Resources["languageCode"] as string;
            if (languageCode == "ja")
            {
                this.CreateInstance.Text = "�C���X�^���X�����";
                this.InstanceType.Text = "�C���X�^���X�^�C�v";
                this.Region.Text = "�n��";
                this.CreateInstanceButton.Content = "�쐬";
                this.Description.Text = "���̃��[���h�ɂ���";
                this.Details.Text = "�ڍ�";
                this.Visits.Text = "�K�␔";
                this.Favorites.Text = "���C�ɓ��萔";
                this.Capacity.Text = "�l�����";
                this.LastUpdated.Text = "�X�V��";
            }
            else
            {
                this.CreateInstance.Text = "Create Instance";
                this.InstanceType.Text = "Instance Type";
                this.Region.Text = "Region";
                this.CreateInstanceButton.Content = "Create";
                this.Description.Text = "About this world";
                this.Details.Text = "Details";
                this.Visits.Text = "Visits";
                this.Favorites.Text = "Favorites";
                this.Capacity.Text = "Capacity";
                this.LastUpdated.Text = "Last Updated";
            }

        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void CreateInstanceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            
            if (_selectedInstanceType != "Group")
            {
                _viewmodel.CreateInstanceAsync(World.WorldId, _selectedInstanceType, _selectedRegion);
                this.Hide();
            }
            else
            { 
                this.Hide();

                var createGroupInstance = new CreateGroupInstancePopup(World, _selectedRegion);
                createGroupInstance.XamlRoot = this.XamlRoot;
                createGroupInstance.ShowAsync();
            }
        }
        private void AuthorLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = $"https://vrchat.com/home/user/{World.AuthorId}";
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
        private void WorldLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = $"https://vrchat.com/home/world/{World.WorldId}";
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
        private void InstanceType_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedInstanceType = radioButton.Content.ToString();
                _selectedInstanceType = selectedInstanceType;
            }
        }
        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                _selectedRegion = selectedRegion;
            }
        }
    }
}