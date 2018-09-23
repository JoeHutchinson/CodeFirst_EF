namespace CountVonCount.Controllers
{
    public sealed class FormData
    {
        public string url { get; set; }

        public override string ToString()
        {
            return $"url:{url}";
        }
    }
}