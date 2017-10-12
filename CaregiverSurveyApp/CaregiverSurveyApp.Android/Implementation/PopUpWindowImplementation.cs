using Android.App;
using Android.Widget;
using Xamarin.Forms;
using CaregiverSurveyApp.Interfaces;
using CaregiverSurveyApp.Views;
using CaregiverSurveyApp.Droid.Implementation;
using Android.Views;

[assembly: Dependency(typeof(PopUpWindowImplementation))]
namespace CaregiverSurveyApp.Droid.Implementation
{
    public class PopUpWindowImplementation : IPopUpWindow
    {
        public void ShowPopup(PopUpWindow popup)
        {
            var alert = new AlertDialog.Builder(Forms.Context);

            var layout = new LinearLayout(Android.App.Application.Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                                 ViewGroup.LayoutParams.WrapContent)
            };

            var edit = new EditText(Forms.Context) { Text = "" };
            edit.InputType = Android.Text.InputTypes.TextFlagCapWords;

            var edit2 = new EditText(Forms.Context) { Text = "" };
            edit2.InputType = Android.Text.InputTypes.TextFlagCapWords;

            layout.AddView(edit, 
                ViewGroup.LayoutParams.MatchParent, 
                ViewGroup.LayoutParams.WrapContent);
            layout.AddView(edit2, 
                ViewGroup.LayoutParams.MatchParent, 
                ViewGroup.LayoutParams.WrapContent);
            
            alert.SetView(layout);
            alert.SetTitle(popup.Title);
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = "OK",
                    Text = edit.Text,
                    Text2 = edit2.Text
                });
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopUpWindowArgs
                {
                    Button = "Cancel",
                    Text = edit.Text,
                    Text2 = edit2.Text
                });
            });

            alert.Show();
        }
    }
}