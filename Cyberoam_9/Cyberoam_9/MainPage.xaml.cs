using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Cyberoam_9
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

        async void init()
        {
            try
            {
                await roamingfoalder.GetFileAsync("Data.txt");

                StorageFile myStorage = await roamingfoalder.CreateFileAsync("Data.txt", CreationCollisionOption.OpenIfExists);
                // FileStream filestream = myStorage.OpenFile("Data.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader reader = new StreamReader(await myStorage.OpenStreamForReadAsync()))
                {
                    this.ip.Text = reader.ReadLine();
                    this.username.Text = reader.ReadLine();
                    this.passwordBox1.Password = reader.ReadLine();
                    this.checkBox.IsChecked = Convert.ToBoolean(reader.ReadLine());
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
        async void login()
        {

            string data = "mode=191&username=" + username.Text.ToLower() + @"&password=" + passwordBox1.Password;

            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue(("application/x-www-form-urlencoded")));
                    var response = await hc.PostAsync(new Uri(@"http://" + ip.Text + @":8090/login.xml"), new HttpStringContent(data));

                    using (XmlReader rd = XmlReader.Create(new StringReader(response.Content.ToString())))
                    {
                        rd.ReadToFollowing("message");
                        string msg = rd.ReadElementContentAsString();
                        if (msg.ToLower() == "You have successfully logged in".ToLower())
                        {

                            username.IsEnabled = false;
                            ip.IsReadOnly = true;
                            passwordBox1.IsEnabled = false;
                            mybutton.Content = "Logout";
                        }
                        messagebox = new MessageDialog(msg);
                        indicate.IsActive = false;
                        await messagebox.ShowAsync();
                    }
                }
            }
            catch (Exception)
            {
                indicate.IsActive = false;
                messagebox = new MessageDialog("Oops! Network error");
                await messagebox.ShowAsync();
            }

        }        /// <summary>
                 /// end of login
                 /// </summary>
        async void logout()
        {

            string data = "mode=193&username=" + username.Text.ToLower() + @"&password=" + passwordBox1.Password;
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue(("application/x-www-form-urlencoded")));
                    var response = await hc.PostAsync(new Uri(@"http://" + ip.Text + @":8090/logout.xml"), new HttpStringContent(data));

                    using (XmlReader rd = XmlReader.Create(new StringReader(response.Content.ToString())))
                    {
                        rd.ReadToFollowing("message");
                        string msg = rd.ReadElementContentAsString();
                        if (msg.ToLower() != "You have successfully logged in".ToLower())
                        {
                            mybutton.Content = "Login";
                            username.IsEnabled = true;
                            ip.IsReadOnly = false;
                            passwordBox1.IsEnabled = true;
                        }
                        messagebox = new MessageDialog(msg);
                        indicate.IsActive = false;
                        await messagebox.ShowAsync();
                    }
                }
            }
            catch (Exception)
            {
                mybutton.Content = "Login";
                username.IsEnabled = true;
                ip.IsReadOnly = false;
                passwordBox1.IsEnabled = true;
                messagebox = new MessageDialog("Oops! Network error");
                indicate.IsActive = false;
                await messagebox.ShowAsync();
            }
        }////////////
         ///////End of Logout

        public async void button1_Click(object sender, RoutedEventArgs e)
        {
            indicate.IsActive = true;
            if (ip.Text.ToString() == "127.0.0.1" && username.Text.ToString() == "demo#" && passwordBox1.Password == "demo#")
            {
                if (NetworkInterface.GetIsNetworkAvailable() == false)
                {
                    messagebox = new MessageDialog("Oops! Network error");
                    await messagebox.ShowAsync();
                }
                else if (mybutton.Content.ToString() == "Login")
                {
                    mybutton.Content = "Logout";
                    username.IsEnabled = false;
                    ip.IsReadOnly = true;
                    passwordBox1.IsEnabled = false;
                    messagebox = new MessageDialog("You have successfully logged in");
                    await messagebox.ShowAsync();
                }
                else
                {
                    mybutton.Content = "Login";
                    username.IsEnabled = true;
                    ip.IsReadOnly = false;
                    passwordBox1.IsEnabled = true;
                    messagebox = new MessageDialog("You have successfully logged off");
                    await messagebox.ShowAsync();
                }
                indicate.IsActive = false;
            }
            else
            {
                try
                {
                    if (mybutton.Content.ToString() == "Login")
                    {
                        if (username.Text.Length > 0 || ip.Text.Length > 0)
                        {
                            login();
                        }
                        else
                        {
                            messagebox = new MessageDialog("Oops! There are invalid Input Parameters, please review them.");
                            await messagebox.ShowAsync();
                        }
                    }
                    else
                    {
                        logout();
                        if (checkBox.IsChecked == false)
                        {
                            passwordBox1.Password = "";
                        }
                    }


                }
                catch (Exception)
                {
                    indicate.IsActive = false;
                    //  indicator.IsVisible = false;
                    messagebox = new MessageDialog("Oops! There are invalid Input Parameters, please review them.");
                    await messagebox.ShowAsync();
                    //MessageBox.Show("Oops! There are invalid Input Parameters, please review them.");
                }
            }

            StorageFile sampleFile = await roamingfoalder.CreateFileAsync("Data.txt", CreationCollisionOption.ReplaceExisting);
            {

                string tmp;

                if (checkBox.IsChecked == true)
                {
                    tmp = "" + ip.Text + "\n" + username.Text.ToLower() + "\n" + passwordBox1.Password + "\n" + "true" + "";
                }
                else
                {
                    tmp = "" + ip.Text + "\n" + username.Text.ToLower() + "\n" + "" + "\n" + "false" + "";
                }
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
    }
}
