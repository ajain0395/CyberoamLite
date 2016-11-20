using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CyberoamLite
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Storage.StorageFolder roamingfoalder = Windows.Storage.ApplicationData.Current.RoamingFolder;
        MessageDialog messagebox;
        public MainPage()
        {
            this.InitializeComponent();
                  this.NavigationCacheMode = NavigationCacheMode.Required;
            init();


        }
       

        async void mymessagebox(string ms)
        {
            try
            {
                mybutton.IsEnabled = false;
                messagebox = new MessageDialog(ms);
               await messagebox.ShowAsync();
                    mybutton.IsEnabled = true;
            }
            catch(Exception)
            {
                mybutton.IsEnabled = true;
            }
              

            }
        async void init()
        {

            try
            {
               
                await roamingfoalder.GetFileAsync("Data.txt");

                Windows.Storage.StorageFile myStorage = await roamingfoalder.CreateFileAsync("Data.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
                // FileStream filestream = myStorage.OpenFile("Data.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader reader = new StreamReader(await myStorage.OpenStreamForReadAsync()))
                {
                    ip.Text = reader.ReadLine();
                    username.Text = reader.ReadLine();
                    passwordbox.Password = reader.ReadLine();
                    checkBox.IsChecked = Convert.ToBoolean(reader.ReadLine());
                    if (checkBox.IsChecked == false)
                    {
                        passwordbox.Password = "";
                    }
                    
                }

            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// init end
        /// </summary>


        //      /// 
        void active()
        {
            fields(false);
            indicate.IsActive = true;
            mybutton.IsEnabled = false;
        }
        void deactive()
        {

            mybutton.IsEnabled = true;
            indicate.IsActive = false;
        }
        void fields(bool x)
        {
            username.IsEnabled = x;
            checkBox.IsEnabled = x;
            ip.IsEnabled = x;
            passwordbox.IsEnabled = x;
        }
        async void login()
        {

            string data = "mode=191&username=" + username.Text.ToLower() + @"&password=" + passwordbox.Password;
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue(("application/x-www-form-urlencoded")));
                    var response = await hc.PostAsync(new Uri(@"http://" + ip.Text + @":8090/login.xml"), new HttpStringContent(data));

                    using (XmlReader rd = XmlReader.Create(new StringReader(response.Content.ToString())))
                    {
                        rd.ReadToFollowing("message");
                        string msg = rd.ReadElementContentAsString();
                        if (msg.ToLower() == "You have successfully logged in".ToLower())
                        {

                            fields(false);
                            mybutton.Content = "Logout";
                        }
                        else
                        {
                            fields(true);
                        }
                        mymessagebox(msg);
                        deactive();
                       
                    }
                }

            }
            catch (Exception)
            {
               
                mymessagebox("Oops! Network error");
                deactive();
                fields(true);
            }


        }        /// <summary>
                 /// end of login
                 /// </summary>
        async void logout()
        {

            string data = "mode=193&username=" + username.Text.ToLower() + @"&password=" + passwordbox.Password;
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue(("application/x-www-form-urlencoded")));
                    var response = await hc.PostAsync(new Uri(@"http://" + ip.Text + @":8090/logout.xml"), new HttpStringContent(data));

                    using (XmlReader rd = XmlReader.Create(new StringReader(response.Content.ToString())))
                    {
                        rd.ReadToFollowing("message");
                        string msg = rd.ReadElementContentAsString();
                        if (msg.ToLower() != "You have successfully logged in".ToLower())
                        {
                            mybutton.Content = "Login";
                            fields(true);
                        }
                        mymessagebox(msg);
                        deactive();
                      
                    }
                }

            }
            catch (Exception)
            {
                mybutton.Content = "Login";
                
                mymessagebox("Oops! Network error");
                fields(true);
                deactive();
            }

        }///////////
        ///////End of Logout

        async void loginlogout()
        {
            active();
            if (ip.Text.ToString() == "127.0.0.1" && username.Text.ToString() == "demo#" && passwordbox.Password == "demo#")
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == false)
                {
                    mymessagebox("Oops! Network error");
                    deactive();
                    fields(true);

                }
                else if (mybutton.Content.ToString() == "Login")
                {
                    mybutton.Content = "Logout";

                    mymessagebox("You have successfully logged in");
                    fields(false);

                }
                else
                {
                    mybutton.Content = "Login";
                    mymessagebox("You have successfully logged off");
                    fields(true);

                }
                deactive();
            }
            else
            {
                try
                {
                    if (mybutton.Content.ToString() == "Login")
                    {
                        if (username.Text.Length > 0 && ip.Text.Length > 0 && passwordbox.Password.Length > 0)
                        {
                            login();


                        }
                        else
                        {
                            mymessagebox("Oops! There are invalid Input Parameters, please review them.");
                            deactive();
                            fields(true);


                        }
                    }
                    else
                    {
                        logout();

                        if (checkBox.IsChecked == false)
                        {
                            passwordbox.Password = "";
                        }

                    }


                }
                catch (Exception)
                {
                    mymessagebox("Oops! There are invalid Input Parameters, please review them.");
                    deactive();
                    fields(true);


                }
            }

            Windows.Storage.StorageFile sampleFile = await roamingfoalder.CreateFileAsync("Data.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            {

                string tmp;


                tmp = "" + ip.Text + "\n" + username.Text.ToLower() + "\n" + passwordbox.Password + "\n" + "true" + "\n" + mybutton.Content.ToString() + "";

                //    if (myStorage.FileExists("Data.txt"))
                {
                    using (StreamWriter writer = new StreamWriter(await sampleFile.OpenStreamForWriteAsync()))
                    {
                        string mydata = tmp;
                        writer.WriteLine(mydata);
                        writer.Flush();
                    }
                }

            }

        }
        public void button1_Click(object sender, RoutedEventArgs e)
        {
            loginlogout();
        }

        /// <summary>
        /// end of button click
        /// </summary>
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }


        private void username_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key== Windows.System.VirtualKey.Enter)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
        }

        private void passwordBox1_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                loginlogout();
            }
        }

        
    }
}
