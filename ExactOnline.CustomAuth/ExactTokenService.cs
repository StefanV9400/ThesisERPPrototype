using ExactOnline.CustomAuth.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ExactOnline.CustomAuth
{
    public partial class ExactOnlineConnect
    {

        public class ExactTokenService
        {

            private string _url = Globals.TOKEN_ENDPOINT;
            private string _grantType = "refresh_token";
            private static readonly HttpClient _client = new HttpClient();

            /// <summary>
            /// Create a simple cookie
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="name"></param>
            /// <param name="item"></param>
            /// <param name="seconds"></param>
            /// <returns>HttpCookie</returns>
            //public System.Net.Http.HttpClient CreateCookie(string userId, string name, string item, double seconds)
            //{
            //    var cookie = new HttpCookie(userId);

            //    // Add value
            //    cookie.Values.Add(name, item);

            //    // set expiry date-time
            //    cookie.Expires = DateTime.Now.AddSeconds(seconds);

            //    // Write to the client
            //    return cookie;
            //}

            /// <summary>
            /// Get the value of the token stored in the web browser
            /// </summary>
            /// <param name="request"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public string GetTokenValue(HttpRequest request, string name)
            {
                var req = request.Cookies;
                var cookie = request.Cookies[name];

                if (cookie == null)
                {
                    return "";
                }

                return HttpUtility.UrlDecode(cookie);
            }

            /// <summary>
            /// Returns the cookie with the requested name
            /// </summary>
            /// <param name="request"></param>
            /// <param name="name"></param>
            /// <returns>HttpCookie</returns>
            public string GetCookie(HttpRequest request, string name)
            {
                return request.Cookies[name];
            }

            /// <summary>
            /// Check if token should be refreshed (TODO: now on 8 minutes)
            /// </summary>
            /// <param name="refreshExpirery"></param>
            /// <returns></returns>
            public bool ShouldRefreshToken(string refreshExpirery)
            {
                var time = DateTime.Parse(refreshExpirery);
                var timeNow = DateTime.Now;
                var result = time - timeNow;
                if (result.TotalSeconds < 0)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Check if the users should login again, it could be he is opening the browsers the next day
            /// </summary>
            /// <param name="date"></param>
            /// <returns></returns>
            public bool ShouldLoginAgain(DateTime date)
            {
                return date < DateTime.Now;
            }

            public async Task<HttpResponseMessage> RefreshTokens(string accesToken, string refreshToken, string clientId, string clientSecret)
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
            {

                    { "refresh_token", refreshToken },
                    { "grant_type", _grantType},
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
            };
                    var content = new FormUrlEncodedContent(values);
                    // client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accesToken);

                    return await client.PostAsync(_url, content);
                }
            }

            public async Task<TokenRefreshVM> SetRefreshValues(HttpResponseMessage response, Microsoft.AspNetCore.Http.HttpRequest request)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (json != null || !String.IsNullOrEmpty(json) || !String.IsNullOrWhiteSpace(json) || json != @"")
                {

                    var tokenResult = JObject.Parse(json);
                    var newToken = tokenResult["access_token"].Value<string>();
                    var newRefresh = tokenResult["refresh_token"].Value<string>();
                    var newExpirery = tokenResult["expires_in"].Value<Double>();

                    // Set cookies
                    //request.Cookies
                    //    .Append(Globals.EXACT_TOKEN, HttpUtility.UrlEncode(newToken), new CookieOptions()
                    //    {
                    //        Expires = DateTime.Now.AddMinutes(10),
                    //    });
                    //request.Cookies
                    //    .Append(Globals.EXACT_REFRESH_TOKEN, HttpUtility.UrlEncode(newRefresh), new CookieOptions()
                    //    {
                    //        Expires = DateTime.Now.AddMinutes(10),
                    //    });
                    //request.Cookies
                    //    .Append(Globals.EXACT_EXPIRING_TOKEN, DateTime.Now.AddSeconds(newExpirery).ToString(), new CookieOptions()
                    //    {
                    //        Expires = DateTime.Now.AddMinutes(10),
                    //    });

                    return new TokenRefreshVM
                    {
                        AccessToken = newToken,
                        RefreshToken = newRefresh,
                    };

                }
                return null;
            }


            public async Task<TokenRefreshVM> TokenCheck(string AccessToken, string RefreshToken, string ExpireryTime)
            {
                // Check values
                if (ShouldRefreshToken(ExpireryTime))
                {
                    var tokenResponse = await RefreshTokens(AccessToken, RefreshToken, Globals.CLIENT_ID, Globals.CLIENT_SECRET);
                    var message = tokenResponse.Content.ReadAsStringAsync();
                    if (tokenResponse.StatusCode == HttpStatusCode.BadRequest)
                        return null;

                    var json = await tokenResponse.Content.ReadAsStringAsync();
                    if (json != null || !String.IsNullOrEmpty(json) || !String.IsNullOrWhiteSpace(json) || json != @"")
                    {
                        // Get values
                        var tokenResult = JObject.Parse(json);
                        var newToken = tokenResult["access_token"].Value<string>();
                        var newRefresh = tokenResult["refresh_token"].Value<string>();
                        var newExpirery = tokenResult["expires_in"].Value<Double>();

                        return new TokenRefreshVM
                        {
                            AccessToken = newToken,
                            RefreshToken = newRefresh,
                            ExpireryTime = DateTime.Now.AddSeconds(newExpirery),
                        };
                    }
                }
                return null;
            }



        }

    }
}
