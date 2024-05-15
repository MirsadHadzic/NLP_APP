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
    public partial class pageMenu : ContentPage
    {
        string sUserName = "";

        public pageMenu(string sLoggedUser = "")
        {
            sUserName = sLoggedUser;
            InitializeComponent();
        }

        // Event handler for the back button click
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void cmdTxt_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pageText(sUserName));
        }

        private void cmdUrl_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pageUrl(sUserName));
        }

        private void cmdFile_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pageFile(sUserName));
        }

        private void cmdAudio_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pageAudio(sUserName));
        }

        private void cmdHistory_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new pageHistory(sUserName));
        }
    }
}