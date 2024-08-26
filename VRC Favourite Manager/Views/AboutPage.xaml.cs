using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Documents;
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                CreatorTextBlock.Text = "�쐬��: Raifa";
                SupportThankYouTextBlock.Text = "���x�����肪�Ƃ��������܂��I�x����";
                hyperlinktext.Text = "������";
                SupportThankYouTextSuffix.Text = "����I";
                MITLicenseTextBlock.Text = "���̃A�v���P�[�V������MIT���C�Z���X�̉��Ń��C�Z���X����Ă��܂��B";
                ComplianceTextBlock.Text = "���̃A�v���P�[�V�������g�p���邱�ƂŁA���ׂĂ̓K�p����郉�C�Z���X�ɏ]�����Ƃɓ��ӂ�����̂Ƃ��܂��B";
                CCBYNCTextBlock.Text = "�ꕔ�̃R���|�[�l���g��CC-BY-NC-4.0���C�Z���X�̉��Ń��C�Z���X����Ă���A�񏤗p�ړI�݂̂Ŏg�p�ł��܂��B";
            }
            else
            {
                CreatorTextBlock.Text = "Created by: Raifa";
                SupportThankYouTextBlock.Text = "Thank you for your support! You can support me";
                hyperlinktext.Text = "here";
                SupportThankYouTextSuffix.Text = "";
                MITLicenseTextBlock.Text = "This application is licensed under the MIT License.";
                ComplianceTextBlock.Text =
                    "By using this application, you agree to comply with all applicable licenses.";
                CCBYNCTextBlock.Text =
                    "Some components are separately licensed under CC-BY-NC-4.0 and are for non-commercial use only.";

            }
        }
    }
}