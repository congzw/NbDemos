using System;
using System.Collections.Generic;
using ZQNB.Common;

namespace WidgetDemo.Lib
{
    /// <summary>
    /// չʾ�õ���֯ģ��
    /// </summary>
    public class OrgDto
    {
        /// <summary>
        /// ���ڵ�Id
        /// </summary>
        public virtual Guid? ParentId { get; set; }
        /// <summary>
        /// �Ƿ���վ
        /// </summary>
        public virtual bool IsSite { get; set; }
        /// <summary>
        /// ��ϵ��
        /// </summary>
        public virtual string RelationCode { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string DistrictCode { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ��֯���ʹ���
        /// </summary>
        public string OrgTypeCode { get; set; }
        /// <summary>
        /// ��ַ
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ��ϵ��ʽ
        /// </summary>
        public string Contact { get; set; }

        #region IMapInfoPart ��Ա

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        #endregion
    }

    public class OrgDtoHelper
    {
        public static IList<OrgDto> MockOrgDtos()
        {
            var ids = GuidHelper.CreateMockGuidQueue(100);
            var orgDtos = new List<OrgDto>();
            var orgRoot = new OrgDto() { Id = ids.Dequeue(), Name = "1.", RelationCode = "1." };
            orgDtos.Add(orgRoot);

            for (int i = 1; i <= 2; i++)
            {
                var append = i + ".";
                var orgI = new OrgDto() { Id = ids.Dequeue(), Name = orgRoot.Name + append, RelationCode = orgRoot.RelationCode + append };
                orgDtos.Add(orgI);
                for (int j = 1; j <= 2; j++)
                {
                    var appendJ = j + ".";
                    var orgJ = new OrgDto() { Id = ids.Dequeue(), Name = orgI.Name + appendJ, RelationCode = orgI.RelationCode + appendJ };
                    orgDtos.Add(orgJ);
                    for (int k = 1; k <= 2; k++)
                    {
                        var appendK = k + ".";
                        var orgK = new OrgDto() { Id = ids.Dequeue(), Name = orgJ.Name + appendK, RelationCode = orgJ.RelationCode + appendK };
                        orgDtos.Add(orgK);
                    }
                }
            }
            return orgDtos;
        }

        public static IList<OrgDto> MockOrgDtos(int deep, int count)
        {
            throw new NotImplementedException("todo!");
        }
    }
}