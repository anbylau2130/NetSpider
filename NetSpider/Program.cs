using JumpKick.HttpLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NetSpider.HtmlParser;
using System.Drawing;

namespace NetSpider
{
    class Program
    {
        private static Dictionary<string, string> dic = new Dictionary<string, string>();
        private static string host = "d.v6p.co";
        static void Main(string[] args)
        {
            Console.WriteLine("请输入要爬去的URL：(不要输入http://)");
            string url = "http://d.v6p.co/index.php";
            NetSpider(url);
            Console.Read();
        }

        public static void NetSpider(string  url)
        {
            
            //要爬取url
            Http.Get(url).OnSuccess((webheader, stream, response) =>
            {
                //1,获取文本内容
                var result = StreamDecoder.DecodeData(stream, response).ToLower();

                //2,存取关注数据 title..
                HtmlDocument htmldoc = new HtmlDocument();

                htmldoc.LoadHtml(result);
                  
                var title = new TitleParser(htmldoc).DoParser();
                dic[title] = url;
                Console.WriteLine(title + ":" + url);
                var urls = new UrlParser(htmldoc).DoParser();
                if (urls==null)
                {
                    return;
                }
                Queue<string> queue = new Queue<string>(urls);

                foreach (var item in queue)
                {
                    if (response.ResponseUri.Host != host)
                    {
                        return;
                    }
                    if (item.StartsWith("http"))
                    {
                        NetSpider(item.Trim());
                    }
                    else
                    {
                        string temp = "http://" + response.ResponseUri.Host + "/" + item.Trim();
                        NetSpider(temp);
                    }
                }

                //获取新的url地址 
                //过滤url地址
                //将新的url地址加入到队列中
                //循环操作
            }).OnFail(action => { 
                //状态码处理
            }).Go();

        }

    }
}
