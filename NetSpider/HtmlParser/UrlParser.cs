using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NetSpider.HtmlParser
{
    public class UrlParser
    {
        public HtmlDocument HtmlDoc { get; set; } 

        public UrlParser(HtmlDocument htmldoc)
        {
            HtmlDoc = htmldoc;
        }

        public  List<string> DoParser()
        {
            List<string> urls = new List<string>();
            var htmlnodeCollection=this.HtmlDoc.DocumentNode.SelectNodes("//a");
            if (htmlnodeCollection==null)
            {
                return null;
            }
            var result = (from htmlnode in htmlnodeCollection
                where
                    htmlnode.HasAttributes &&
                    !htmlnode.GetAttributeValue("href", "#").Contains("#") &&
                    !htmlnode.GetAttributeValue("href", "#").Contains("javascript")&&
                    htmlnode.GetAttributeValue("href","#").Length>2
                select htmlnode.GetAttributeValue("href", "#").Trim()).ToList();

            return result;
        }
    }
}
