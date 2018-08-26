using System;

namespace SimpleMultiTenancy.Data.Demos
{
    public class Foo
    {
        public Foo()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}