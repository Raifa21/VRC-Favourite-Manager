// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class WorldDetailsPopup : ContentDialog
    {
        public WorldModel World { get; set; }

        public WorldDetailsPopup(WorldModel world)
        {
            this.InitializeComponent();
            this.World = world;
            this.DataContext = world;
        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void CreateInstanceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Handle instance creation logic here
            var world = (WorldModel)this.DataContext;
            // Implement your logic for creating an instance
        }
    }
}