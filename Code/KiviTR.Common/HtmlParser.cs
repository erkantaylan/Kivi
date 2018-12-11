using System.Net;
using HtmlAgilityPack;

namespace KiviTR.Common
{

    public class HtmlParser
    {
        private readonly HtmlDocument htmlDoc;

        public HtmlParser(string html)
        {
            htmlDoc = new HtmlDocument();
            html = WebUtility.HtmlDecode(html);
            htmlDoc.LoadHtml(html);
        }

        public string GetHtmlInputText(string xPath)
        {
            HtmlNode singleNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);
            return singleNode.Attributes["value"].Value;
        }

        public string GetHtmlSpanText(string xPath)
        {
            return htmlDoc.DocumentNode.SelectSingleNode(xPath).InnerText;
        }
    }

}