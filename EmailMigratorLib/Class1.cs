using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using HtmlAgilityPack.Samples;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

namespace EmailMigratorLib
{
    public class GoDaddy
    {
        string _user;
        string _domain;
        string _password;

        string server = "";
        CookieCollection session = null;


        public GoDaddy(string user, string domain, string password)
        {
            _user = user;
            _domain = domain;
            _password = password;            
        }

        public bool Login()
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("https://login.secureserver.net/index.php?app=wbe&preview=0");
            myHttpWebRequest.Method = "POST";
            
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.10) Gecko/20100919 Firefox/3.6.10";
            myHttpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            myHttpWebRequest.Headers["Accept-Language"] = "en-us";
            myHttpWebRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            myHttpWebRequest.Referer = "https://login.secureserver.net/index.php?app=wbe";
            myHttpWebRequest.CookieContainer = new CookieContainer();

            // Set the content type of the data being posted.
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";

            string postData = "return_app=wbe&username=" + _user + "&loginlist=" + _user + "%40" + _domain + "&password=" + _password;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);            

            // Set the content length of the string being posted.
            myHttpWebRequest.ContentLength = byte1.Length;

            Stream newStream = myHttpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            newStream.Close();

            HttpWebResponse r = (HttpWebResponse) myHttpWebRequest.GetResponse();

            bool loggedin = false;
            server = "";
            session = new CookieCollection();
            if(r.StatusCode == HttpStatusCode.OK)
            {                
                if(r.ResponseUri.Host != "login.secureserver.net")
                {
                    server = r.ResponseUri.Host;
                    session = r.Cookies;                
                    loggedin = true;
                }
            }

            r.Close();
            return loggedin;
        }

        public List<string> GetFolders()
        {            
            HtmlDocument homepage = GetHtmlDocument("http://" + server + "/webmail.php?folder=INBOX");

            XPathNavigator navigator = homepage.CreateNavigator();
            XPathExpression expression = navigator.Compile("//*[@id=\"foldertree_INBOX\"]//li");
            XPathNodeIterator nodes = navigator.Select(expression);

            List<string> folders = new List<string>();

            folders.Add("INBOX");
            while (nodes.MoveNext())
            {
                if (nodes.Current.GetAttribute("class", "") != "separator")
                {
                    string folder = nodes.Current.GetAttribute("id", "").Replace("foldertree_", "");                   
                    folders.Add(folder);
                }
            }

            return folders;
        }

        public List<int> GetMessageUids(string folder)
        {
            string reply =
                PostAJAX("http://" + server + "/pajax_call_dispatcher.php?class=AJAXWebmail&method=index", 
                "{\"className\": \"AJAXWebmail\", \"method\": \"index\", \"params\": [\"" + folder + "\", null, {\"msg_per_page\": \"5000\", \"in_search\": false}]}");
            
            JObject o = JObject.Parse(reply);
            string messages = (string)o["tbody"];

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(messages);

            List<int> messageids = new List<int>();
            
            XPathNavigator navigator = doc.CreateNavigator();
            XPathExpression expression = navigator.Compile("//tr");
            XPathNodeIterator nodes = navigator.Select(expression);

            while (nodes.MoveNext())
            {
                if (nodes.Current.GetAttribute("afrom", "") != null)
                {
                    string id = nodes.Current.GetAttribute("id", "");
                    string[] ids = id.Split(new Char[] { '|' });
                    messageids.Add(Convert.ToInt32(ids[1]));
                }
            }

            return messageids;
        }

        public byte[] GetMessage(string folder, int id)
        {
            string reply =
                PostAJAX("http://" + server + "/pajax_call_dispatcher.php?class=AJAXMsgCache&method=getFields",
                "{\"className\": \"AJAXMsgCache\", \"method\": \"getFields\", \"params\": [{\"raw\": true, \"fullheaders\": false, \"folder\": \"" + folder + "\", \"uid\": \"" + id.ToString() + "\", \"secure_fetch\": true, \"num_inlineimgs\": 0}, \"" + id.ToString() + "|" + folder + "\", [\"raw\"], true]}");
            
            JObject o = JObject.Parse(reply);
            string message = (string)o["raw_body"];

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(message);

            StringWriter buffer = new StringWriter();
            HtmlToText htt = new HtmlToText();
            htt.ConvertTo(doc.DocumentNode, buffer);
            
            
            string blah = buffer.ToString();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(blah);

            return byte1;
        }

        static int CopyStream(Stream source, Stream dest)
        {
            byte[] buffer = new byte[65536];
            int total = 0;
            int read;
            do
            {
                read = source.Read(buffer, 0, buffer.Length);
                total += read;
                dest.Write(buffer, 0, read);
            } while (read != 0);
            return total;
        }

        HtmlAgilityPack.HtmlDocument GetHtmlDocument(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);                
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.14) Gecko/2009101001 Firefox/3.0.14 (.NET CLR 3.5.30729)";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Headers.Add("Accept-Language", "en-us");
                req.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(session);
                req.KeepAlive = false;

                HttpWebResponse result = (HttpWebResponse)req.GetResponse();
                Stream ReceiveStream = result.GetResponseStream();
                MemoryStream m = new MemoryStream();
                CopyStream(ReceiveStream, m);

                Encoding encoding = new UTF8Encoding();
                string webpage = encoding.GetString(m.ToArray());
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(webpage);

                return doc;
            }
            catch (Exception)
            {                
                return null;
            }
        }

        string PostAJAX(string url, string postData)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.14) Gecko/2009101001 Firefox/3.0.14 (.NET CLR 3.5.30729)";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Headers.Add("Accept-Language", "en-us");
                req.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(session);
                req.KeepAlive = false;

                req.ContentType = "text/json; charset=utf-8";
                req.ContentLength = postData.Length;

                Encoding encoding = new UTF8Encoding();
                byte[] byte1 = encoding.GetBytes(postData);

                // Set the content length of the string being posted.
                req.ContentLength = byte1.Length;

                Stream newStream = req.GetRequestStream();
                newStream.Write(byte1, 0, byte1.Length);
                newStream.Close();

                HttpWebResponse result = (HttpWebResponse)req.GetResponse();
                Stream ReceiveStream = result.GetResponseStream();
                MemoryStream m = new MemoryStream();
                CopyStream(ReceiveStream, m);
                
                return encoding.GetString(m.ToArray());                
            }
            catch (Exception)
            {
                return null;
            }
        }
        
    }
}
