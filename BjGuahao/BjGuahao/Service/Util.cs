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
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using BjGuahao.Model;
using System.Collections.Generic;
using System.Xml;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using DataLib;
using HtmlAgilityPack;

namespace BjGuahao.Service
{
    public static class Util
    {
        #region GetData
        /// <summary>
        /// 取得150家医院信息，输出xml格式数据。
        /// </summary>
        public static void GetHospitals()
        {
            HttpClient request = new HttpClient(Config.hosListUrl, true);
            request.AddHeader("Referer", Config.HomeUrl);
            request.Method = HttpClient.Get_Method;
            request.LoadedSucceed += (s, result) =>
            {
                
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(result as string);
                HtmlAgilityPack.HtmlNode root = html.DocumentNode;
                //-<td width="33%" class="pad10"  valign="top">
                var nodes = (from item in root.Descendants("td") where item.GetAttributeValue("class", "") == "pad10" && item.GetAttributeValue("width", "") == "33%" && item.GetAttributeValue("valign", "") == "top" select item).ToList();

                XElement beijing = new XElement("root");

                foreach (var node in nodes)
                {
                    //<td width="33%" class="pad10"  valign="top"><a href="yyks.php?hpid=142" target="_blank" class="bl_yy">北京大学第三医院</a><br />
                    //等级：三级甲等<br />
                    //地址：北京市海淀区花园北路49号；[第二门诊部]北京海淀区西三旗育新小区内23号楼；[中央党校院区]北京市海淀区大有庄100号(北五环肖家河桥西侧辅路)<br />
                    //</td> 
                    XElement hostpital = new XElement("hostpital");
                    var link = node.Element("a");

                    hostpital.SetAttributeValue("link", link.GetAttributeValue("href", "").Trim());
                    beijing.Add(hostpital);

                    string[] list = node.InnerText.Split('\n');

                    hostpital.SetAttributeValue("name", list[0].Trim());
                    hostpital.SetAttributeValue("level", list[1].Substring(list[1].IndexOf('：') + 1));
                    hostpital.SetAttributeValue("address", list[2].Substring(list[2].IndexOf("：") + 1));
                }

                Debug.WriteLine(beijing.ToString());

            };
            request.run();
        }

        public static void ReBuildData()
        {
            string data = IsoFile.ReadResourceContent("data.xml");
            if (data != null)
            {
                XElement x = XElement.Parse(data);
                x.Descendants("hospital").ToList().ForEach(c => 
                {
                    c.SetAttributeValue("hpid", "");

                    var arr = c.Attribute("link");
                    if (arr != null)
                    {
                        string linkValue = arr.Value;
                        int index = linkValue.IndexOf("hpid=");
                        if (index != -1)
                        {
                            string hpid = linkValue.Substring(index + "hpid=".Length);
                            c.SetAttributeValue("hpid", hpid);

                        }
                        else
                        {
                            Debug.WriteLine("无hpid");
                        }
                    }

                    c.SetAttributeValue("district", "");
                    arr = c.Attribute("address");
                    if (arr != null)
                    {
                        string linkValue = arr.Value;
                        int start = linkValue.IndexOf("北京市");

                        if (start != -1)
                        {
                            linkValue = linkValue.Substring(start + "北京市".Length);
                        }
                        
                        int end = linkValue.IndexOf("区");
                        if (end != -1)
                        {
                            string district = linkValue.Substring(0, end + 1);
                            c.SetAttributeValue("district", district);

                        }
                        else
                        {
                            Debug.WriteLine("无区!");
                        }
                    }
                        
                });
            }
        }

        /// <summary>
        /// 取得医院所有科室信息。
        /// </summary>
        public static List<Department> GetDepartments(string result)
        { 
            //HttpClient request = new HttpClient(Config.bjdxdsyy, true);
            //request.AddHeader("Referer", Config.HomeUrl);
            //request.Method = HttpClient.Get_Method;
            //request.LoadedSucceed += (s, result) =>
            //{

               HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(result as string);
                HtmlAgilityPack.HtmlNode root = html.DocumentNode;

                //<table width="740" border="0" align="center" cellpadding="0" cellspacing="1" bgcolor="#B8D9F5" class="marg15">
                //<tr><td width="100" align="center" bgcolor="#E7F2FC" class="blue pad">内科</td><td bgcolor="#FFFFFF" class="pad"><a href="/comm/beiyi3/ksyy.php?ksid=1010101&hpid=142">心血管科门诊</a>

                var ksboxes = (from item in root.Descendants("div") where item.GetAttributeValue("class", "") == "yyksbox"  select item).ToList();

                if (ksboxes != null && ksboxes.Count > 0)
                {
                    List<Department> departs = new List<Department>();

                    foreach (var ksbox in ksboxes)
                    {
                        Department depart = new Department();

                        depart.Name = ksbox.Element("div") != null ? ksbox.Element("div").InnerText.Trim() : "未知科室";

                        var links = ksbox.Descendants("a");
                        if (links == null && links.Count() == 0)
                        { continue; }

                        foreach (var link in links)
                        {
                            string linkName = link.InnerText;
                            string linkHref = link.GetAttributeValue("href", "");

                            depart.Sections.Add(new SectionInfo { Name = linkName, Link = linkHref });
                        }

                        departs.Add(depart);
                    }

                    //var trs = ksboxes.Elements("tr").ToList();
                    //foreach (var tr in trs)
                    //{
                    //    string departName = tr.FirstChild.InnerText;       
                 
                    //    Department depart = new Department{Name = departName};
                    //    departs.Add(depart);

                    //    var links = tr.LastChild.Elements("a");
                    //    foreach( var link in links )
                    //    {
                    //        string linkName = link.InnerText;
                    //        string linkHref = link.GetAttributeValue("href", "");

                    //        depart.Sections.Add(new SectionInfo {Name = linkName , Link = linkHref  });
                    //    }
                    //}
                    return departs;
                    ////SaveXml(departs);
                }
                return null;
            //};
            //request.run();
        }

        /// <summary>
        /// 弹出html报文。
        /// </summary>
        public static void ShowContent()
        { 
            HttpClient request = new HttpClient(Config.guahao, true);
            request.AddHeader("Referer", Config.HomeUrl);
            request.Method = HttpClient.Get_Method;
            request.LoadedSucceed += (s, result) =>
            {
                MessageBox.Show(result as string);
            };
            request.run();
        }        

        #endregion

        #region SetData

        public static void SaveXml(List<Department> departs)
        {

            DataLib.Serializer.ToXml(departs, "xp");

            var list = DataLib.Serializer.ToObject(typeof(List<Department>), "xp") as List<Department>;
        }

        #endregion

        public static void TestIsoFileMethod()
        {
            IsoFile.DeleteFile("x.txt", "xml");
            IsoFile.CreateFile("x.txt", "xml", "hello");
            bool b =IsoFile.IsFileExist("x.txt", "#$%#xml");
            string r = IsoFile.ReadFile("x.txt", "xml");
            IsoFile.DeleteFile("x.txt", "xml");

            IsoSetting.Remove("5441");
            IsoSetting.Add("1", "1");
            IsoSetting.Add("1", "2");
        }

        public static void ParseTest()
        {
            string content = IsoFile.ReadResourceContent("test.xml");

            if (!string.IsNullOrEmpty(content))
            {
                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(content);
                HtmlNode root = html.DocumentNode;

                var tables = root.Descendants("table");

                ///采集医院标题
                var table = (from t in tables where t.GetAttributeValue("class", "") == "dqwz" select t).FirstOrDefault();
                if (table != null)
                {
                    var a = (from t in table.Descendants("a") where t.GetAttributeValue("class", "") == "blue" select t).FirstOrDefault();
                    if (a != null)
                    {
                        string hosInfo = a.InnerText;
                        Debug.WriteLine(hosInfo);
                    }
                }

                ///采集周次链接
                var nod =(from t in root.Descendants("td") where t.GetAttributeValue("height", "") == "30" && t.GetAttributeValue("align", "") == "center" select t).FirstOrDefault();
                if (nod != null)
                {
                    //MessageBox.Show(nod.InnerText);
                }
                
                ///采集日期链接
                nod = (from t in tables where t.GetAttributeValue("width", "") == "764" && t.GetAttributeValue("class", "") == "marg15" select t).FirstOrDefault();
                if (nod != null)
                {
                    var nodes = nod.Descendants("a");
                    foreach (var n in nodes)
                    {
                        Debug.WriteLine(n.InnerHtml);
                    }
                }

                ///采集表头描述
                nod = (from t in tables where t.GetAttributeValue("class","") == "ks_bg" select t).FirstOrDefault();
                if (nod != null)
                {
                    //MessageBox.Show(nod.InnerText);
                }

                ///采集挂号数据
                nod = (from t in tables where t.GetAttributeValue("bgcolor", "") == "#B8D9F5" select t).FirstOrDefault();
                if (nod != null)
                {
                    var trs = nod.Descendants("tr");
                    if (trs != null && trs.Count() > 0)
                    {
                        OrderInfo  _OrderInfo = new OrderInfo();
                        foreach (var tr in trs)
                        {
                            var orderTds = tr.Elements("td");
                            if (orderTds == null || orderTds.Count() == 0)
                                continue;
                            var orderList = orderTds.ToList();
                            OrderDetail _orderDetail = new OrderDetail();
                            _OrderInfo.Details_.Add(_orderDetail);
                            if (orderList.Count == 11)
                            {
                                _orderDetail.DateTime_ = orderList[0].InnerText.Trim();
                                _orderDetail.Period = orderList[1].InnerText.Trim();
                                _orderDetail.Yard = orderList[2].InnerText.Trim();
                                _orderDetail.Section = orderList[3].InnerText.Trim();
                                _orderDetail.Doctor = orderList[4].InnerText.Trim();
                                _orderDetail.Post = orderList[5].InnerText.Trim();
                                _orderDetail.Cost = orderList[6].InnerText.Trim();
                                _orderDetail.Strong = orderList[7].InnerText.Trim();
                                _orderDetail.Total = orderList[8].InnerText.Trim();
                                _orderDetail.Surplus = orderList[9].InnerText.Trim();
                                _orderDetail.State = orderList[10].InnerText.Trim();

                                var entryNode = orderList[10].Element("a");
                                if (entryNode != null)
                                {
                                    _orderDetail.OrderLink = entryNode.GetAttributeValue("href", "");
                                }
                            }
                            else
                            {
                                MessageBox.Show("数据返回有错误.");
                            }
                        }
                    }
                    
                }
            }
        }
    }
}
