using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace unitTest
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            return ConfigureApp.Android.StartApp();
        }
    }
}