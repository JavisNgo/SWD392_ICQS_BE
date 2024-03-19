namespace SWD_ICQS.ModelsView
{
    public class ContractViewForGet
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractorName { get; set; }
        public string? ContractUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? EditDate { get; set; }
        public int? Status { get; set; }
        public string? Progress { get; set; }

    }
}
