using System;
using System.Collections.Generic;

namespace AutoMapperDemo.Demos
{
    public interface IFoo
    {
        Guid Id { get; set; }
        string Name { get; set; }
        Guid? ParentId { get; set; }
    }

    public class FooEntity : IFoo
    {
        public FooEntity()
        {
            Children = new List<FooEntity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public IList<FooEntity> Children { get; set; }
    }

    public class FooDto : IFoo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
