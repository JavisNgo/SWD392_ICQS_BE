namespace SWD_ICQS.ModelsView
{
    public class ProductsView
    {
        public int Id { get; set; }
        public int ContractorId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public bool? Status { get; set; }

        public List<ProductImagesView>? productImagesViews { get; set; }
    }
}
