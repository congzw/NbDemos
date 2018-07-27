using System.Collections.Generic;

namespace AceDemo.Models
{
    public class SimpleTree
    {
        public SimpleTree()
        {
            Children = new List<SimpleTree>();
        }

        public string RelationCode { get; set; }
        public string Name { get; set; }

        public IList<SimpleTree> Children { get; set; }
        public static List<SimpleTree> Create()
        {
            var treeItems = new List<SimpleTree>();
            var rootItem = new SimpleTree() { Name = "1.", RelationCode = "1." };
            treeItems.Add(rootItem);

            AppendChild(treeItems, rootItem, 2, 4);

            //add for order test
            var itemAdd = new SimpleTree() {Name = "1.3.", RelationCode = "1.3."};
            rootItem.Children.Add(itemAdd);
            treeItems.Add(itemAdd);

            return treeItems;
        }
        public static List<SimpleTree> Create(int count ,int deep)
        {
            var treeItems = new List<SimpleTree>();
            AppendChild(treeItems, null, count, deep);
            return treeItems;
        }
        private static void AppendChild(IList<SimpleTree> items, SimpleTree current, int count, int deep)
        {
            if (deep == 1 && current != null)
            {
                //最后一级处理完毕
                return;
            }

            if (current == null)
            {
                //处理一级
                for (int i = 1; i <= count; i++)
                {
                    //first level item
                    var rootItem = new SimpleTree() { Name = i + ".", RelationCode = i + "." };
                    items.Add(rootItem);
                    AppendChild(items, rootItem, count, deep);
                }
                return;
            }
            var children = CreateChild(current, count);
            foreach (var child in children)
            {
                items.Add(child);
                AppendChild(items, child, count, deep - 1);
            }

        }
        private static IList<SimpleTree> CreateChild(SimpleTree current, int childCount)
        {
            IList<SimpleTree> treeItems = new List<SimpleTree>();
            for (int i = 1; i <= childCount; i++)
            {
                var appendCode = i + ".";
                var child = new SimpleTree() { Name = current.Name + appendCode, RelationCode = current.RelationCode + appendCode };
                treeItems.Add(child);
                current.Children.Add(child);
            }
            return treeItems;
        }
    }
}
