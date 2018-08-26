using SimpleMultiTenancy.Data.Abstract;
using System;

namespace SimpleMultiTenancy.Data.Domain
{
    public abstract class Entity : IEntity
    {
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}