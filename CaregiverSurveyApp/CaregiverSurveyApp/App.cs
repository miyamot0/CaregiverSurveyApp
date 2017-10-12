using CaregiverSurveyApp.Pages;
using CaregiverSurveyApp.Values;
using Xamarin.Forms;

namespace CaregiverSurveyApp
{
    public class App : Application
    {
        public static string AppName = "com.smallnstats.caregiversurveyapp";
        public static string Token = Credentials.Token;
        public static string ApiAddress = Credentials.Address;
        public static string DeviceName = "DebugDevice";
        public static int Count = 0;

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            MainPage = new GamePage();
        }
    }
}
