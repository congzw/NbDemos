using AutoMapper;

namespace AutoMapperDemo.Demos
{
    public class Config
    {
        //todo move auto mapper to common fx
        public static void InitAutoMapper()
        {
            //Mapper.Initialize(cfg => cfg.CreateMap<Order, OrderDto>());
            ////or
            //var config = new MapperConfiguration(cfg => cfg.CreateMap<Order, OrderDto>());


            //Mapper.Initialize(cfg => cfg.CreateMap<FooEntity, FooDto>());
            //Mapper.Initialize(cfg => cfg.CreateMap<FooDto, FooEntity>());
            
            //Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
            //Mapper.Initialize(cfg => cfg.AddProfile(new MappingProfile()));
        }
    }

    //public class MappingProfile : Profile
    //{
    //    public MappingProfile()
    //    {
    //        CreateMap<FooEntity, FooDto>();
    //        //this.RecognizePrefixes("m_");

    //        //// Self-referential mapping
    //        //cfg.CreateMap<Category, CategoryDto>().MaxDepth(3);

    //        //// Circular references between users and groups
    //        //cfg.CreateMap<User, UserDto>().PreserveReferences();

    //        //cfg.CreateMap<Source, Destination>()
    //        //.ForMember(d => d.Child, opt => opt.UseDestinationValue());
    //    }
    //}

    //// Flattens with NameSplitMember
    //// Only applies to types that have same name with destination ending with Dto
    //// Only applies Dto post fixes to the source properties
    //public class ToDTO : Profile
    //{
    //    protected override void Configure()
    //    {
    //        AddMemberConfiguration().AddMember<NameSplitMember>().AddName<PrePostfixName>(
    //                _ => _.AddStrings(p => p.Postfixes, "Dto"));
    //        AddConditionalObjectMapper().Where((s, d) => s.Name == d.Name + "Dto");
    //    }
    //}

    //// Doesn't Flatten Objects
    //// Only applies to types that have same name with source ending with Dto
    //// Only applies Dto post fixes to the destination properties
    //public class FromDTO : Profile
    //{
    //    protected override void Configure()
    //    {
    //        AddMemberConfiguration().AddName<PrePostfixName>(
    //                _ => _.AddStrings(p => p.DestinationPostfixes, "Dto"));
    //        AddConditionalObjectMapper().Where((s, d) => d.Name == s.Name + "Dto");
    //    }
    //}


}
