using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using 沪江小D.Service;
using HtmlAgilityPack;
using System.Diagnostics;

namespace 沪江小D.View
{
    public partial class Main : PhoneApplicationPage
    {
        public Main()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void query_Click(object sender, RoutedEventArgs e)
        {
            string input = this.inputWord.Text;
            if (!string.IsNullOrEmpty(input))
            {
                queryWord(input);
            }
        }

        private void queryWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return;
            MyHttpClient myClient = new MyHttpClient("http://www.bjguahao.gov.cn/comm/index.php");
            myClient.LoadedSucceed += (s, text) =>
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(text);
                HtmlNode root = htmlDoc.DocumentNode;
                HtmlNode word_meaning_node = (from item in root.Descendants("div") where item.HasAttributes && item.Attributes.FirstOrDefault().Value == "panel_comment" select item).FirstOrDefault();
                if (word_meaning_node != null && !string.IsNullOrEmpty(word_meaning_node.InnerText))
                {
                    this.word_meaning.Text = word_meaning_node.InnerText;
                }
                else
                {
                    this.word_meaning.Text = "抱歉，未查询到该单词。";
                }
            };
            myClient.run();
        }

        private void inputWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Debug.WriteLine("enter");
                string input = this.inputWord.Text;
                if (!string.IsNullOrEmpty(input))
                {
                    queryWord(input);
                }
                this.Focus();
            }
        }
    }
}