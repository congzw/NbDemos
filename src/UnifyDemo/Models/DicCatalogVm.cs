using System;
using System.Collections.Generic;

namespace UnifyDemo.Models
{
    public interface IDicCatalogVm
    {
        //object Items { get; set; }
        //object Relations { get; set; }

        DicCatalogSelectResult selectResult { get; set; }
        bool isEmptyItems(string category);
        //vm.resultChanged(category, item, oldItem);
        Action<string, DicCatalogSelectResultItem, DicCatalogSelectResultItem> resultChanged { get; set; }
    }
    public class DicCatalogSelectResult : Dictionary<String, DicCatalogSelectResultItem>
    {
    }
    public class DicCatalogSelectResultItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
