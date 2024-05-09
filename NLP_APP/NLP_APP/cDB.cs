using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLP_APP
{
    class cDB
    {
        private string _sUrlBase = "";

        public async Task<string> LogActivity(string pUsername, string pType, string pResult, string pContent)
        {
            pType = pType.Substring(0, Math.Min(pType.Length, 5));
            pResult = pResult.Substring(0, Math.Min(pResult.Length, 50));
            pContent = pContent.Substring(0, Math.Min(pContent.Length, 2000));
            // https://myfp.lvbsx.com/NLP/log_action.php?user=Anes&type=Text&result=positive 0.23&content=nice day
            string sRes = await GetStringFromBHDB($"log_action.php?user={pUsername}&type={pType}&result={pResult}&content={pContent}");
            return sRes;
        }
        public async Task<string> LoginUser(string username, string pass)
        {
            string sRes = await GetStringFromBHDB($"login_user.php?user={username}&pass={pass}");
            return sRes;
        }

        public async Task<string> NewUser(string username, string pass)
        {
            string sRes = await GetStringFromBHDB($"new_user.php?user={username}&pass={pass}");
            return sRes;
        }

        private async Task<string> GetStringFromBHDB(string sUrlEnd)
        {
            sUrlEnd = sUrlEnd.Replace(' ', '+');
            string sUrl = $"{_sUrlBase}/{sUrlEnd}";
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Calling {sUrl}. * start");
#endif
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var _cancelTokenSource = new CancellationTokenSource();
                    var _cancelToken = _cancelTokenSource.Token;
                    var response = await client.GetAsync(sUrl);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"Calling {sUrl}. * end download error HttpStatusCode.NotFound");
#endif
                        return "Error";
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"Calling {sUrl}. * HttpStatusCode.OK");
#endif
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        try
                        {
                            return responseContent;
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine($"Calling {sUrl}. * end download error ex: {ex.Message}");
#endif
                            return $"Error: {ex.Message}";
                        }
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Downloading {sUrl}. * end download error e: {e.Message}");
#endif
                    return $"Error: {e.Message}"; 
                }

                return "Error";
            }
        }
    }
}
