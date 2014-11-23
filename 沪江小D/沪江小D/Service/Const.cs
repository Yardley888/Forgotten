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

namespace 沪江小D.Service
{
    public static class Const
    {
        public static  class Xpath
        {
            //词义：
            public static string Word_Meaning = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]";
            //同义词：
            public static string Word_Same = "/html[1]/body[1]/div[1]/div[1]/div[2]/div[2]/ul[1]";
            //名词解释：
            public static string Word_Explain = "/html[1]/body[1]/div[1]/div[1]/div[5]/div[2]/div[1]/ol[1]";
            //参考例句：
            public static string Word_Sample = "/html[1]/body[1]/div[1]/div[1]/div[3]/div[2]/ol[1]";
            //常用短语
            public static string Word_Phrase = "/html[1]/body[1]/div[1]/div[1]/div[4]/ol[1]";
            //时态
            public static string Word_Tense = "/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]";
        }
        public static class DivIdValues
        { 
            //单词释义
            public static string Word_Meaning = "panel_comment";
            //单词扩意
            public static string Word_Expand = "panel_forms";
            //名词解释
            public static string Word_Property = "ee_com_ID0EZC_content";


        }
    }
}
