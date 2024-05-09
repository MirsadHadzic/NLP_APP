using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using HtmlAgilityPack;
using NLP_APP.Persistence;

namespace NLP_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pageUrl : ContentPage
    {
        string sUserName = "";
        public pageUrl(string pUsername = "")
        {
            sUserName = pUsername;
            InitializeComponent();
        }

        private async void cmdStart_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUrl.Text))
            {
                DependencyService.Get<IShowMessage>().Show("Nothing to analyse...", true);
                return;
            }

            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                DependencyService.Get<IShowMessage>().Show("No internet connection...", true);
                return;
            }
 
            string sTxt = String.Empty;

            // Create an instance of HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the specified URL
                    HttpResponseMessage response = await client.GetAsync(txtUrl.Text);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content as a string
                        string content = await response.Content.ReadAsStringAsync();

                        // Load the content into HtmlDocument
                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(content);

                        // Extract visible text from the HTML document
                        sTxt = ExtractVisibleText(htmlDoc.DocumentNode);

                        // Output the visible text to the console
                        //Console.WriteLine(sTxt);
                    }
                    else
                    {
                        //Console.WriteLine("Failed to retrieve content. Status code: " + response.StatusCode);
                        DependencyService.Get<IShowMessage>().Show("Failed to retrieve content. Status code: " + response.StatusCode, true);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            //await callService(sTxt);
            cSentiment sen = new cSentiment();
            string sentiment = await sen.CheckSentence(sTxt);
            DependencyService.Get<IShowMessage>().Show(sentiment, true);
            cDB db = new cDB();
            await db.LogActivity(sUserName, "url", sentiment, txtUrl.Text);
        }

        static string ExtractVisibleText(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return node.InnerText;
            }

            if (node.NodeType == HtmlNodeType.Element && node.Name == "script")
            {
                return ""; // Ignore script content
            }

            // Recursively process child nodes
            return string.Join(" ", node.ChildNodes.Select(ExtractVisibleText).Where(text => !string.IsNullOrWhiteSpace(text)));
        }
        /*
        async public Task<string> callService(string sToEvaluate)
        {
            try
            {
                var client = new HttpClient();
                string sCon = "[\r\n    {\r\n        \"id\": \"1\",\r\n        \"language\": \"en\",\r\n        \"text\": \"###_###_###\"\r\n    }\r\n]";
                sCon = sCon.Replace("###_###_###", sToEvaluate.Replace("\"", ""));
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new System.Uri("https://sentiment-analysis9.p.rapidapi.com/sentiment"),
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
                    string sResult = $"{jaRes[0]["prediction"].Value<string>()} with probability {jaRes[0]["probability"].Value<string>()}";
                    DependencyService.Get<IShowMessage>().Show(sResult, true);
                }
            }
            catch (Exception ex)
            {
                string sMess = "Error: " + ex.Message;
                if (ex.InnerException != null)
                    sMess += $" [{ex.InnerException.Message}]";
                DependencyService.Get<IShowMessage>().Show(sMess, true);
            }

            return "";
        }
        */
    }
}