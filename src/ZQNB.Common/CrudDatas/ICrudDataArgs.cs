namespace ZQNB.Common.CrudDatas
{
    //create, read, update and delete
    public interface ICrudDataArgs
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        string DataType { get; set; }
    }
    public abstract class BaseCrudDataArgs : ICrudDataArgs
    {
        public virtual string DataType { get; set; }
    }
    public class CreateArgs : BaseCrudDataArgs
    {
    }
    public class GetAllArgs : BaseCrudDataArgs
    {
    }
    public class GetArgs : BaseCrudDataArgs
    {
        public object Id { get; set; }
    }
    public class UpdateArgs : BaseCrudDataArgs
    {
    }
    public class DeleteArgs : BaseCrudDataArgs
    {
    }
}