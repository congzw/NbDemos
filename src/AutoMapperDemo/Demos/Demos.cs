using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapperDemo.Demos.SimpleProjects;

namespace AutoMapperDemo.Demos
{
    public static class ConvertDemo
    {
        public static void Run()
        {
            //ForAutoMapper();
            ForSimpleProject();
        }

        private static void ForAutoMapper()
        {
            var entity = new FooEntity();
            entity.Id = Guid.NewGuid();
            entity.ParentId = null;
            entity.Name = "Foo";

            var fooDto = Mapper.DynamicMap<FooDto>(entity);
            Console.WriteLine(fooDto.ToJson());


            var fooEntity = Mapper.DynamicMap<FooEntity>(fooDto);
            Console.WriteLine(fooEntity.ToJson());
        }

        private static void ForSimpleProject()
        {
            var query = Enumerable.Range(1, 10).Select(x =>
            {
                var entity = new FooEntity();
                entity.Id = Guid.NewGuid();
                entity.ParentId = null;
                entity.Name = "Foo_" + x.ToString("00");
                return entity;
            }).AsQueryable();

            var fooDtos = query.SimpleProject().To<FooDto>();
            Console.WriteLine(fooDtos.ToJson());

            //var fooDtos = query.ProjectTo<FooEntity, FooDto>();
            //Console.WriteLine(fooDtos.ToJson());


            //var fooEntity = Mapper.DynamicMap<FooEntity>(fooDto);
            //Console.WriteLine(fooEntity.ToJson());
        }

    }
}