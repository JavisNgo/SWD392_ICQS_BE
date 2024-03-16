namespace SWD_ICQS.ModelsView
{
    public class RequestDetailView
    {
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductsView? ProductView { get; set; }
    }
}
