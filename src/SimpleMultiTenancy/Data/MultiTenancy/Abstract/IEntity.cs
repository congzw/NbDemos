using System;

namespace SimpleMultiTenancy.Data.Abstract
{
    public interface IEntity
    {
        string CreatedBy { get; set; }

        DateTime? CreatedDate { get; set; }

        string UpdatedBy { get; set; }

        DateTime? UpdatedDate { get; set; }
    }
}