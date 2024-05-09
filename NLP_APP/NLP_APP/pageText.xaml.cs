using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NLP_APP.Persistence;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using Xamarin.Essentials;

namespace NLP_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pageText : ContentPage
    {
        string sUserName = "";

        public pageText(string pUsername = "")
        {
            sUserName = pUsername;
            InitializeComponent();
            //var assets = Application.Context.Assets;
            //inferenceInterface = new TensorFlowInferenceInterface(assets, "model.pb");
        }



        private async void cmdStart_ClickedAsync(object sender, EventArgs e)
        {
            //TensorflowClassifier tcc = new TensorflowClassifier();
            //DependencyService.Get<IClassify>().GetClassify("This is a beautiful day.");
            if (String.IsNullOrWhiteSpace(editor.Text))
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
            //await callService(editor.Text);
            cSentiment sen = new cSentiment();
            string sentiment = await sen.CheckSentence(editor.Text);
            DependencyService.Get<IShowMessage>().Show(sentiment, true);
            cDB db = new cDB();
            await db.LogActivity(sUserName, "text", sentiment, editor.Text);
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