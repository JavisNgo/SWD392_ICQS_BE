using static SWD_ICQS.Entities.Requests;

namespace SWD_ICQS.ModelsView
{
    public class RequestViewForGet
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractorName { get; set; }
        public string? Note { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public RequestsStatusEnum? Status { get; set; }
        public List<RequestDetailView>? requestDetailViews { get; set; }

    }
}
