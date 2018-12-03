using PinterestFollow.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PinterestFollow.Classes
{
    public class PinterestHelper
    {
        public static bool Login(ref User User)
        {
            
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://pinterest.com/login/");
            req.CookieContainer = new CookieContainer();
            req.Proxy = GetProxyForUser(User);
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.152 Safari/537.22";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream());
            var responseString = reader.ReadToEnd();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(responseString);
            var csrfmiddlewaretoken = doc.DocumentNode.SelectSingleNode("//input[@name = 'csrfmiddlewaretoken']").Attributes["value"].Value;
            var _ch = doc.DocumentNode.SelectSingleNode("//input[@name = '_ch']").Attributes["value"].Value;
            var strPostData = string.Format("email={0}&password={1}&csrfmiddlewaretoken={2}&_ch={3}", User.UserName, User.Password, csrfmiddlewaretoken, _ch);
            req = (HttpWebRequest)HttpWebRequest.Create("https://pinterest.com/login/?next=%2Flogin%2F");
            req.CookieContainer = new CookieContainer();
            req.Proxy = GetProxyForUser(User);
            
            foreach (Cookie cookie in res.Cookies)
                req.CookieContainer.Add(cookie);
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.152 Safari/537.22";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://pinterest.com/login";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Headers.Add("Accept-Language: en-US");
            req.Method = "POST";

            #region WritePostData

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strPostData);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();
            #endregion
            res = (HttpWebResponse)req.GetResponse();
            reader = new StreamReader(res.GetResponseStream());

            responseString = reader.ReadToEnd();
            if (res.ResponseUri.AbsolutePath.StartsWith("/login/"))
            {
                User.LastMessage = "Invalid User Name Or Passowrd";
                
                User.IsLogin = false;
                User.HaveError = true;
                User.Enabled = false;
                throw new Exception("Invalid User Name Or Passowrd");
            }


            try
            {
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(responseString);
                var vUserName = doc.DocumentNode.SelectSingleNode("//*[@id='UserNav' or @class='UserMenu Module']/a").Attributes["href"].Value;
                User.pUsername = vUserName.Replace("/", "").Replace("\\", "");
            }
            catch (Exception ex)
            {
                User.IsLogin = false;
                User.HaveError = true;
                User.Enabled = false;
                User.LastMessage = ex.Message;
                return false;
            }
            if (User.Cookies == null) User.Cookies = new List<Cookie>();
            foreach (Cookie cookie in res.Cookies)
            {

            }
            foreach (Cookie Cookie in req.CookieContainer.GetCookies(new Uri("http://pinterest.com")))
            {
                User.Cookies.Add(Cookie);
            }
            try
            {
                GetMemo(ref User);
            }
            catch (Exception)
            {

            }

            User.LastSignIn = DateTime.Now;
            User.IsLogin = true;

            return true;
        }
        public static bool GetMemo(ref User User)
        {
            var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            var v5 = new Cookie() { Name = "viewParameter", Value = "FEED_FOLLOWING", Domain = v1.Domain };
            var v6 = new Cookie() { Name = "viewType", Value = "FEED", Domain = v1.Domain };

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("http://pinterest.com/get_memo/?memo_placement=0&extra=%7B%7D");
            CookieContainer myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);

            #region Header
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUSMSE)";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://pinterest.com/";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Headers.Add("Accept-Language: en-US");
            req.Method = "GET";
            req.Headers.Add(string.Format("X-CSRFToken: {0}", v2.Value));
            req.Headers.Add("X-Pinterest-Referrer: http://pinterest.com/");
            req.Headers.Add("X-Requested-With: XMLHttpRequest");
            #endregion


            System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());

            foreach (Cookie co in myCookies.GetCookies(req.RequestUri))
            {
                if (co.Name == "memo")
                {
                    var memo = User.Cookies.FirstOrDefault(aa => aa.Name == "memo");
                    if (memo == null)
                        User.Cookies.Add(co);
                }
            }

            foreach (Cookie co in resp.Cookies)
            {
                if (co.Name == "memo")
                {
                    var memo = User.Cookies.FirstOrDefault(aa => aa.Name == "memo");
                    if (memo == null)
                        User.Cookies.Add(co);
                }
            }

            return false;
        }
        private static IWebProxy GetProxyForUser(User User)
        {
            var Proxy = User.Proxies;
            
            var strs = Proxy.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (strs.Length == 2)
                {
                    WebProxy W = new WebProxy(User.Proxies);
                    return W;
                }
                else if (strs.Length == 3)
                {
                    WebProxy W = new WebProxy(string.Join(":", strs[0], strs[1]));
                    W.Credentials = new NetworkCredential() { UserName = strs[2] };

                    return W;
                }
                else if (strs.Length == 4)
                {

                    WebProxy W = new WebProxy(string.Join(":",  strs[0], strs[1]));
                    W.Credentials = new NetworkCredential() { UserName = strs[2], Password = strs[3] };
                    return W;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static List<string> GetUserFromPage(string Url, User User)
        {
            var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            var v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
            var v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);
            CookieContainer myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);

            #region Header
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUSMSE)";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://pinterest.com/";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Headers.Add("Accept-Language: en-US");
            req.Method = "GET";
            req.Headers.Add(string.Format("X-CSRFToken: {0}", v2.Value));
            req.Headers.Add("X-Pinterest-Referrer: http://pinterest.com/");
            req.Headers.Add("X-Requested-With: XMLHttpRequest");
            #endregion


            System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());


            var returnString = sr.ReadToEnd();




            HtmlAgilityPack.HtmlDocument Doc = new HtmlAgilityPack.HtmlDocument();
            Doc.LoadHtml(returnString);


            var scripts = Doc.DocumentNode.SelectNodes("//script[@type='text/javascript']");
            string strmarker ="";
            bool b1 = false;
            if (scripts == null)
            {

            }
            if (scripts.Count > 2)
            {
                var sSelectedNode = scripts.Where(aa => aa.InnerText.IndexOf("settings.marker") > 0);


                foreach (var item in scripts.Where(aa => aa.InnerText.IndexOf("\"marker\":") > 0))
                {
                    if (item.InnerText.IndexOf("\"marker\":") >= 0)
                    {
                        strmarker = item.InnerText;
                        b1 = true;
                    }
                }

                if (!b1) return new List<string>();

                strmarker = strmarker.Substring(strmarker.IndexOf("\"marker\":"), 28);
                strmarker = strmarker.Replace("\"marker\":", "");
                strmarker = strmarker.Replace("\"", "");
                strmarker = strmarker.Replace(";", "");
                strmarker = strmarker.Trim();
            }
            else
            {

                var sSelectedNode = scripts.Where(aa => aa.InnerText.IndexOf("settings.marker") > 0);


                foreach (var item in scripts.Where(aa => aa.InnerText.IndexOf("settings.marker") > 0))
                {
                    if (item.InnerText.IndexOf("$.pageless.settings.marker") >= 0)
                    {
                        strmarker = item.InnerText;
                        b1 = true;
                    }
                }

                if (!b1) return new List<string>();

                strmarker = strmarker.Substring(strmarker.IndexOf("\""), 20);

                strmarker = strmarker.Replace("\"", "");
                strmarker = strmarker.Replace(";", "");
                strmarker = strmarker.Trim();
            }
            




            v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
            v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };


            Url += string.Format("&marker={0}", strmarker);
            req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);
            myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);

            #region Header
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUSMSE)";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://pinterest.com/";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Headers.Add("Accept-Language: en-US");
            req.Method = "GET";
            req.Headers.Add(string.Format("X-CSRFToken: {0}", v2.Value));
            req.Headers.Add("X-Pinterest-Referrer: http://pinterest.com/");
            req.Headers.Add("X-Requested-With: XMLHttpRequest");
            #endregion


            resp = (System.Net.HttpWebResponse)req.GetResponse();
            sr = new System.IO.StreamReader(resp.GetResponseStream());


            returnString = sr.ReadToEnd();




            Doc = new HtmlAgilityPack.HtmlDocument();
            Doc.LoadHtml(returnString);




            var nodes = Doc.DocumentNode.SelectNodes("//div[@class='person odd' or @class='person']/div[@class='PersonInfo']/h4/a");

            List<string> strList = new List<string>();
            if (nodes == null) return new List<string>();
            foreach (var item in nodes)
            {
                var value = item.Attributes["href"].Value;
                value = value.Replace("/", "").Replace("\\", "");
                strList.Add(value);
            }

            return strList;
        }
        public static bool FollowUser(string username, User User)
        {
            if (IsFollowUser(username, User)) return false;

            var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            var v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
            var v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };
            var Url = string.Format("http://pinterest.com/{0}/follow/", username);
            Uri uri = new Uri(Url);

            
            


            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            CookieContainer myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);
            #region Header

            req.Accept = "application/json, text/javascript, */*; q=0.01";
            req.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            req.Headers.Add("Accept-Encoding: deflate,sdch");
            req.Headers.Add("Accept-Language: en-US,en;q=0.8,fa;q=0.6");
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";

            req.Headers.Add("Origin: http://pinterest.com");
            req.Referer = string.Format("http://pinterest.com/{0}/", username);
            req.Headers.Add("X-CLASSIC-APP: 1");
            req.Headers.Add(string.Format("X-CSRFToken:{0}", v2.Value));
            req.Headers.Add(string.Format("X-Pinterest-Referrer: http://pinterest.com/{0}/followers", username));
            

            req.Headers.Add("X-Requested-With: XMLHttpRequest");

            req.Method = "POST";

            #endregion
            //HttpWebResponse xyz = (HttpWebResponse)req.GetResponse();
            req.ContentLength = 0;
            System.Net.HttpWebResponse resp = null;
            try
            {
                resp = (System.Net.HttpWebResponse)req.GetResponse();
            }

            catch (WebException)
            {

            }

            StreamReader reader = new StreamReader(resp.GetResponseStream());
            var str = reader.ReadToEnd();

            if (str.IndexOf("\"status\": \"failure\"") >= 0)
            {
                User.HaveError = true;
                User.LastMessage = "You've reached your following limit. Your limit will increase if more people follow you. ";
                return false;
            }
            else if (str.IndexOf("success") > 0)
            {
                return true;
            }
            return false;
        }
        private static bool IsFollowUser(string username, User User)
        {
            try
            {
                var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
                var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

                var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
                var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
                var v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
                var v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };
                var Url = string.Format("http://pinterest.com/{0}/", username);
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);
                CookieContainer myCookies = new CookieContainer();
                //req.CookieContainer = new CookieContainer();
                myCookies.Add(v1);
                myCookies.Add(v2);
                myCookies.Add(v3);
                myCookies.Add(v4);
                myCookies.Add(v5);
                myCookies.Add(v6);
                req.CookieContainer = myCookies;

                req.Proxy = GetProxyForUser(User);

                #region Header
                req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUSMSE)";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = "https://pinterest.com/";
                req.Accept = "text/html, application/xhtml+xml, */*";
                req.Headers.Add("Accept-Language: en-US");
                req.Method = "GET";
                req.Headers.Add(string.Format("X-CSRFToken: {0}", v2.Value));
                req.Headers.Add("X-Pinterest-Referrer: http://pinterest.com/");
                req.Headers.Add("X-Requested-With: XMLHttpRequest");
                #endregion

                System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();

                StreamReader Reader = new StreamReader(resp.GetResponseStream());

                string strContents = Reader.ReadToEnd();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(strContents);

                var v = doc.DocumentNode.SelectSingleNode("//a[@data-text-follow='Follow All']");

                if (v == null) return false;

                if (v.InnerHtml.Trim().ToLower() == "unfollow all") return true;
                return false;
            }
            catch (Exception)
            {
                return true;
            }
            
        }
        public static void SetUserInfo(User User)
        {
            var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            var v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
            var v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };
            var Url = string.Format("http://pinterest.com/{0}/", User.pUsername);
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);
            CookieContainer myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);

            #region Header
            req.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; BOIE9;ENUSMSE)";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Referer = "https://pinterest.com/";
            req.Accept = "text/html, application/xhtml+xml, */*";
            req.Headers.Add("Accept-Language: en-US");
            req.Method = "GET";
            req.Headers.Add(string.Format("X-CSRFToken: {0}", v2.Value));
            req.Headers.Add("X-Pinterest-Referrer: http://pinterest.com/");
            req.Headers.Add("X-Requested-With: XMLHttpRequest");
            #endregion

            System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();

            StreamReader Reader = new StreamReader(resp.GetResponseStream());

            string strContents = Reader.ReadToEnd();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(strContents);

            var vFollowers = doc.DocumentNode.SelectSingleNode("//*[@id=\"ContextBar\"]/div/ul[2]/li[1]/a/strong");
            if (vFollowers != null)
            {
                SetUserFollowers(User, vFollowers.InnerText);
            }
            
            var vFollowing = doc.DocumentNode.SelectSingleNode("//*[@id=\"ContextBar\"]/div/ul[2]/li[2]/a/strong");
            if (vFollowing != null)
            {
                SetUservFollowing(User, vFollowing.InnerText);
            }
        }
        private static void SetUservFollowing(User User, string Count)
        {
            try
            {
                var VUser = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);
                VUser.Following = Convert.ToInt32(Count);
            }
            catch (Exception)
            {
            }
        }
        private static void SetUserFollowers(User User, string Count)
        {
            try
            {
                var VUser = Program.Database.Users.FirstOrDefault(aa => aa.UserName == User.UserName);
                VUser.Followers = Convert.ToInt32(Count);
            }
            catch (Exception)
            {
            }
        }

        public static bool  UnFollowUser(string username, User User)
        {
            if (!IsFollowUser(username, User)) return false;

            var v1 = User.Cookies.FirstOrDefault(aa => aa.Name == "_pinterest_sess");
            var v2 = User.Cookies.FirstOrDefault(aa => aa.Name == "csrftoken");

            var v3 = new Cookie() { Name = "component", Value = "None", Domain = v1.Domain };
            var v4 = new Cookie() { Name = "element", Value = "None", Domain = v1.Domain };
            var v5 = new Cookie() { Name = "viewParameter", Value = "USER_BOARDS", Domain = v1.Domain };
            var v6 = new Cookie() { Name = "viewType", Value = "USER", Domain = v1.Domain };
            var Url = string.Format("http://pinterest.com/{0}/follow/", username);
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Url);
            CookieContainer myCookies = new CookieContainer();
            //req.CookieContainer = new CookieContainer();
            myCookies.Add(v1);
            myCookies.Add(v2);
            myCookies.Add(v3);
            myCookies.Add(v4);
            myCookies.Add(v5);
            myCookies.Add(v6);
            req.CookieContainer = myCookies;

            req.Proxy = GetProxyForUser(User);

            #region Header
            req.Accept = "application/json, text/javascript, */*; q=0.01";
            req.Headers.Add("Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            req.Headers.Add("Accept-Encoding: deflate,sdch");
            req.Headers.Add("Accept-Language: en-US,en;q=0.8,fa;q=0.6");
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.152 Safari/537.22";
            req.Headers.Add("Origin: http://pinterest.com");
            req.Referer = string.Format("http://pinterest.com/{0}/", username);
            req.Headers.Add("X-CLASSIC-APP: 1");
            req.Headers.Add(string.Format("X-CSRFToken:{0}", v2.Value));
            req.Headers.Add(string.Format("X-Pinterest-Referrer: http://pinterest.com/{0}/", username));
            req.Headers.Add("X-Requested-With: XMLHttpRequest");
            req.KeepAlive = true;
            req.Host = "pinterest.com";
            req.Method = "POST";
            #endregion


            #region WritePostData

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("unfollow=1");
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();
            #endregion

            System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();

            StreamReader reader = new StreamReader(resp.GetResponseStream());
            var str = reader.ReadToEnd();

            if (str.IndexOf("\"status\": \"failure\"") >= 0)
            {
                User.HaveError = true;
                User.LastMessage = "You've reached your following limit. Your limit will increase if more people follow you. ";
                User.Enabled = false;
                return false;
            }
            else if (str.IndexOf("success") > 0)
            {
                return true;
            }
            return false;
        }
    }
}

