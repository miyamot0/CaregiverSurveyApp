using CaregiverSurveyApp.Pages;
using CaregiverSurveyApp.Values;
using Xamarin.Forms;

namespace CaregiverSurveyApp
{
    public class App : Application
    {
        public static string Token = Credentials.Token;
        public static string ApiAddress = Credentials.Address;
        public static string DeviceName = "DebugDevice";
        public static int Count = 0;

        public App()
        {
            MainPage = new GamePage();
        }
    }
}
