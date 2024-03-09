using AutoMapper;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;

namespace SWD_ICQS.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Accounts, AccountsView>().ReverseMap();
            CreateMap<Contractors, ContractorsView>().ReverseMap();
            CreateMap<Customers, CustomersView>().ReverseMap();
            CreateMap<Categories, CategoriesView>().ReverseMap();
            CreateMap<Subscriptions, SubscriptionsView>().ReverseMap();
            CreateMap<Blogs, BlogsView>().ReverseMap();
            CreateMap<BlogImages, BlogImagesView>().ReverseMap();
            CreateMap<Messages, MessagesView>().ReverseMap();    
            CreateMap<Products, ProductsView>().ReverseMap();

            CreateMap<Constructs, ConstructsView>().ReverseMap();
            CreateMap<Orders, OrdersView>().ReverseMap();

        }
    }
}
