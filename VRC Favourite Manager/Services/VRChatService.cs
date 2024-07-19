﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using VRC_Favourite_Manager.Models;
using System.Diagnostics;
using System.Net;
using Windows.Media.Protection.PlayReady;
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.Services
{
    public class VRChatService
    {
        //is not readonly as config can be re-assigned
        private readonly Configuration _config;
        private readonly ApiClient client;
        private AuthenticationApi authApi;
        private UsersApi userApi;
        private WorldsApi worldsApi;
        private ApiResponse<CurrentUser> response;
        public bool RequiresEmailotp { get; private set; }


        public VRChatService()
        {
            _config = new Configuration();
            _config.UserAgent = "VRC Favourite Manager/0.0.1 Raifa";

            client = new ApiClient();
        }


        public void SetAPIKey(string apiKey)
        {
            _config.AddApiKey("auth" , apiKey);
        }

        public ApiResponse<VerifyAuthTokenResult> CheckAuthentication()
        {
            authApi = new AuthenticationApi(client, client, _config);
            return authApi.VerifyAuthTokenWithHttpInfo();
        }
        
        /// <summary>
        /// Sets the login and password for the user. 
        /// </summary>
        public bool Login(string username, string password)
        {
            _config.Username = username;
            _config.Password = password;

            authApi = new AuthenticationApi(client, client, _config);

            try
            {
                response = authApi.GetCurrentUserWithHttpInfo();
                if (RequiresEmail2FA(response))
                {
                    RequiresEmailotp = true;
                    return true;
                }
                else
                {
                    RequiresEmailotp = false;
                    return true;
                }


            }
            catch (ApiException e)
            {
                throw new VRCIncorrectCredentialsException();
            }
        }


        private static bool RequiresEmail2FA(ApiResponse<CurrentUser> resp)
        {
            // We can just use a super simple string.Contains() check
            if (resp.RawContent.Contains("emailOtp"))
            {
                return true;
            }

            return false;
        }

        public Verify2FAEmailCodeResult VerifyEmail2FA(string twoFactorAuthCode)
        {
            try
            {
                return authApi.Verify2FAEmailCode(new TwoFactorEmailCode(twoFactorAuthCode));
            }
            catch (ApiException e)
            {
                throw new VRCIncorrectCredentialsException();
            }
        }
        public Verify2FAResult Verify2FA(string twoFactorAuthCode)
        {
            try
            {
                return authApi.Verify2FA(new TwoFactorAuthCode(twoFactorAuthCode));
            }
            catch (ApiException e)
            {
                throw new VRCIncorrectCredentialsException();
            }
        }

        public bool confirmLogin()
        {
            response = authApi.GetCurrentUserWithHttpInfo();
            return response.StatusCode == HttpStatusCode.Accepted;
        }
        public void StoreAuth()
        {
            var configManager = new ConfigManager();
            try
            {
                configManager.WriteConfig("auth = \"" + _config.ApiKey + "\"");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing API key to config file: " + e.Message);
            }
        }



        /// <summary>
        /// Called initially when the user does not have any saved worlds. Looks through all favourited worlds and returns them.
        /// 
        /// </summary>
        /// <returns>A list of worlds, each containing info about itself</returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in</exception>
        /// <exception cref="VRCAPIException">When the API does not return data as expected.</exception>
        public async Task<List<Models.WorldModel>> GetAllFavoriteWorldsAsync()
        {
            try
            {
                int pageSize = 100;
                int currentPage = 0;
                var apiInstance = new WorldsApi(client, client, _config);
                bool hasNext = true;
                List<LimitedWorld> worlds = new List<LimitedWorld>();
                while (hasNext)
                {
                    var tempFavoriteList =
                        await apiInstance.GetFavoritedWorldsAsync(null, null, pageSize, null, pageSize * currentPage);
                    worlds.AddRange(tempFavoriteList);
                    if (tempFavoriteList.Count < pageSize)
                    {
                        hasNext = false;
                    }
                    currentPage++;
                }

                List<Models.WorldModel> favoriteWorlds = new List<Models.WorldModel>();
                foreach (var world in worlds)
                {
                    favoriteWorlds.Add(new WorldModel
                    {
                        ImageUrl = world.ImageUrl,
                        Name = world.Name,
                        AuthorName = world.AuthorName,
                        RecommendedCapacity = world.RecommendedCapacity,
                        Capacity = world.Capacity
                    });

                }
                return favoriteWorlds;
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                throw new VRCNotLoggedInException();
            }
            catch (ApiException ex)
            {
                Console.WriteLine("Exception when calling API: {0}", ex.Message);
                throw new VRCAPIException();
            }
        }
        /// <summary>
        ///  Returns the 60 most recently favourited worlds. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="VRCNotLoggedInException">When the user is not logged in</exception>
        /// <exception cref="VRCAPIException">When the API does not return data as expected.</exception>
        public async Task<List<Models.WorldModel>> GetFavoriteWorldsAsync()
        {
            try
            {
                var apiInstance = new WorldsApi(client, client, _config);
                var worlds = await apiInstance.GetFavoritedWorldsAsync();
                List<Models.WorldModel> favoriteWorlds = new List<Models.WorldModel>();
                foreach (var world in worlds)
                {
                    favoriteWorlds.Add(new WorldModel
                    {
                        ImageUrl = world.ImageUrl,
                        Name = world.Name,
                        AuthorName = world.AuthorName,
                        RecommendedCapacity = world.RecommendedCapacity,
                        Capacity = world.Capacity
                    });

                }
                return favoriteWorlds;
            }
            catch (ApiException ex) when (ex.ErrorCode == 401)
            {
                throw new VRCNotLoggedInException();
            }
            catch (ApiException ex)
            {
                Console.WriteLine("Exception when calling API: {0}", ex.Message);
                throw new VRCAPIException();
            }
        }
    }
}
