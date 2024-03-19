namespace SWD_ICQS.ModelsView
{
    public class ContractsView
    {
        public int Id { get; set; } 
        public int RequestId { get; set; } // Thêm thuộc tính RequestId
        public string? ContractUrl { get; set; }
        public string? Progress { get; set; }
        public int? Status { get; set; }
    }
}
