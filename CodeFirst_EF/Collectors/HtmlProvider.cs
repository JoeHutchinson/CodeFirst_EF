using HtmlAgilityPack;

namespace CodeFirst_EF.Collectors
{
    public class HtmlProvider : HtmlWeb, IHtmlProvider
    {
    }

    public interface IHtmlProvider
    {
        HtmlDocument Load(string url);
    }
}
