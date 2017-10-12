using CaregiverSurveyApp.Pages;
using Xamarin.Forms;

namespace CaregiverSurveyApp
{
    public class App : Application
    {
        public static string AppName = "com.smallnstats.caregiversurveyapp";
        public static string Token = "";
        public static string ApiAddress = "";
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
