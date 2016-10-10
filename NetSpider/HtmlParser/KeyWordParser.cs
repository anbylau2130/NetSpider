using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NetSpider.HtmlParser
{
    //关键字解析
    public class KeyWordParser
    {
        public HtmlDocument HtmlDoc { get; set; } 

        public KeyWordParser(HtmlDocument htmldoc)
        {
             this.HtmlDoc = htmldoc;
        }

        public  string DoParser()
        {
            throw new NotImplementedException();
        }
    }
}
