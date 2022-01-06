namespace BasicWebAppDemo.Controllers
{
    public class ODataQueryResponse
    {
        public int Val { get; set; }

        public string TestStr { get; set; }

        public string Filter { get; set; }

        public string OrderBy { get; set; }

        public string SkipToken { get; set; }
    }
}