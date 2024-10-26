namespace View.grpc.Models.Response
{
    public class ViewAccessList
    {
        public IList<ViewAccessModel> Items { get; set; } = new List<ViewAccessModel>();
        public bool HasMore { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        public IList<ViewLinks> Links { get; set; } = new List<ViewLinks>();
    }
}
