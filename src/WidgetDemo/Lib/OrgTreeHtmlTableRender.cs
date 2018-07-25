using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace WidgetDemo.Lib
{
    /// <summary>
    /// 组织树用表格展示
    /// </summary>
    public class OrgTreeHtmlTableRender
    {
        /// <summary>
        /// 把带有RelationCode的组织列表，转化成HtmlTable可展示的string
        /// </summary>
        /// <param name="sortedOrgs"></param>
        /// <param name="tableCssClass"></param>
        /// <returns></returns>
        public string MakeTableHtmlString(IList<OrgDto> sortedOrgs, string tableCssClass = "table table-bordered table-hover")
        {
            int maxDeep = sortedOrgs.Max(x => x.RelationCode.Split('.').Length);

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                //---------table start--------
                writer.AddAttribute(HtmlTextWriterAttribute.Class, tableCssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Table); // Begin Table

                WriteThead(writer, maxDeep);
                WriteTbody(writer, maxDeep, sortedOrgs);

                writer.RenderEndTag(); // Begin Table
                //---------table end--------
            }

            string result = stringWriter.ToString();
            return result;
        }

        private static void WriteThead(HtmlTextWriter writer, int maxDeep)
        {
            //---------thead start--------
            //    <thead>
            //        <tr>
            //            <th class="right">#关系编号</th>
            //            <th>组织(根)</th>
            //            <th>组织(2级)</th>
            //            <th>组织(3级)</th>
            //            <th>组织(4级)</th>
            //            <th class="text-center col-xs-?">操作</th>
            //        </tr>
            //    </thead>
            writer.RenderBeginTag(HtmlTextWriterTag.Thead); // Begin Thead
            writer.RenderBeginTag(HtmlTextWriterTag.Tr); // Begin Tr

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-right");
            writer.RenderBeginTag(HtmlTextWriterTag.Th); // Begin 0 Th
            writer.Write("#关系编号");
            writer.RenderEndTag(); // End  0 Th

            for (int i = 0; i < maxDeep; i++)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Th); // Begin i Th
                if (i == 0)
                {
                    writer.WriteEncodedText(string.Format("根组织（{0}级）", i + 1));
                }
                else
                {
                    writer.WriteEncodedText(string.Format("组织（{0}级）", i + 1));
                }
                writer.RenderEndTag(); // End  i Th
            }

            ////<th class="text-center col-xs-2">操作</th>
            //int theColSpan = 12 / (maxDeep + 1 + 1);
            //writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-center col-xs-" + (theColSpan < 3 ? 3 : theColSpan));

            //<th class="text-center">操作</th>
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-center");
            writer.RenderBeginTag(HtmlTextWriterTag.Th); // Begin i Th
            writer.WriteEncodedText("操作");
            writer.RenderEndTag(); // End  i Th

            writer.RenderEndTag(); // End Tr
            writer.RenderEndTag(); // End Thead
            //---------thead end--------
        }

        private static void WriteTbody(HtmlTextWriter writer, int maxDeep, IList<OrgDto> sortedOrgs)
        {
            var rowCount = sortedOrgs.Count;

            //---------tbody start--------
            //    <tbody>
            //        <tr>
            //            <td class="right">1</td>
            //            <td>山东省教育厅</td>
            //            <td></td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td class="right">1.1</td>
            //            <td>威海市教育局</td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td></td>
            //            <td class="right">1.1.1</td>
            //            <td>环翠区</td>
            //            <td></td>
            //        </tr>
            //    </tbody>
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody); // Begin Thead

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var theOrgDto = sortedOrgs[rowIndex];
                int relationCodeIndex = theOrgDto.RelationCode.Split('.').Length - 1;

                writer.RenderBeginTag(HtmlTextWriterTag.Tr); // Begin Tr

                for (int colIndex = 0; colIndex < maxDeep + 1; colIndex++)
                {
                    if (colIndex == 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-right");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td
                        writer.WriteEncodedText(theOrgDto.RelationCode);
                        writer.RenderEndTag(); // End Td
                    }

                    //if (colIndex < relationCodeIndex)
                    //{
                    //    writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td
                    //    writer.RenderEndTag(); // End Td
                    //}


                    if (colIndex == relationCodeIndex - 1)
                    {
                        var theRelationCodeDeep = theOrgDto.RelationCode.Split('.').Length - 1;
                        if (theRelationCodeDeep != 1)
                        {
                            writer.AddAttribute("colspan", theRelationCodeDeep.ToString());
                        }
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td
                        writer.RenderEndTag(); // End Td
                    }

                    if (colIndex == relationCodeIndex)
                    {
                        int rolSpan = maxDeep - colIndex;
                        if (rolSpan != 1)
                        {
                            writer.AddAttribute("colspan", rolSpan.ToString());
                        }

                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "active");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td
                        writer.WriteEncodedText(theOrgDto.Name);
                        writer.RenderEndTag(); // End Td
                    }
                }

                WriteOpTds(writer, theOrgDto);

                writer.RenderEndTag(); // End Tr
            }
            writer.RenderEndTag(); // End tbody
            //---------tbody end--------
        }

        //----helpers----
        private static void WriteOpTds(HtmlTextWriter writer, OrgDto theOrgDto)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            //--------buttons start-------
            //<button class="btn btn-info btn-xs" ng-click="create('abc')" title="创建子节点"><i class="fa fa-plus"></i> 创建</button>
            //<button class="btn btn-danger btn-xs" ng-click="delete('abc')"><i class=" fa fa-trash-o"></i> 删除</button>
            //<button class="btn btn-warning btn-xs" ng-click="edit('abc')><i class="fa fa-pencil"></i> 修改</button>
            //<button class="btn btn-primary btn-xs" ng-click="show('abc')><i class="fa fa-search"></i> 详情</button>
            WriteButton(writer, theOrgDto, "btn btn-info btn-xs", string.Format("create('{0}')", theOrgDto.Id), "fa fa-trash-o", "创建", "点击创建子节点");
            WriteButton(writer, theOrgDto, "btn btn-danger btn-xs", string.Format("delete('{0}')", theOrgDto.Id), "fa fa-trash-o", "修改", "点击修改此节点");
            WriteButton(writer, theOrgDto, "btn btn-warning btn-xs", string.Format("edit('{0}')", theOrgDto.Id), "fa fa-pencil", "删除", "点击删除此节点");
            WriteButton(writer, theOrgDto, "btn btn-primary btn-xs", string.Format("show('{0}')", theOrgDto.Id), "fa fa-search", "详情", "点击查看详细");

            writer.RenderEndTag();
            //--------buttons end-------
        }
        private static void WriteButton(HtmlTextWriter writer, OrgDto theOrgDto, string buttonClass, string methodClick, string iconClass, string buttonText, string title = null)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, buttonClass);
            //writer.AddAttribute("orgId", theOrgDto.Id.ToString());
            writer.AddAttribute("ng-click", methodClick, false);
            writer.AddAttribute(HtmlTextWriterAttribute.Title, title);
            writer.RenderBeginTag(HtmlTextWriterTag.Button);

            //<i class="fa fa-trash-o"></i>
            writer.AddAttribute(HtmlTextWriterAttribute.Class, iconClass);
            writer.RenderBeginTag(HtmlTextWriterTag.I);
            writer.RenderEndTag();

            writer.WriteEncodedText(buttonText);
            writer.RenderEndTag();
        }

        private static void WriteTbody3(HtmlTextWriter writer, int maxDeep, IList<OrgDto> sortedOrgs)
        {
            var rowCount = sortedOrgs.Count;

            //---------tbody start--------
            //    <tbody>
            //        <tr>
            //            <td class="right">1</td>
            //            <td>山东省教育厅</td>
            //            <td></td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td class="right">1.1</td>
            //            <td>威海市教育局</td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td></td>
            //            <td class="right">1.1.1</td>
            //            <td>环翠区</td>
            //            <td></td>
            //        </tr>
            //    </tbody>
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody); // Begin Thead

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var theOrgDto = sortedOrgs[rowIndex];
                int relationCodeIndex = theOrgDto.RelationCode.Split('.').Length - 1;

                writer.RenderBeginTag(HtmlTextWriterTag.Tr); // Begin Tr

                for (int colIndex = 0; colIndex < maxDeep + 1; colIndex++)
                {
                    if (colIndex == 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-right");
                    }

                    writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td

                    if (colIndex == 0)
                    {
                        writer.WriteEncodedText(theOrgDto.RelationCode);
                    }

                    if (colIndex == relationCodeIndex + 1)
                    {
                        writer.WriteEncodedText(theOrgDto.Name);
                    }

                    writer.RenderEndTag(); // End Td
                }
                writer.RenderEndTag(); // End Tr
            }
            writer.RenderEndTag(); // End tbody
            //---------tbody end--------
        }
        private static void WriteTbody2(HtmlTextWriter writer, int maxDeep, IList<OrgDto> sortedOrgs)
        {
            var rowCount = sortedOrgs.Count;

            //---------tbody start--------
            //    <tbody>
            //        <tr>
            //            <td class="right">1</td>
            //            <td>山东省教育厅</td>
            //            <td></td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td class="right">1.1</td>
            //            <td>威海市教育局</td>
            //            <td></td>
            //            <td></td>
            //        </tr>
            //        <tr>
            //            <td></td>
            //            <td></td>
            //            <td class="right">1.1.1</td>
            //            <td>环翠区</td>
            //            <td></td>
            //        </tr>
            //    </tbody>
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody); // Begin Thead

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var theOrgDto = sortedOrgs[rowIndex];
                int relationCodeIndex = theOrgDto.RelationCode.Split('.').Length - 1;

                writer.RenderBeginTag(HtmlTextWriterTag.Tr); // Begin Tr

                for (int colIndex = 0; colIndex < maxDeep + 1; colIndex++)
                {
                    if (colIndex == relationCodeIndex)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-right");
                    }

                    writer.RenderBeginTag(HtmlTextWriterTag.Td); // Begin Td

                    if (colIndex == relationCodeIndex)
                    {
                        writer.WriteEncodedText(theOrgDto.RelationCode);
                    }

                    if (colIndex == relationCodeIndex + 1)
                    {
                        writer.WriteEncodedText(theOrgDto.Name);
                    }

                    writer.RenderEndTag(); // End Td
                }
                writer.RenderEndTag(); // End Tr
            }
            writer.RenderEndTag(); // End tbody
            //---------tbody end--------
        }

        static OrgTreeHtmlTableRender()
        {
            Instance = new OrgTreeHtmlTableRender();
        }
        public static OrgTreeHtmlTableRender Instance { get; set; }
    }
}
