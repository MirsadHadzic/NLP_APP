using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace unitTest
{
    [TestFixture(Platform.Android)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void WelcomeTextIsDisplayed()
        {
            AppResult[] results = app.WaitForElement(c => c.Marked("Welcome to Xamarin.Forms!"));
            app.Screenshot("Welcome screen.");

            Assert.IsTrue(results.Any());
        }

        // trebam u menu page-u staviti AutomationId za svaki button, to se stavlja da bi se lakse identifikovao button tokom testiranja
        // moram, takodjer, update napraviti za za svaki xaml page, moze kod stack lyaout-a svakog, mora se staviti, npr. AutomationId = "TextPageIdentifier"

        [Test]
        public void TxtButtonNavigatesToTextPage()
        {
            app.Tap(c => c.Marked("cmdTxt"));
            app.Screenshot("Navigated to Text Page");

            AppResult[] results = app.WaitForElement(c => c.Marked("TextPageIdentifier"));
            Assert.IsTrue(results.Any(), "Text Page did not appear.");
        }

        [Test]
        public void UrlButtonNavigatesToUrlPage()
        {
            app.Tap(c => c.Marked("cmdUrl"));
            app.Screenshot("Navigated to URL Page");

            AppResult[] results = app.WaitForElement(c => c.Marked("UrlPageIdentifier"));
            Assert.IsTrue(results.Any(), "URL Page did not appear.");
        }

        [Test]
        public void FileButtonNavigatesToFilePage()
        {
            app.Tap(c => c.Marked("cmdFile"));
            app.Screenshot("Navigated to File Page");

            AppResult[] results = app.WaitForElement(c => c.Marked("FilePageIdentifier"));
            Assert.IsTrue(results.Any(), "File Page did not appear.");
        }

        [Test]
        public void AudioButtonNavigatesToAudioPage()
        {
            app.Tap(c => c.Marked("cmdAudio"));
            app.Screenshot("Navigated to Audio Page");

            AppResult[] results = app.WaitForElement(c => c.Marked("AudioPageIdentifier"));
            Assert.IsTrue(results.Any(), "Audio Page did not appear.");
        }

        [Test]
        public void HistoryButtonNavigatesToHistoryPage()
        {
            app.Tap(c => c.Marked("cmdHistory"));
            app.Screenshot("Navigated to History Page");

            AppResult[] results = app.WaitForElement(c => c.Marked("HistoryPageIdentifier"));
            Assert.IsTrue(results.Any(), "History Page did not appear.");
        }
    }
}
