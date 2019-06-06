using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VERPS.WebApp.Database;

namespace ExactOnline.CustomAuth
{
    public partial class ExactOnlineConnect
    {
        public class ExactService
        {
            private string _accessToken;
            private string _refreshToken;
            private string _expireryTime;
            private string _userId;
            private ExactTokenService _service = new ExactTokenService();
            private VERPSDBContext _context;

            public VERPSDBContext Context
            {
                set { _context = value; }
            }

            public string UserID
            {
                set { _userId = value; }
            }

            public string AccessToken
            {
                set { _accessToken = value; }
            }

            public string RefreshToken
            {
                set { _refreshToken = value; }
            }

            private const string _baseURL = "https://start.exactonline.nl";
            private const string _latestVersion = "/api/v1/";
            private string _division;

            public string Division
            {
                set { _division = value + "/"; }
            }


            public async Task<T> Get<T>(string urlModification, bool needToken = true, bool isSingle = true)
            {
                var tokenContext = _context.ExactTokens
                        .FirstOrDefault(x => x.Token == _accessToken);

                if (tokenContext != null)
                {

                    var refResults = await _service.TokenCheck(tokenContext.Token, tokenContext.RefreshToken, tokenContext.RefreshTime.ToString());
                    if (refResults != null)
                    {
                        _accessToken = refResults.AccessToken;
                        _refreshToken = refResults.RefreshToken;

                        tokenContext.Token = _accessToken;
                        tokenContext.RefreshToken = _refreshToken;
                        tokenContext.RefreshTime = refResults.ExpireryTime;

                        _context.SaveChanges();
                    }
                }

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, _baseURL + _latestVersion + urlModification))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (needToken)
                        {
                            if (_accessToken == "")
                                return default(T);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                        }
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {

                            var output = response.Content.ReadAsStringAsync();
                            // TODO: REFRESHtOKEN CHECK
                            string json = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var res = JsonConvert.DeserializeObject<dynamic>(json);
                                var check = Convert.ToString(res.d["results"]);
                                if (check != "[]") // TEMP CHECK
                                {
                                    if (!isSingle)
                                    {
                                        var result = Convert.ToString(res.d["results"]);
                                        return JsonConvert.DeserializeObject<T>(result);
                                    }
                                    var ok = Convert.ToString(res.d["results"][0]);
                                    return JsonConvert.DeserializeObject<T>(ok);
                                }
                                return default(T);
                            }

                        }
                        return default(T);
                    }
                }
            }


            public async Task<T> PostNew<T>(string urlModification, bool needToken = true, bool isSingle = true, object data = null)
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _baseURL + _latestVersion + urlModification))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (needToken)
                        {
                            if (_accessToken == "")
                                return default(T);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                        }
                        var toJson = JsonConvert.SerializeObject(data);
                        var content = new StringContent(toJson, Encoding.UTF8, "application/json");
                        request.Content = content;

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {

                            var output = response.Content.ReadAsStringAsync();
                            // TODO: REFRESHtOKEN CHECK
                            string json = await response.Content.ReadAsStringAsync();
                            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                            {
                                //-----------------------------
                                // dit kan er uiteindelijk uit
                                // Debug op dit punt om te ontdekken hoe de werkelijke classe eruit ziet die opgehaald wordt//
                                var thi = JObject.Parse(json);
                                //-----------------------------

                                var res = JsonConvert.DeserializeObject<dynamic>(json);
                                var check = Convert.ToString(res.d);
                                if (check != "[]") // TEMP CHECK
                                {
                                    if (!isSingle)
                                    {
                                        var result = Convert.ToString(res.d);
                                        return JsonConvert.DeserializeObject<T>(result);
                                    }
                                    var ok = Convert.ToString(res.d);
                                    return JsonConvert.DeserializeObject<T>(ok);
                                }
                                return default(T);

                                //ErrorClass er = JsonConvert.DeserializeObject<ErrorClass>(json);
                                //Error = er.error;
                            }

                        }
                        return default(T);
                    }
                }
            }


            public async Task<HttpResponseMessage> Post(string urlModification, bool needToken = true, bool isSingle = true, object data = null)
            {
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _baseURL + _latestVersion + urlModification))
                    {
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (needToken)
                        {
                            if (_accessToken == "")
                                return null;
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                        }

                        var toJson = JsonConvert.SerializeObject(data);
                        var content = new StringContent(toJson, Encoding.UTF8, "application/json");
                        request.Content = content;

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            //   var header = response.Headers
                            var content222 = response.Content.ReadAsStringAsync();
                            return response;
                        }
                    }
                }
            }

        }

    }
}
