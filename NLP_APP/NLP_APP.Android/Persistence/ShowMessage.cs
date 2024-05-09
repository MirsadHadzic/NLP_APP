using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using NLP_APP.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(NLP_APP.Droid.ShowMessage))]
namespace NLP_APP.Droid{

    internal class ShowMessage : IShowMessage
    {
        public void Show(string sMessage, bool bLongMess = true)
        {
            if (sMessage.Length == 0)
                return;
            var context = Android.App.Application.Context;
            /*
            Toast.MakeText(Android.App.Application.Context, 
                sMessage, 
                (bLongMess)? ToastLength.Long : ToastLength.Short
            ).Show();
            */

            // big text version
            SpannableStringBuilder biggerText = new SpannableStringBuilder(sMessage);
            biggerText.SetSpan(new RelativeSizeSpan(1.1f), 0, sMessage.Length, 0);
            Toast.MakeText(context, biggerText, (bLongMess) ? ToastLength.Long : ToastLength.Short).Show();
        }
    }
}