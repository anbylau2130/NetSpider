using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NetSpider
{
   public static class StreamDecoder
    {
       public static string DecodeData(Stream responseStream, HttpWebResponse response)
       {
           //先从content-type取
           string encodingname = null;
           string contenttype = response.Headers["content-type"];
           if (contenttype != null)
           {
               int index = contenttype.IndexOf("charset=");
               if (index != -1)
               {
                   encodingname = contenttype.Substring(index + 8);
               }
           }

           MemoryStream stream = new MemoryStream();
           byte[] buffer = new byte[0x400];
           for (int i = responseStream.Read(buffer, 0, buffer.Length); i > 0; i = responseStream.Read(buffer, 0, buffer.Length))
           {
               stream.Write(buffer, 0, i);
           }
           responseStream.Close();

           if (encodingname == null)
           {
               MemoryStream streamtemp = stream;
               streamtemp.Seek((long)0, SeekOrigin.Begin);
               string text = new StreamReader(streamtemp, Encoding.ASCII).ReadToEnd();
               HtmlDocument htmldoc = new HtmlDocument();
               htmldoc.LoadHtml(text.ToLower());
               var contentNode = htmldoc.DocumentNode.SelectSingleNode("//meta[@http-equiv='content-type']");
               if (contentNode != null)
               {
                   var content = contentNode.GetAttributeValue("content", "");
                   encodingname = content.Substring(content.IndexOf("="), content.Length - content.IndexOf("=")).TrimStart('=');
               }
           }
           Encoding encoding = null;
            try
            {
                if (encodingname == "GBK")
                {
                    encodingname = "GB2312";
                }
                encoding = Encoding.GetEncoding(encodingname);
            }
            catch
            {
                encoding = Encoding.GetEncoding("gb2312");
            }
           stream.Seek((long)0, SeekOrigin.Begin);
           StreamReader reader = new StreamReader(stream, encoding);
           return reader.ReadToEnd();
       } 
    }
}
