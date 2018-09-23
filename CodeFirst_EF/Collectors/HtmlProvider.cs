using HtmlAgilityPack;

namespace CodeFirst_EF.Collectors
{
    public sealed class HtmlProvider : HtmlWeb, IHtmlProvider
    {
    }

    public interface IHtmlProvider
    {
        HtmlDocument Load(string url);
    }
}
