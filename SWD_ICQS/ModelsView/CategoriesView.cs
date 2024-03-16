namespace SWD_ICQS.ModelsView
{
    public class CategoriesView
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<ConstructsView>? constructsViewList { get; set; }
    }
}
