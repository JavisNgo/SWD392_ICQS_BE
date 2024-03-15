using static SWD_ICQS.Entities.Requests;

namespace SWD_ICQS.ModelsView
{
    public class RequestView
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ContractorId { get; set; }
        public string? Note { get; set; }
        public double? TotalPrice { get; set; }
        public RequestsStatusEnum? Status { get; set; }
        public List<RequestDetailView>? requestDetailViews { get; set; }


    }
}
