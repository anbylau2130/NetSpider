using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NetSpider.HtmlParser
{
    //标题解析
    public class TitleParser 
    {
        public HtmlDocument HtmlDoc { get; set; } 

        public TitleParser(HtmlDocument htmldoc)
        {
            HtmlDoc = htmldoc;
        }

        public  string DoParser()
        {
            var tem = this.HtmlDoc.DocumentNode.SelectSingleNode("//title");
            if (tem!=null)
            {
                return this.HtmlDoc.DocumentNode.SelectSingleNode("//title").InnerHtml;
            }
            return string.Empty;
        }
    }
}
