using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OpenAI_API;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Xamarin.Essentials;
using NLP_APP.Persistence;
using Plugin.AudioRecorder;

namespace NLP_APP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pageAudio : ContentPage
    {
        string sOpenAiKey = "";
        string sLastFile = String.Empty;
        AudioRecorderService audioService = new AudioRecorderService();
        AudioPlayer audioPlayer = new AudioPlayer();
        string sUserName = "";

        public pageAudio(string pUsername = "")
        {
            InitializeComponent();
            audioService = new AudioRecorderService();
            sUserName = pUsername;
        }

        private async Task<string> AudioFile2Txt(string sFileName)
        {
            HttpClient client = new HttpClient();
            string filePath = sFileName;

            try
            {
                DependencyService.Get<IShowMessage>().Show("Audio -> text", false);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sOpenAiKey);

                using (var memoryStream = new MemoryStream())
                {
                    using (var fs = File.OpenRead(filePath))
                    {
                        fs.CopyTo(memoryStream);
                        var content = new MultipartFormDataContent
                    {
                        { new StringContent("whisper-1"), "model" },
                        { new StringContent("text"), "response_format" },
                        { new ByteArrayContent(memoryStream.ToArray()), "file", filePath }
                    };
                        var response = await client.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error Content: {errorContent}");
                            DependencyService.Get<IShowMessage>().Show($"Error Content: {errorContent}", true);
                            return "";
                        }

                        var transcript = await response.Content.ReadAsStringAsync();
                        // check sentiment
                        DependencyService.Get<IShowMessage>().Show($"Check sentiment for {transcript}", false);
                        cSentiment sen = new cSentiment();
                        string sRes = await sen.CheckSentence(transcript);
                        cDB db = new cDB();
                        await db.LogActivity(sUserName, "audio", sRes, transcript);
                        return sRes;
                    }
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IShowMessage>().Show($"audioStart error: {ex.Message}", true);
                return "";
            }
        }

        private async void cmdAudioToTxt_ClickedAsync(object sender, EventArgs e)
        {
            //OpenAIAPI aiClient = new OpenAIAPI(sOpenAiKey);
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Please select a file to listen..."

                };
                var result = await FilePicker.PickAsync(options);
                if (result == null)
                {
                    DependencyService.Get<IShowMessage>().Show("Simon & Garafunkel: Sound of silence...", true);
                    return;
                }

                string sSentiment = await AudioFile2Txt(result.FullPath);
                DependencyService.Get<IShowMessage>().Show(sSentiment, true);
                return;
            }
            catch (Exception ex)
            {
                DependencyService.Get<IShowMessage>().Show($"audioToTxt error: {ex.Message}", true);
            }
        }

        private async void cmdAudioStart_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(sLastFile))
                {
                    File.Delete(sLastFile);
                    sLastFile = "";
                }
                if (!audioService.IsRecording)
                    await audioService.StartRecording();
                SetEnabled(true);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IShowMessage>().Show($"audioStart error: {ex.Message}", true);
            }
        }

        private async void cmdAudioStop_Clicked(object sender, EventArgs e)
        {
            try
            {
                await audioService.StopRecording();
                sLastFile = audioService.GetAudioFilePath();

                SetEnabled(false);
                // play? = 
                string sSentiment = await AudioFile2Txt(sLastFile);
                DependencyService.Get<IShowMessage>().Show(sSentiment, true);
            }
            catch (Exception ex)
            {
                DependencyService.Get<IShowMessage>().Show($"audioStop error: {ex.Message}", true);
            }
        }

        private void SetEnabled (bool bIsRecording)
        {
            cmdAudioStart.IsEnabled = cmdPlay.IsEnabled = !bIsRecording;
            cmdAudioStop.IsEnabled = bIsRecording;
        }

        private void cmdPlay_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(sLastFile))
            {
                if (File.Exists(sLastFile))
                    audioPlayer.Play(sLastFile);
            }
        }
    }
}