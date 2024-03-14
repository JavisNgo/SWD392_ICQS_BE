using SWD_ICQS.Entities;

namespace SWD_ICQS.ModelsView
{
    public class ConstructsView
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int ContractorId { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? EstimatedPrice { get; set; }
        public bool? Status { get; set; }
        public CategoriesView? CategoriesView { get; set; }
        public List<ConstructImagesView>? constructImagesViews { get; set; }
        public List<ConstructProductsView>? constructProductsViews { get; set; }
    }
}
