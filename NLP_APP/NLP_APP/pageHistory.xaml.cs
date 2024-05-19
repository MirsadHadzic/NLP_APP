using Newtonsoft.Json.Linq;
using NLP_APP.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NLP_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public class structHistory
    {
        public string sDateTime { get; set; }
        public string sType { get; set; }
        public string sResult { get; set; }
        public string sContext { get; set; }
    }

    public partial class pageHistory : ContentPage
    {
        private string sUserName = "";
        private List<structHistory> lstHistory = new List<structHistory>();
        public pageHistory(string pUserName)
        {
            InitializeComponent();
            sUserName = pUserName;
        }

        protected override async void OnAppearing()
        {
            try
            {
                // get json string from database
                cDB db = new cDB();
                string sJson = await db.GetHistory(sUserName);
                // json to list
                JArray ja = JArray.Parse(sJson);
                lstHistory.Clear();
                foreach (var jObj in ja)
                {
                    structHistory stNew = new structHistory();
                    stNew.sDateTime = jObj["dt"].Value<string>();
                    stNew.sContext = jObj["con"].Value<string>();
                    stNew.sResult = jObj["res"].Value<string>();
                    stNew.sType = jObj["type"].Value<string>();
                    lstHistory.Add(stNew);
                }
                lstHistory = lstHistory.OrderByDescending(x => DateTime.Parse(x.sDateTime)).ToList();

                listView.ItemsSource = null;
                listView.ItemsSource = lstHistory;
                // header
                lblInfo.Text = $"History for user: {sUserName}";
            }
            catch (Exception e)
            {
                DependencyService.Get<IShowMessage>().Show($"Error: {e.Message}", true);
            }
        }
    }
}