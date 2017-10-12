using CaregiverSurveyApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CaregiverSurveyApp.Views
{
    /// <summary>
    /// Assigned/returned args to cross-platform window
    /// </summary>
    public class PopUpWindowArgs : EventArgs
    {
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Button { get; set; }
    }

    /// <summary>
    /// Pop-up window interface
    /// </summary>
    public class PopUpWindow
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Text2 { get; set; }
        public List<string> Buttons { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        public PopUpWindow(string title, string text, params string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons.ToList();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        public PopUpWindow(string title, string text) : this(title, text, "OK", "Cancel") { }

        /// <summary>
        /// Event call following the close of the window, drawing args
        /// </summary>
        public event EventHandler<PopUpWindowArgs> PopupClosed;
        public void OnPopupClosed(PopUpWindowArgs e)
        {
            PopupClosed?.Invoke(this, e);
        }

        /// <summary>
        /// Dependency Service call to individual implementation
        /// </summary>
        public void Show()
        {
            DependencyService.Get<IPopUpWindow>().ShowPopup(this);
        }
    }
}