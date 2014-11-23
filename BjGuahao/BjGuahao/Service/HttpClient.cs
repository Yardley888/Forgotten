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
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using ImageTools.Controls;
using ImageTools.IO;
using ImageTools;
using ImageTools.IO.Gif;

namespace BjGuahao.Service
{
    public class HttpClient
    {
        #region Fields

        public static string Get_Method = "GET";
        public static string Post_Method = "POST";

        private HttpWebRequest myRequest;
        private string url;
        private string method_ = Get_Method;
        private bool bSetCookie = false;
        private bool bPostForm = false;

        private byte[] postData;

        #endregion

        #region Properties

        public string Method
        {
            get { return method_; }
            set
            {
                method_ = value;
                myRequest.Method = method_;
            }
        }

        #endregion

        #region 事件

        public delegate void LoadedSucceedEventHander(object sender, object result);
        public delegate void LoadedFailedEventHander(object sender, object result);
        public event LoadedSucceedEventHander LoadedSucceed;
        public event LoadedFailedEventHander LoadedFailed;

        #endregion

        #region 构造函数

        public HttpClient(string url, bool bSetCookie = false)
        {
            if (!string.IsNullOrEmpty(url))
            {
                this.url = url;
                this.bSetCookie = bSetCookie;
                try
                {
                    myRequest = (HttpWebRequest)HttpWebRequest.Create(this.url);
                    
                    if (bSetCookie)
                    {
                        myRequest.CookieContainer = Common.GetCookie();
                    }
                }
                catch
                { }
            }

        }

        #endregion

        #region 公开方法

        public void run()
        {
            if (myRequest != null)
            {
                try
                {
                    if (bPostForm)
                    {
                        myRequest.BeginGetRequestStream(result =>
                            {
                                using (Stream stream = myRequest.EndGetRequestStream(result))
                                {
                                    stream.Write(this.postData, 0, postData.Length);
                                }
                                myRequest.BeginGetResponse(MyResponseCallback, myRequest);
                            }, null);
                    }
                    else
                    {
                        myRequest.BeginGetResponse(MyResponseCallback, myRequest);
                    }
                }
                catch(Exception e)
                {
                    onLoadedFailed("连接失败");
                }
            }
            else
            {
                onLoadedFailed("创建连接失败");
            }
        }

        public void AddHeader(string key, string value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                myRequest.Headers[key] = value;
            }
        }

        public void SetFormData(string data)
        {
            //byte[] postData = Encoding.UTF8.GetBytes(data);
            byte[] postData = HtmlAgilityPack.Gb2312Encoding.Current.GetBytes(data);
            myRequest.ContentType = "application/x-www-form-urlencoded";
            this.postData = postData;
            this.bPostForm = true;
        }

        #endregion

        #region 私有方法

        private void MyResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest requst = (HttpWebRequest)result.AsyncState;
                HttpWebResponse response = (HttpWebResponse)requst.EndGetResponse(result);

                string contentType = response.ContentType;

                if (contentType.StartsWith("text", StringComparison.CurrentCultureIgnoreCase))
                {
                    Encoding coding = null;
                    if (contentType.ToLower().Contains("gbk") || contentType.ToLower().Contains("gb2312"))
                    {
                        coding = new HtmlAgilityPack.Gb2312Encoding();
                    }
                    else
                    {
                        coding = new UTF8Encoding();
                    }
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader streamReader = new StreamReader(stream, coding))
                    {
                        string content = streamReader.ReadToEnd();

                        if (!string.IsNullOrEmpty(content))
                        {
                            onLoadedSucceed(content);
                        }
                    }
                    response.Close();
                }
                else if (contentType.ToLower().StartsWith("image"))
                {
                    if (contentType.ToLower().Contains("gif"))
                    {
                        Stream stream = response.GetResponseStream();

                        this.BeginInvoke(() =>
                            {
                                //ImageTools.IO.Decoders.AddDecoder<GifDecoder>();
                                AnimatedImage gifImage = new AnimatedImage() { Width = 100, Height = 100 };

                                ExtendedImage extendedImag = new ExtendedImage();

                                GifDecoder dc = new GifDecoder();
                                dc.Decode(extendedImag, stream);
                                gifImage.Source = extendedImag;

                                stream.Dispose();
                                onLoadedSucceed(gifImage);
                                response.Close();
                            });
                    }
                    else
                    {
                        Debug.WriteLine("Not support!");
                    }

                }
                else
                {
                    Debug.WriteLine("Not support!");
                }
                
                if (bSetCookie)
                {
                    SetCookie(response);
                }

                
            }
            catch (WebException)
            {
                onLoadedFailed("加载失败");
            }
            catch (Exception e)
            {
                onLoadedFailed(e.GetType().ToString());
            }
        }

        private void SetCookie(HttpWebResponse response)
        {

            if (response != null && response.Headers.Count > 0)
            {
                string Cookie = response.Headers["Set-Cookie"];
                if (!string.IsNullOrEmpty(Cookie))
                {
                    string[] list = Cookie.Split(';');
                    string temp = list[0];
                    string[] paire = temp.Split('=');
                    if (paire.Length == 2)
                    {
                        string key = paire[0];
                        string value = paire[1];
                        Common.AddCookie(key, value);
                    }
                }
            }
        }

        private void onLoadedFailed(string failedString)
        {
            if (LoadedFailed != null)
            {
                this.BeginInvoke(() => 
                { 
                    LoadedFailed(this, failedString); 
                });
            }
        }

        private void onLoadedSucceed(object content)
        {

            if (LoadedSucceed != null)
            {
                this.BeginInvoke(() => 
                { 
                    LoadedSucceed(this, content); 
                });
            }
        }

        private void BeginInvoke(Action a)
        {
            Deployment.Current.Dispatcher.BeginInvoke(a);
        }

        #endregion
    }


}
