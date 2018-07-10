using System;
using AutoMapper;

namespace AutoMapperDemo.Demos
{
    public static class ConvertDemo
    {
        public static void Run()
        {
            var entity = new FooEntity();
            entity.Id = Guid.NewGuid();
            entity.ParentId = null;
            entity.Name = "Foo";

            var fooDto = Mapper.DynamicMap<FooDto>(entity);
            Console.WriteLine(fooDto.ToJson());


            var fooEntity = Mapper.DynamicMap<FooEntity>(fooDto);
            Console.WriteLine(fooEntity.ToJson());


            //// Configure AutoMapper
            //Mapper.Initialize(cfg =>
            //  cfg.CreateMap<CalendarEvent, CalendarEventForm>()
            //    .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Date.Date))
            //    .ForMember(dest => dest.EventHour, opt => opt.MapFrom(src => src.Date.Hour))
            //    .ForMember(dest => dest.EventMinute, opt => opt.MapFrom(src => src.Date.Minute)));

            //// Perform mapping
            //CalendarEventForm form = Mapper.Map<CalendarEvent, CalendarEventForm>(calendarEvent);
        }
    }
}