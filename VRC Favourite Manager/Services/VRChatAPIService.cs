﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading;
using VRC_Favourite_Manager.Common;
using System.Text.Json;
using Tomlyn;
using Windows.Media.Protection.PlayReady;

namespace VRC_Favourite_Manager.Services
{
    public interface IVRChatAPIService
    {
        Task<bool> VerifyAuthTokenAsync(string authToken, string twoFactorAuthToken);
        Task<bool> VerifyLoginAsync(string username, string password);
        Task<bool> Authenticate2FAAsync(string twoFactorCode, string twoFactorAuthType);
        Task<bool> LogoutAsync();
        Task<List<Models.WorldModel>> GetFavoriteWorldsAsync(int n, int offset);
        Task<string> CreateInstanceAsync(string worldId, string instanceType);
    }
    public class VRChatAPIService : IVRChatAPIService
    {
        private HttpClient _Client;

        private CookieContainer _cookieContainer;

        private string _authToken;

        private string _twoFactorAuthToken;


        public VRChatAPIService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true,

            };
            _Client = new HttpClient(handler);
        }

        /// <summary>
        /// Verifies if the user has provided the correct auth token.
        /// <param name="authToken">The authentication token.</param>
        /// <param name="twoFactorAuthToken">The two factor authentication token.</param>
        /// <returns>Returns if the auth token is valid or not.</returns>
        public async Task<bool> VerifyAuthTokenAsync(string authToken, string twoFactorAuthToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
            request.Headers.Add("Cookie", $"auth={authToken};twoFactorAuth={twoFactorAuthToken}");
            var response = await _Client.SendAsync(request);


            response.EnsureSuccessStatusCode();
            Debug.WriteLine("Status Code: " + response.StatusCode);
            // Check if verification was successful.
            var responseString = await response.Content.ReadAsStringAsync();

            // Deserialize the response string to a JSON object.
            var authResponse = JsonSerializer.Deserialize<Models.VerifyAuthTokenResponse>(responseString);
            if (authResponse.ok)
            {
                _authToken = authToken;
                _twoFactorAuthToken = twoFactorAuthToken;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the user has successfully logged in with the auth token.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="twoFactorAuthToken"></param>
        /// <returns>The user's display name</returns>
        /// <exception cref="VRCIncorrectCredentialsException"></exception>
        public async Task<bool> VerifyLoginWithAuthTokenAsync(string authToken, string twoFactorAuthToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth/user?");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={authToken};twoFactorAuth={twoFactorAuthToken}");
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Error: " + e.Message);
                throw new VRCIncorrectCredentialsException();
            }
        }


        /// <summary>
        /// Verifies if the user has provided the correct login credentials.
        /// If valid, the auth token is stored in the response header. This is passed onto StoreAuthToken to be stored.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>If the user has successfully logged in or not</returns>
        /// <exception cref="VRCRequiresTwoFactorAuthException">User requires 2FA, contains a TwoFactorAuthType of either "email" or "default".</exception>
        public async Task<bool> VerifyLoginAsync(string username, string password)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://vrchat.com/api/1/auth/user?");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
                request.Headers.Add("Authorization", CreateAuthString(username, password));
                Debug.WriteLine(request.RequestUri);
                if (!string.IsNullOrEmpty(_twoFactorAuthToken))
                {
                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
                    }
                    else
                    {
                        request.Headers.Add("Cookie", $"twoFactorAuth={_twoFactorAuthToken}");
                    }
                }

                var response = await _Client.SendAsync(request);


                Console.WriteLine($"Status Code: {response.StatusCode}");

                // Output the response headers
                Console.WriteLine("Headers:");
                foreach (var header in response.Headers)
                {
                    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                // Output the response content
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Content:");
                Console.WriteLine(responseContent);


                // This throws a HttpRequestException if the status code is not a success code.
                response.EnsureSuccessStatusCode();

                // Pass the header to store the auth token.
                StoreAuthToken(response.Headers);

                // Check if the user requires a certain 2FA method. 
                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString.Contains("emailOtp"))
                {
                    throw new VRCRequiresTwoFactorAuthException("email");
                }
                else if (responseString.Contains("totp"))
                {
                    throw new VRCRequiresTwoFactorAuthException("default");
                }
                else {
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Error: " + e.Message);
                throw new VRCIncorrectCredentialsException();
            }
        }

        /// <summary>
        /// Generates a auth string according to VRC's API requirements.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The encoded authorization string.</returns>
        private static string CreateAuthString(string username, string password)
        {
            var encodedUsername = WebUtility.UrlEncode(username);
            var encodedPassword = WebUtility.UrlEncode(password);
            var authString = $"{encodedUsername}:{encodedPassword}";
            var base64AuthString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            Debug.WriteLine($"Basic {base64AuthString}");
            return $"Basic {base64AuthString}";
        }


        public async Task<bool> Authenticate2FAAsync(string twoFactorCode, string twoFactorAuthType)
        {
            try
            {
                HttpRequestMessage request;
                if (twoFactorAuthType == "email")
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/auth/twofactorauth/emailotp/verify");
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/auth/twofactorauth/totp/verify");
                }
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken}");
                var content = new StringContent($"{{\n  \"code\": \"{twoFactorCode}\"\n}}", Encoding.UTF8, "application/json");

                request.Content = content;
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Pass the header to store the auth token.
                StoreAuthToken(response.Headers);

                return true;
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("Error: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Retrieves and stores the auth token or the two factor auth token from the response header.
        /// </summary>
        /// <param name="headers">The response header obtained from /auth/user </param>
        private void StoreAuthToken(HttpResponseHeaders headers)
        {
            var configManager = new ConfigManager();
            if (headers.TryGetValues("set-cookie", out var authValues))
            {
                var authToken = authValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(authToken))
                {
                    if (authToken.Contains("twoFactorAuth"))
                    {
                        var twoFactorAuthToken = authToken.Split(';')[0];
                        twoFactorAuthToken = twoFactorAuthToken.Replace("twoFactorAuth=", "");
                        _twoFactorAuthToken = twoFactorAuthToken;
                        configManager.WriteToConfig("twoFactorAuth", twoFactorAuthToken);
                    }
                    else if (authToken.Contains("auth"))
                    {
                        authToken = authToken.Split(';')[0];
                        authToken = authToken.Replace("auth=", "");
                        _authToken = authToken;
                        configManager.WriteToConfig("auth", authToken);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the authentication cookie.
        /// </summary>
        /// <returns>If the user has successfully logged out or not.</returns>
        public async Task<bool> LogoutAsync()
        {
            ConfigManager configManager = new ConfigManager();
            var request = new HttpRequestMessage(HttpMethod.Put, "https://vrchat.com/api/1/logout");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
            request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Deserialize the response string to a JSON object.
            var authResponse = JsonSerializer.Deserialize<Models.LogoutResponse>(responseString);
            configManager.Logout();
            Debug.WriteLine(authResponse.message);

            return authResponse.message == "Ok!";
        }

        /// <summary>
        /// Obtains the user's favorite worlds. 
        /// </summary>
        /// <param name="n">The number of worlds to return. This application will default to 100.</param>
        /// <param name="offset">Offset to allow pagination of worlds.</param>
        /// <returns>A list of worlds, sorted by date added to favorites. </returns>
        public async Task<List<Models.WorldModel>> GetFavoriteWorldsAsync(int n, int offset)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://vrchat.com/api/1/worlds/favorites?n={n}&offset={offset}");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
            request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
            Debug.WriteLine(request.RequestUri);
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseString);
            try
            {
                var responseWorlds =
                    JsonSerializer.Deserialize<List<Models.ListFavoriteWorldsResponse>>(responseString);
                var worldModels = new List<Models.WorldModel>();
                foreach (var world in responseWorlds)
                {
                    var worldModel = new Models.WorldModel
                    {
                        ThumbnailImageUrl = world.ThumbnailImageUrl,
                        WorldName = world.Name,
                        WorldId = world.Id,
                        AuthorName = world.AuthorName,
                        AuthorId = world.AuthorId,
                        Capacity = world.Capacity,
                        LastUpdate = world.UpdatedAt.ToString(CultureInfo.InvariantCulture)?[..10],
                        Description = world.Description,
                        Visits = world.Visits,
                        Favorites = world.Favorites,
                    };
                    worldModels.Add(worldModel);
                }

                return worldModels;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Deserialization error: {ex.Message}");
                return null;
            }

        }

        /// <summary>
        /// Creates an instance of a world, and creates an invite for the user.
        /// </summary>
        /// <param name="worldId">An ID which represents the world to be created.</param>
        /// <param name="instanceType">The instance type of the instance being created. Allowed parameters are: "public","friends+","friends","invite+","invite".</param>
        /// <returns>Returns the instanceId of the instance which was created.</returns>
        /// <exception cref="VRCNotLoggedInException">When the authentication tokens fails to authenticate the user.</exception>
        public async Task<string> CreateInstanceAsync(string worldId, string instanceType)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/instances");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
                request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");

                string type;
                bool canRequestInvite;
                bool inviteOnly;
                switch (instanceType)
                {
                    case "public":
                        type = "public";
                        canRequestInvite = false;
                        inviteOnly = false;
                        break;
                    case "friends+":
                        type = "hidden";
                        canRequestInvite = false;
                        inviteOnly = false;
                        break;
                    case "friends":
                        type = "friends";
                        canRequestInvite = false;
                        inviteOnly = false;
                        break;
                    case "invite+":
                        type = "private";
                        canRequestInvite = true;
                        inviteOnly = false;
                        break;
                    default:
                        type = "private";
                        canRequestInvite = false;
                        inviteOnly = true;
                        break;
                }

                var content = new StringContent(
                    $"{{\n  \"worldId\": \"{worldId}\",\n  \"type\": \"{type}\",\n  \"region\": \"jp\",\n  \"ownerId\": \"<string>\",\n  \"queueEnabled\": false,\n  \"canRequestInvite\": {canRequestInvite},\n  \"inviteOnly\": {inviteOnly}\n}}",
                    null, "application/json");
                request.Content = content;
                var response = await _Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var createInstanceResponse = JsonSerializer.Deserialize<Models.CreateInstanceResponse>(responseString);

                InviteSelfAsync(createInstanceResponse.WorldId, createInstanceResponse.InstanceId);
                return createInstanceResponse.InstanceId;
            }
            catch (HttpRequestException)
            {
                throw new VRCNotLoggedInException();
            }
        }

        /// <summary>
        /// Invites the user to the instance provided.
        /// </summary>
        /// <param name="worldId"></param>
        /// <param name="instanceId"></param>
        /// <exception cref="VRCFailedToCreateInviteException">When the invite has failed to be created.</exception>
        private async void InviteSelfAsync(string worldId, string instanceId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://vrchat.com/api/1/instances/{worldId}:{instanceId}/invite");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "VRC Favourite Manager/dev 0.0.1 Raifa");
            request.Headers.Add("Cookie", $"auth={_authToken};twoFactorAuth={_twoFactorAuthToken}");
            var content =
                new StringContent(
                    $"{{\n  \"worldId\": \"{worldId}\",\n  \"instanceId\": \"{instanceId}\",\n  \"userId\": \"<string>\"\n}}",
                    null, "application/json");
            request.Content = content;
            var response = await _Client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var responseString = await response.Content.ReadAsStringAsync();
            var inviteResponse = JsonSerializer.Deserialize<Models.SendSelfInviteResponse>(responseString);
            if (inviteResponse.Status_code != 200)
            {
                throw new VRCFailedToCreateInviteException();
            }

        }
    }
}
