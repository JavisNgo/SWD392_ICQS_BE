namespace SWD_ICQS.ModelsView
{
    public class DepositOrdersView
    {
        public int RequestId { get; set; }
        public double DepositPrice { get; set; }
        public DateTime? DepositDate { get; set; }
        public string? TransactionCode { get; set; }
        public DepositOrderStatusEnum? Status { get; set; }
        public enum DepositOrderStatusEnum
        {
            PENDING,
            COMPLETED,
            REJECTED,
            REFUNDED
        }

        public RequestView? RequestView { get; set; }
    }
}
