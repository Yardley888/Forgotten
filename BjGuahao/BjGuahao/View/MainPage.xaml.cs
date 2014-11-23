using Microsoft.Phone.Controls;
using BjGuahao.Service;
using System;
using System.Windows.Controls;
using ImageTools.Controls;

namespace BjGuahao
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        
        //-test
        private void image1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HttpClient request = new HttpClient(Config.codeUrl +"?"+ new Random().NextDouble(), true);

            request.Method = HttpClient.Get_Method;
            request.LoadedSucceed += (s, result) =>
            {
                if (result is AnimatedImage)
                {
                    (sender as Grid).Children.Add(result as AnimatedImage);
                }
            };
            request.run();
            
            //ImageDownLoader loader = new ImageDownLoader(this.image1);
            //loader.DownloadImageAsyncUsingHttpRequest("http://avatar.csdn.net/C/A/8/1_antsnm.jpg");
        }

        private void button2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HttpClient logonClient = new HttpClient(Config.LogonUrl, true);
            logonClient.AddHeader("Referer", Config.HomeUrl);
            logonClient.Method = HttpClient.Post_Method;
            //logonClient.SetFormData("truename=" + this.textBox1.Text.Trim() + "&sfzhm=" + this.textBox2.Text.Trim() + "&yzm=" + this.textBox3.Text + "&submit.y=16&submit.x=20");
            logonClient.run();
        }

    }
}
