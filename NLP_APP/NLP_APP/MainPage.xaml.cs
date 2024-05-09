using System;
using Xamarin.Forms;
using Plugin.GoogleClient;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
//using Acr.UserDialogs;
using Newtonsoft.Json;
//using Plugin.FacebookClient;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Xamarin.Forms;
using NLP_APP.Persistence;

namespace NLP_APP
{
    public partial class MainPage : ContentPage
    {
        string sLoggedUser = "";
        IGoogleClientManager _googleService = CrossGoogleClient.Current;
        /*
        async Task LoginGoogleAsync(AuthNetwork authNetwork)
        {
            try
            {
                if (!string.IsNullOrEmpty(_googleService.ActiveToken))
                {
                    //Always require user authentication
                    _googleService.Logout();
                }

                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = async (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    switch (e.Status)
                    {
                        case GoogleActionStatus.Completed:
#if DEBUG
                            var googleUserString = JsonConvert.SerializeObject(e.Data);
                            Debug.WriteLine($"Google Logged in succesfully: {googleUserString}");
#endif
                            var socialLoginData = new NetworkAuthData
                            {
                                Id = e.Data.Id,
                                Logo = authNetwork.Icon,
                                Foreground = authNetwork.Foreground,
                                Background = authNetwork.Background,
                                Picture = e.Data.Picture.AbsoluteUri,
                                Name = e.Data.Name,
                            };
                            await App.Current.MainPage.Navigation.PushModalAsync(new HomePage(socialLoginData));
                            break;
                        case GoogleActionStatus.Canceled:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Canceled", "Ok");
                            break;
                        case GoogleActionStatus.Error:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Error", "Ok");
                            break;
                        case GoogleActionStatus.Unauthorized:
                            await App.Current.MainPage.DisplayAlert("Google Auth", "Unauthorized", "Ok");
                            break;
                    }
                    _googleService.OnLogin -= userLoginDelegate;
                };

                _googleService.OnLogin += userLoginDelegate;

                await _googleService.LoginAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        */
        public MainPage()
        {
            InitializeComponent();
            //auth = DependencyService.Get<IFirebaseAuthentication>();
        }

        protected override void OnAppearing()
        {
            txtUser.Text = txtPass.Text = "";
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            //Navigation.PushModalAsync(new pageMenu());
            //await LoginGoogleAsync();
            /*
            // login
            cDB _db = new cDB();
            string sRes = await _db.LoginUser("Anes.Hadzic@gmail.com", "jedan1");
            if (!String.IsNullOrEmpty(sRes)) // login error
            {
                DependencyService.Get<IShowMessage>().Show(sRes, true);
            }
            else
            {

            }
            */

            // novi user
            cDB _db = new cDB();
            string sRes = await _db.NewUser("Mirsad", "mirsad");
            if (!String.IsNullOrEmpty(sRes)) // user error
            {
                DependencyService.Get<IShowMessage>().Show(sRes, true);
            }
            else
            {

            }
        }


        async Task LoginGoogleAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_googleService.AccessToken))
                {
                    //Always require user authentication
                    _googleService.Logout();
                }
                await App.Current.MainPage.DisplayAlert("Google Auth", "Juhu", "Ok");

                // Define event handler
                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                await App.Current.MainPage.DisplayAlert("Google Auth", "Juhu 1", "Ok");

                // Set up event handler
                userLoginDelegate = async (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    try
                    {
                        switch (e.Status)
                        {
                            case GoogleActionStatus.Completed:
                                // Login succeeded, navigate or perform other actions here
                                await App.Current.MainPage.DisplayAlert("Google Auth", "Login Successful", "Ok");
                                break;
                            case GoogleActionStatus.Canceled:
                                await App.Current.MainPage.DisplayAlert("Google Auth", "Canceled", "Ok");
                                break;
                            case GoogleActionStatus.Error:
                                await App.Current.MainPage.DisplayAlert("Google Auth", "Error", "Ok");
                                break;
                            case GoogleActionStatus.Unauthorized:
                                await App.Current.MainPage.DisplayAlert("Google Auth", "Unauthorized", "Ok");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error in event handler: " + ex.Message);
                    }
                    finally
                    {
                        _googleService.OnLogin -= userLoginDelegate;
                    }
                };

                // Subscribe to event
                _googleService.OnLogin += userLoginDelegate;
                try
                {
                    // Initiate login process
                    await _googleService.LoginAsync();
                    await App.Current.MainPage.DisplayAlert("Google Auth", "Juhu 3", "Ok");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in LoginAsync: " + ex.Message);
                    await App.Current.MainPage.DisplayAlert("Google Auth", "Error during login: " + ex.Message, "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in LoginGoogleAsync: " + ex.Message);
            }
        }



        private void cmdMenu_Clicked(object sender, EventArgs e)
        {
            //cSentiment cs = new cSentiment();
            //string resultat = await cs.CheckSentence("bad, awful, horrible, unlucky day in the morning of the hell");
            Navigation.PushModalAsync(new pageMenu());
        }

        private async void cmdLogin_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUser.Text) || String.IsNullOrWhiteSpace(txtPass.Text))
            {
                DependencyService.Get<IShowMessage>().Show("empty username or pass...", true);
                return;
            }

            cDB _db = new cDB();
            string sRes = await _db.LoginUser(txtUser.Text, txtPass.Text);
            if (!String.IsNullOrEmpty(sRes)) // login error
            {
                DependencyService.Get<IShowMessage>().Show(sRes, true);
                sLoggedUser = "";
            }
            else
            {
                sLoggedUser = txtUser.Text;
                DependencyService.Get<IShowMessage>().Show("User logged...", true);
                await Navigation.PushModalAsync(new pageMenu(sLoggedUser));
            }
        }

        private async void cmdRegister_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new pageRegister());
        }
    }
}
