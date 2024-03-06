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
            CreateMap<Categories, CategoriesView>().ReverseMap();
            CreateMap<Subscriptions, SubscriptionsView>().ReverseMap();

        }
    }
}
