using Newtonsoft.Json.Linq;
using NLP_APP.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NLP_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class pageFile : ContentPage
    {
        string sUserName = "";

        FilePickerFileType customFileType =
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {               
               { DevicePlatform.Android, new[] { "text/plain" }  },
            });
        public pageFile(string pUsername = "")
        {
            sUserName = pUsername;
            InitializeComponent();
        }

        private async void cmdStart_Clicked(object sender, EventArgs e)
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a file to read..."
                , FileTypes = customFileType
            };
            var result = await FilePicker.PickAsync(options);
            if (result != null && result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
            {
                string fileContent = System.IO.File.ReadAllText(result.FullPath);

                fileContent = fileContent.Replace('\r', ' ');
                fileContent = fileContent.Replace('\n', ' ');
                cSentiment sen = new cSentiment();
                string sentiment = await sen.CheckSentence(fileContent);
                DependencyService.Get<IShowMessage>().Show(sentiment, true);
                cDB db = new cDB();
                await db.LogActivity(sUserName, "file", sentiment, result.FileName);
            }
            else // no file
            {
                DependencyService.Get<IShowMessage>().Show("Can't open file...", true);
            }
        }

        /*
        async public Task<double> callService(string sToEvaluate)
        {
            try
            {
                var client = new HttpClient();
                string sCon = "[\r\n    {\r\n        \"id\": \"1\",\r\n        \"language\": \"en\",\r\n        \"text\": \"###_###_###\"\r\n    }\r\n]";
                sCon = sCon.Replace("###_###_###", sToEvaluate.Replace("\"", ""));
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://sentiment-analysis9.p.rapidapi.com/sentiment"),
                    Headers =
                    {
                        { "Accept", "application/json" },
                        { "X-RapidAPI-Key", "ef35850716msh0b8a1e96446b449p14710ejsnd7416bc2b912" },
                        { "X-RapidAPI-Host", "sentiment-analysis9.p.rapidapi.com" },
                    },
                    Content = new StringContent(sCon)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    JArray jaBody = JArray.Parse(body);
                    JArray jaRes = (JArray)jaBody[0]["predictions"];
                    double fRes = jaRes[0]["probability"].Value<double>();
                    string sSign = jaRes[0]["prediction"].Value<string>();
                    if (sSign.Equals("negative"))
                        fRes += -1.0;
                    //string sResult = $"{jaRes[0]["prediction"].Value<string>()} with probability {jaRes[0]["probability"].Value<string>()}";
                    //DependencyService.Get<IShowMessage>().Show(sResult, true);
                    return fRes;
                }
            }
            catch (Exception ex)
            {
                string sMess = "Error: " + ex.Message;
                if (ex.InnerException != null)
                    sMess += $" [{ex.InnerException.Message}]";
                DependencyService.Get<IShowMessage>().Show(sMess, true);
            }

            return 0;
        }
        */
    }
}