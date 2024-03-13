namespace SWD_ICQS.ModelsView
{
    public class ContractsView
    {
        public int RequestId { get; set; } // Thêm thuộc tính RequestId
        public int AppointmentId { get; set; }
        public string? ContractUrl { get; set; }
        public int? Status { get; set; }
    }
}
