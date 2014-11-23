using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace BjGuahao.Service
{

    public class ImageDownLoader
    {
        Image image;

        public ImageDownLoader(Image img)
        {
            image = img;
        }

        public void DownLoadImageAsyncUsingWebClient(string uri)
        {
            if (image == null)
            {
                return;
            }

            WebClient client = new WebClient();
            client.Headers["Referer"] = "http://www.bjguahao.gov.cn/";
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null && e.Result == null)
            {
                return;
            }

            BitmapImage bi = new BitmapImage();
            bi.SetSource(e.Result);
            image.Source = bi;
        }

        public void DownloadImageAsyncUsingHttpRequest(string url)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                var request = HttpWebRequest.Create(url);
                request.Headers["Referer"] = "http://www.bjguahao.gov.cn/";
                request.BeginGetResponse(result =>
                {
                    var response = request.EndGetResponse(result);
                    Stream imageStream = response.GetResponseStream();
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        try
                        {
                            image.Source = null;

                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
                            bitmapImage.SetSource(imageStream);
                            image.Source = bitmapImage;
                        }
                        catch(Exception e) 
                        {
                            
                        }
                    });

                }, null);
            }
            catch
            { }
        }
    }

}
