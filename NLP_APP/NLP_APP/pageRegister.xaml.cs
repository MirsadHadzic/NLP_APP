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
    public partial class pageRegister : ContentPage
    {
        public pageRegister()
        {
            InitializeComponent();
        }

        private async void cmdRegister_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUser.Text) || String.IsNullOrWhiteSpace(txtPass.Text))
            {
                DependencyService.Get<IShowMessage>().Show("empty username or pass...", true);
                return;
            }
            cDB db = new cDB();
            string sRes = await db.NewUser(txtUser.Text, txtPass.Text);
            if (String.IsNullOrEmpty(sRes))
            {
                DependencyService.Get<IShowMessage>().Show($"User registered: {txtUser.Text}", true);
                await Navigation.PopModalAsync();
            }
            else
                DependencyService.Get<IShowMessage>().Show($"Error: {sRes}", true);
        }

        private async void cmdCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}