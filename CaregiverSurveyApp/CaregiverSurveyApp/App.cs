using Xamarin.Forms;

namespace CaregiverSurveyApp
{
    public class App : Application
    {
        public static string Token = "";
        public static string ApiAddress = "";
        public static string DeviceName = "";
        public static int Count = 0;

        public App()
        {
            MainPage = new MainPage();

            //IDictionary envars = Environment.GetEnvironmentVariables();

        }
    }
}
