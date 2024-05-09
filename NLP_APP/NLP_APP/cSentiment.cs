using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLP_APP
{
    class cSentiment
    {
        int nMaxTxtLength = 1000;
        int nMaxLinesInService = 31;
        List<string> lstLines = new List<string>();
        List<double> lstResults = new List<double>();
        string sLangCode = "en";

        public async Task<string> CheckSentence(string inputTxt)
        {
            // remove line breaks
            inputTxt = RemoveSpecialCharacters(inputTxt);
            // split lines
            SplitString2List(inputTxt);
            string sRes = await CallRapidApi();
            return sRes;
        }

        /*
         [
            {
                "id": "1",
                "language": "en",
                "text": "I love the service"
            }
        ]
*/
        private async Task<string> CallRapidApi()
        {
            lstResults.Clear();
            var client = new HttpClient();
            while (lstLines.Count>0)
            {
                int nMaxLines = nMaxLinesInService;
                int nIdx = 1;
                JArray jaSend = new JArray();
                while ( (nMaxLines>0) && (lstLines.Count > 0))
                {
                    JObject jo = new JObject();
                    jo.Add("id", nIdx);
                    jo.Add("language", sLangCode);
                    jo.Add("text", lstLines.ElementAt(0));
                    nIdx += 1;
                    lstLines.RemoveAt(0);
                    jaSend.Add(jo);
                    nMaxLines--;
                } // while (nMaxLines>=0)
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://sentiment-analysis9.p.rapidapi.com/sentiment"),
                    Headers =
                    {
                        { "Accept", "application/json" },
                        { "X-RapidAPI-Key", "" },
                        { "X-RapidAPI-Host", "sentiment-analysis9.p.rapidapi.com" },
                    },
                    Content = new StringContent(jaSend.ToString())
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }
                };

                try
                {
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        var body = await response.Content.ReadAsStringAsync();
                        JArray jaBody = JArray.Parse(body);
                        foreach (JObject joItem in jaBody)
                        {
                            try
                            {
                                JArray jaResults = (JArray)joItem["predictions"];
                                JObject joResults = (JObject)jaResults[0];
                                double fRes = joResults["probability"].Value<double>();
                                string sSign = joResults["prediction"].Value<string>();
                                if (sSign.Equals("negative"))
                                    fRes = -fRes;
                                lstResults.Add(fRes);
                            }
                            catch (Exception) { };
                        }
                    } // using
                }
                catch (Exception e)
                {

                }
            } //while (lstLines.Count>0)

            double fResult = lstResults.Average();
            // string sRes = (fResult >= 0) ? $"positive: {fResult}" : $"negative: {fResult}";

            string sRes;

            if (fResult >= -1 && fResult < -0.7)
            {
                sRes = "Veoma negativan rezultat -> 😡"; //+ "\ud83d\ude01"; // + fResult;
                    //+ Toast.makeText(this, "Smileys = " + ("\ud83d\ude01"), Toast.LENGTH_LONG).show();
            }
            else if (fResult >= -0.7 && fResult < -0.3)
            {
                sRes = "Srednje negativan rezultat -> 😕"; // + fResult;
            }
            else if (fResult >= -0.3 && fResult < 0)
            {
                sRes = "Negativan rezultat -> 😞"; // + fResult;
            }
            else if (fResult >= 0 && fResult < 0.3)
            {
                sRes = "Pozitivan rezultat -> 🙂"; // + fResult;
            }
            else if (fResult >= 0.3 && fResult < 0.7)
            {
                sRes = "Srednje pozitivan rezultat -> 😊"; // + fResult;
            }
            else if (fResult >= 0.7 && fResult <= 1)
            {
                sRes = "Veoma pozitivan rezultat -> 😄"; // + fResult;
            }
            else
            {
                sRes = "Nepostojeci rezultat -> " + fResult;
            }

            return sRes;
        }

        void SplitString2List(string input)
        {
            lstLines.Clear();
            // Split the input string by whitespace
            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string currentLine = "";
            foreach (string word in words)
            {
                if ((currentLine + word).Length > nMaxTxtLength) // Check if adding the current word exceeds the maximum length
                {
                    lstLines.Add(currentLine.Trim()); // Add the current line to the result
                    currentLine = ""; // Reset current line
                }
                currentLine += word + " "; // Add the word to the current line with a space
            }

            if (!string.IsNullOrEmpty(currentLine))
                lstLines.Add(currentLine.Trim()); // Add the last line to the result
        }

        static string RemoveSpecialCharacters(string input)
        {
            // Define a regular expression pattern to match special characters
            string pattern = "[^a-zA-Z0-9 ]";

            // Replace special characters with an empty string
            string result = Regex.Replace(input, pattern, "");

            return result;
        }

    }
}
