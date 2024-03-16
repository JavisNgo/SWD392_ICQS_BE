namespace SWD_ICQS.ModelsView
{
    public class ConstructProductsView
    {
        public int ProductId { get; set; }
        public int ConstructId { get; set; }
        public int Quantity { get; set; }
        public ProductsView? ProductsView { get; set; }
    }
}
