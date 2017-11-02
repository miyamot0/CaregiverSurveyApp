using Acr.UserDialogs;
using ModernHttpClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace CaregiverSurveyApp.Utilities
{
    public static class ServerTools
    {
        /// <summary>
        /// Check if anything is there
        /// </summary>
        /// <returns></returns>
        public static bool CheckAccountContent()
        {
            var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

            if (account != null)
            {
                if (string.IsNullOrWhiteSpace(account.Properties["Key"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["Server"]) ||
                    string.IsNullOrWhiteSpace(account.Properties["DeviceName"]))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Submit credentials to observe response
        /// </summary>
        /// <returns></returns>
        public static Task<string> ChallengeCredentials()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                using (var progress = UserDialogs.Instance.Loading("Checking credentials ...", null, null, true, MaskType.Black))
                {
                    progress.Title = "Checking for credentials...";

                    var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();

                    if (account != null)
                    {
                        if (CheckAccountContent())
                        {
                            string result = await await ServerTools.TestServerCredentials(account.Properties["Server"],
                                            account.Properties["Key"],
                                            account.Properties["DeviceName"]).ContinueWith(async t => await ServerTools.CloseUserDialog(t.Result));

                            if (ServerTools.ValidateResponseString(result))
                            {
                                App.Token = account.Properties["Key"];
                                App.ApiAddress = account.Properties["Server"];
                                App.DeviceName = account.Properties["DeviceName"];

                                tcs.SetResult("Status: Ready.");
                            }
                            else
                            {
                                tcs.SetResult("Status: Auth failed.");
                            }
                        }
                        else
                        {
                            tcs.SetResult("Status: Credentials Empty.");
                        }
                    }
                    else
                    {
                        tcs.SetResult("Status: No Credentials Found.");
                    }
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Challenge specific credentials
        /// </summary>
        /// <returns></returns>
        public static async Task<string> ChallengeCredentials(string _address, string _key, string _id)
        {
            Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Checking credentials ...", MaskType.Black));

            string result = await await TestServerCredentials(_address, _key, _id)
                .ContinueWith(async t => await CloseUserDialog(t.Result));

            return result;
        }

        /// <summary>
        /// Await-able window for assigning icon label
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Task<string[]> TextInputWindow(string query)
        {
            TaskCompletionSource<string[]> tcs = new TaskCompletionSource<string[]>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                var login = await UserDialogs.Instance.LoginAsync(new LoginConfig
                {
                    Message = "Enter your credentials",
                    OkText = "Ok",
                    CancelText = "Cancel",
                    LoginPlaceholder = "Server",
                    PasswordPlaceholder = "Key",
                });

                if (login.Ok)
                {
                    tcs.SetResult(new string[] { login.LoginText, login.Password });
                }
                else
                {
                    tcs.SetResult(new string[] { "", "" });
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        static Task<string> CloseUserDialog(string b)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.HideLoading();

                tcs.SetResult(b);
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static async Task<string> TestServerCredentials(string _address, string _key, string _id)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            try
            {
                // Throw if MITM over captive net
                // Throw if SSL issue
                var httpClient = new HttpClient(new NativeMessageHandler(
                    throwOnCaptiveNetwork: true,
                    customSSLVerification: true
                ));

                var mId = String.Format("{0}-{1}",
                    _id,
                    "TestWrite");

                double[] values = { -1, -1, -1, -1, -1, -1, -1, -1 };

                var parameters = new Dictionary<string, string>
                        {
                            { "token", _key },
                            { "content", "record"},
                            { "format", "xml" },
                            { "type", "flat"},
                            { "overwriteBehavior", "overwrite" },
                            { "data", ConstructResponse(mId, values) },
                            { "returnContent", "count" },
                            { "returnFormat", "json" }
                        };

                var encodedContent = new FormUrlEncodedContent(parameters);

                var resp = await httpClient.PostAsync(new Uri(_address), encodedContent);

                string returnStatment = "";

                if (resp.IsSuccessStatusCode)
                {
                    returnStatment = await resp.Content.ReadAsStringAsync();

                    tcs.SetResult(returnStatment);
                }
                else
                {
                    tcs.SetResult(returnStatment);
                }
            }
            catch
            {
                tcs.SetResult("");
            }

            return await tcs.Task;
        }

        /// <summary>
        /// Upload task to REDcap
        /// </summary>
        /// <returns></returns>
        public static Task<bool> UploadData(double[] sendArray)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                using (var progress = UserDialogs.Instance.Loading("Saving data ...", null, null, true, MaskType.Black))
                {
                    bool result = false;

                    for (int i = 0; i < App.RetrySendCount && !result; i++)
                    {
                        result = await SendDataToServer(sendArray);

                        await Task.Delay(5000);

                        progress.Title = string.Format("Retry {0} of {0}", i + 1, App.RetrySendCount);
                    }

                    tcs.SetResult(result);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Sends to server
        /// </summary>
        /// <returns></returns>
        static Task<bool> SendDataToServer(double[] sendArray)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var httpClient = new HttpClient(new NativeMessageHandler(
                        throwOnCaptiveNetwork: true,
                        customSSLVerification: true
                    ));

                    var mId = string.Format("{0}-{1}",
                        App.DeviceName,
                        App.SubmissionCounter.ToString("0000000000"));

                    var parameters = new Dictionary<string, string>
                    {
                        { "token", App.Token },
                        { "content", "record"},
                        { "format", "xml" },
                        { "type", "flat"},
                        { "overwriteBehavior", "normal" },
                        { "data", ServerTools.ConstructResponse(mId, sendArray) },
                        { "returnContent", "count" },
                        { "returnFormat", "json" }
                    };

                    var encodedContent = new FormUrlEncodedContent(parameters);

                    var resp = await httpClient.PostAsync(new Uri(App.ApiAddress), encodedContent);

                    if (resp.IsSuccessStatusCode)
                    {
                        App.SubmissionCounter = App.SubmissionCounter + 1;

                        Debug.WriteLineIf(App.Debugging, "Success Send: " + resp.Content.ReadAsStringAsync().Result);
                        tcs.SetResult(true);
                    }
                    else
                    {
                        Debug.WriteLineIf(App.Debugging, "Failed Send: " + resp.Content.ReadAsStringAsync().Result);
                        tcs.SetResult(false);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(App.Debugging, "Catch: SendDataToServer = " + ex.ToString());
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static string ConstructResponse(string id, double[] values)
        {
            string temp;
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.Append("<records>");
            sb.Append("<item>");
            sb.Append("<record_id>");
            sb.Append(id);
            sb.Append("</record_id>");

            for (int i = 0; i < 8; i++)
            {
                temp = string.Format("<{0}>{1}</{0}>",
                    "delay_" + (i + 1),
                    values[i]);

                sb.Append(temp);
            }


            sb.Append("</item>");
            sb.Append("</records>");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        public static bool ValidateResponseString(string resp)
        {
            return (resp.Trim().Equals("{\"count\": 1}"));
        }
    }
}
