using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void NavigateToAuthenticationPage()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(AuthenticationPage));
        }
    }
}
