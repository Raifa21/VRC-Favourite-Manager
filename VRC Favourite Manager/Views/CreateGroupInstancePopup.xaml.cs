using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class CreateGroupInstancePopup : ContentDialog
    {
        public CreateGroupInstancePopup(WorldModel world, string region)
        {
            this.InitializeComponent();
            this.DataContext = new CreateGroupInstancePopupViewModel(world, region);

            if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.SelectGroupTitle.Text = "�O���[�v��I��";
                this.SelectGroupInstanceType.Text = "�O���[�v�C���X�^���X�^�C�v��I��";
                this.GroupTextSub_1.Text = "�I�����ꂽ���[���������[�U�[�ƍ����ł��܂�";
                this.GroupTextSub_2.Text = "�I�����ꂽ���[���������[�U�[�ƍ����ł��܂�";
                this.GroupPlusTextSub.Text = "�C���X�^���X���̃��[�U�[�̃t�����h�͒N�ł����邱�Ƃ��ł��܂�";
                this.GroupPublicTextSub.Text = "�N�ł����邱�Ƃ��ł��܂�";
                this.SelectRoles.Text = "���[����I��";
                this.Everyone.Text = "���ׂāi�N�ł��A�N�Z�X�\�j";
                this.GroupInstanceOverview.Text = "�O���[�v�C���X�^���X�����";
                this.GroupConfirm.Text = "�O���[�v";
                this.InstanceTypeConfirm.Text = "�C���X�^���X�^�C�v";
                this.RolesConfirm.Text = "���̃C���X�^���X�ɍ����ł��郍�[���F";
                this.InstanceQueueConfirm.Text = "�����̏ꍇ�̃C���X�^���X�ҋ@��";
                this.EnableInstanceQueue.Text = "�C���X�^���X�ҋ@��";
                this.CreateInstanceButton.Content = "�C���X�^���X�����";
            }
            else
            {
                this.SelectGroupTitle.Text = "Select Group";
                this.SelectGroupInstanceType.Text = "Select Group Instance Type";
                this.GroupTextSub_1.Text = "Only the selected group roles may join";
                this.GroupTextSub_2.Text = "Only the selected group roles may join";
                this.GroupPlusTextSub.Text = "Any Friend of a user in the instance may join";
                this.GroupPublicTextSub.Text = "Anyone can join";
                this.SelectRoles.Text = "Select Roles";
                this.Everyone.Text = "Everyone";
                this.GroupInstanceOverview.Text = "Create Group Instance";
                this.GroupConfirm.Text = "Group";
                this.InstanceTypeConfirm.Text = "Instance Type";
                this.RolesConfirm.Text = "Roles that can join this instance:";
                this.InstanceQueueConfirm.Text = "Instance Queue when full";
                this.EnableInstanceQueue.Text = "Enable Instance Queue";
                this.CreateInstanceButton.Content = "Create Instance";
            }
        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void GroupSelected(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.GroupSelected(button.CommandParameter as string);
                if (button.Name == "Restricted")
                {
                    viewModel.IsRoleRestricted = true;
                }
            }
        }

        private void SelectType(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.AccessTypeSelected(button.Name);
                viewModel.IsGroupRolesLoadingComplete = false;
            }
        }

        private void ShowInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = true;
        }

        private void HideInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = false;
        }

        private void SelectRolesChanged_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }
        private void SelectRolesChanged_Unchecked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }

        private void RolesSelected(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.RolesSelected();
        }

        private void InvertInstanceQueue(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.InvertInstanceQueue();
        }

        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                if(viewModel != null)
                {
                    viewModel.Region = selectedRegion;
                }
            }
        }
        private void CreateInstance(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.CreateInstanceAsync();

            this.Hide();
        }
    }
}
