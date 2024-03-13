namespace SWD_ICQS.ModelsView
{
    public class BlogsView
    {
        public string? Code { get; set; }
        public int ContractorId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? PostTime { get; set; }
        public DateTime? EditTime { get; set; }
        public bool? Status { get; set; }

        public List<BlogImagesView>? blogImagesViews { get; set; }
    }
}
