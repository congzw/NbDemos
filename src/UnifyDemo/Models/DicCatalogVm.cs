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


    public class DicCatalogItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        //hiddenByRelation(vm, vm.grades, shouldShowPhaseSubjectGrade);
        public Func<bool, IDicCatalogVm, DicCatalogItem> Func { get; set; }
    }

    public class DicItemCategory
    {
        public string key { get; set; }
        public string name { get; set; }
        public string itemsKey { get; set; }
        public Func<bool, IDicCatalogVm> ShouldShow { get; set; }
    }

    public class RelationHashtable : Dictionary<string, bool>
    {
        public bool HasRelation(params string[] codes)
        {
            return false;
            //todo
        }
        public void AddRelation(params string[] codes)
        {
            //todo
        }


    }
}
