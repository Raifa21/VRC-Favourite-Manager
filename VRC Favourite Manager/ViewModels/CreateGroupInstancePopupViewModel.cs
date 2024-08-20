﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class CreateGroupInstancePopupViewModel : ObservableObject
    {
        private readonly VRChatAPIService _vrChatApiService;
        private CancellationTokenSource _cancellationTokenSource;
        private WorldModel _selectedWorld;
        private string _region;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _showCancelButton;
        public bool ShowCancelButton
        {
            get => _showCancelButton;
            set => SetProperty(ref _showCancelButton, value);
        }

        private bool _isGroupRolesLoadingComplete;
        public bool IsGroupRolesLoadingComplete
        {
            get => _isGroupRolesLoadingComplete;
            set => SetProperty(ref _isGroupRolesLoadingComplete, value);
        }

        private bool _canCreateGroupInstance;
        public bool CanCreateGroupInstance
        {
            get => _canCreateGroupInstance;
            set => SetProperty(ref _canCreateGroupInstance, value);
        }

        private bool _canCreateRestricted;
        public bool CanCreateRestricted
        {
            get => _canCreateRestricted;
            set => SetProperty(ref _canCreateRestricted, value);
        }

        private bool _canCreateGroupOnly;

        public bool CanCreateGroupOnly
        {
            get => _canCreateGroupOnly;
            set => SetProperty(ref _canCreateGroupOnly, value);
        }

        private bool _canCreateGroupPlus;

        public bool CanCreateGroupPlus
        {
            get => _canCreateGroupPlus;
            set => SetProperty(ref _canCreateGroupPlus, value);
        }
        
        private bool _canCreateGroupPublic;

        public bool CanCreateGroupPublic
        {
            get => _canCreateGroupPublic;
            set => SetProperty(ref _canCreateGroupPublic, value);
        }



        private GroupModel _selectedGroup;
        private List<string> _userRoles;
        private string _groupAccessType;
        private List<string> _selectedRoles;
        private bool _isQueueEnabled;

        public ICommand CancelLoadingCommand { get; }

        public ObservableCollection<GroupModel> Groups { get; set; }

        public ObservableCollection<GroupRolesModel> GroupRoles { get; set; }

        public CreateGroupInstancePopupViewModel(WorldModel selectedWorld, string region)
        {
            _vrChatApiService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _selectedWorld = selectedWorld;
            _region = region;

            Groups = new ObservableCollection<GroupModel>();
            _selectedRoles = new List<string>();
            GetGroups();


            CanCreateGroupInstance = false;
            CancelLoadingCommand = new RelayCommand(CancelLoading);
        }

        private async void GetGroups()
        {
            IsLoading = true;
            ShowCancelButton = false;
            Message = "Loading...";

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                var delayTask = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                var groupsTask = _vrChatApiService.GetGroupsAsync();

                if (await Task.WhenAny(groupsTask, delayTask) == delayTask)
                {
                    Message = "Please use the official site. Loading is taking too long.";
                    ShowCancelButton = true;
                    return;
                }

                var groups = await groupsTask;

                if (groups == null || !groups.Any())
                {
                    Message = "You have no groups, please create or join a group to create a group instance.";
                }
                else
                {
                    Groups.Clear();
                    foreach (var group in groups)
                    {
                        Groups.Add(new GroupModel
                        {
                            Name = group.Name,
                            Id = group.GroupId,
                            Privacy = group.Privacy,
                            Icon = group.IconUrl,
                            GroupRoles = new List<GroupRolesModel>()
                        });
                    }
                    Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void CancelLoading()
        {
            _cancellationTokenSource?.Cancel();
            Message = "Loading canceled.";
            IsLoading = false;
        }

        public async void GroupSelected(string groupName)
        {
            IsGroupRolesLoadingComplete = false;
            IsLoading = true;
            ShowCancelButton = false;
            Message = "Loading group roles...";

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                _selectedGroup = Groups.FirstOrDefault(group => group.Name == groupName);
                GroupRoles = new ObservableCollection<GroupRolesModel>();
                foreach(var groupRole in _selectedGroup.GroupRoles)
                {
                    GroupRoles.Add(new GroupRolesModel
                    {
                        Name = groupRole.Name,
                        Id = groupRole.Id,
                        Permissions = groupRole.Permissions,
                        IsManagementRole = groupRole.IsManagementRole,
                        Order = groupRole.Order,
                        IsSelected = false
                    });
                }
                if (_selectedGroup == null)
                {
                    Message = "Group not found.";
                    return;
                }

                Debug.WriteLine($"Selected group: {_selectedGroup.Name}");
                Debug.WriteLine($"Selected group ID: {_selectedGroup.Id}");

                var delayTask = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                var groupRolesTask = _vrChatApiService.GetGroupRolesAsync(_selectedGroup.Id);
                var userRolesTask = _vrChatApiService.GetUserRoleAsync(_selectedGroup.Id);

                var allTasks = Task.WhenAll(groupRolesTask, userRolesTask);

                if (await Task.WhenAny(allTasks, delayTask) == delayTask)
                {
                    Message = "Please use the official site. Loading is taking too long.";
                    ShowCancelButton = true;
                    return;
                }

                _selectedGroup.GroupRoles = await groupRolesTask;
                _userRoles = await userRolesTask;

                UpdatePermissions();

                Message = string.Empty;
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                IsGroupRolesLoadingComplete = true;
                Debug.WriteLine("Group roles loaded.");
                Debug.WriteLine($"User roles: {_userRoles.Count}");
                Debug.WriteLine(CanCreateGroupInstance
                    ? "User can create group instance."
                    : "User cannot create group instance.");
                Debug.WriteLine(CanCreateRestricted
                    ? "User can create restricted group instance."
                    : "User cannot create restricted group instance.");
                Debug.WriteLine(CanCreateGroupOnly
                    ? "User can create group only group instance."
                    : "User cannot create group only group instance.");
                Debug.WriteLine(CanCreateGroupPlus
                    ? "User can create group plus group instance."
                    : "User cannot create group plus group instance.");
                Debug.WriteLine(CanCreateGroupPublic
                    ? "User can create public group instance."
                    : "User cannot create public group instance.");

            }
        }

        private void UpdatePermissions()
        {
            var permissions = new List<string>();
            foreach (var role in _userRoles)
            {
                var groupRole = _selectedGroup.GroupRoles.Find(gr => gr.Id == role);
                if (groupRole != null)
                {

                    foreach (var permission in groupRole.Permissions)
                    {
                        if (!permissions.Contains(permission))
                        {
                            permissions.Add(permission);
                        }
                    }
                }
            }
            foreach(var permission in permissions)
            {
                Debug.WriteLine($"Permission: {permission}");
            }

            CanCreateRestricted = permissions.Contains("group-instance-restricted-create") || permissions.Contains("*");
            CanCreateGroupOnly = permissions.Contains("group-instance-open-create") || permissions.Contains("*");
            CanCreateGroupPlus = permissions.Contains("group-instance-plus-create") || permissions.Contains("*");
            CanCreateGroupPublic = (permissions.Contains("group-instance-public-create") || permissions.Contains("*")) && _selectedGroup.Privacy == "default";

            CanCreateGroupInstance = (_canCreateGroupOnly || _canCreateGroupPlus || _canCreateGroupPublic || _canCreateRestricted) &&
                                     (permissions.Contains("group-instance-join") || permissions.Contains("*"));
        }

        public void AccessTypeSelected(string instanceType)
        {
            _groupAccessType = instanceType.ToLower();
        }

        public void RolesSelected(List<string> roles)
        {
            _selectedRoles = roles;
        }

        public async void CreateInstanceAsync()
        {
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _selectedGroup.Id, _region,
                _groupAccessType, _selectedRoles, _isQueueEnabled);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class GroupModel()
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Privacy { get; set; }
        public string Icon { get; set; }
        public List<GroupRolesModel> GroupRoles { get; set; }
    }

    public class GroupRolesModel()
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<string> Permissions { get; set; }

        public bool IsManagementRole { get; set; }

        public int Order { get; set; }

        public bool IsSelected { get; set; }
    }
}