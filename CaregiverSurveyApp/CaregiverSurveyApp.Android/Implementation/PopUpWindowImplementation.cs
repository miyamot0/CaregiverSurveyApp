
using Android.App;
using Android.Widget;
using Xamarin.Forms;
using CaregiverSurveyApp.Interfaces;
using CaregiverSurveyApp.Views;
using CaregiverSurveyApp.Droid.Implementation;

[assembly: Dependency(typeof(PopUpWindowImplementation))]
namespace CaregiverSurveyApp.Droid.Implementation
{
    public class PopUpWindowImplementation : IPopUpWindow
    {
        public void ShowPopup(PopUpWindow popup)
        {
            var alert = new AlertDialog.Builder(Forms.Context);
            var edit = new EditText(Forms.Context) { Text = popup.Text };
            edit.InputType = Android.Text.InputTypes.TextFlagCapWords;
            alert.SetView(edit);
            alert.SetTitle(popup.Title);
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = "OK",
                    Text = edit.Text
                });
            });
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = "Cancel",
                    Text = edit.Text
                });
            });
            alert.Show();
        }
    }
}