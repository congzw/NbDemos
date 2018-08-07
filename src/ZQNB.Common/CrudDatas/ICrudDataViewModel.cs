using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using ZQNB.Common.Data.Model;

namespace ZQNB.Common.CrudDatas
{
    /// <summary>
    /// 文件导入模型
    /// </summary>
    public interface ICrudDataViewModel : INbEntity<Guid>
    {
        /// <summary>
        /// 对应的数据类型
        /// </summary>
        /// <returns></returns>
        string GetTypeName();

        /// <summary>
        /// 获取数据的列信息
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        IList<ClassColumnInfo> GetColumnInfos();

        IList<object> GetAllObjects();

        object GetObject(Guid id);

        MessageResult AddObject(object entity);

        MessageResult UpdateObject(object entity);

        MessageResult Delete(Guid id);

        void Flush();
    }

    public interface ICrudDataViewModel<TViewModel> : ICrudDataViewModel
        where TViewModel : ICrudDataViewModel, new()
    {
        IList<TViewModel> GetAll();

        TViewModel Get(Guid id);

        MessageResult Add(TViewModel vo);

        MessageResult Update(TViewModel vo);
    }

    /// <summary>
    /// 支持上传数据、解析验证、数据预览、模板下载的数据模型（Excel等）
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseCrudDataViewModel<TViewModel, TEntity> : ICrudDataViewModel<TViewModel>
        where TViewModel : ICrudDataViewModel, new()
        where TEntity : INbEntity<Guid>
    {
        public Guid Id { get; set; }

        public virtual IList<ClassColumnInfo> GetColumnInfos()
        {
            var metas = ClassColumnInfo.Create(GetType());
            return metas;
        }

        public IList<object> GetAllObjects()
        {
            var datas = GetAll();
            var objects = CastToObjects(datas);
            return objects;
        }

        public object GetObject(Guid id)
        {
            var data = Get(id);
            return data;
        }

        public MessageResult AddObject(object entity)
        {
            var result = Add((TViewModel)entity);
            return result;
        }

        public MessageResult UpdateObject(object entity)
        {
            var result = Update((TViewModel)entity);
            return result;
        }

        public MessageResult Delete(Guid id)
        {
            var messageResult = new MessageResult();
            if (id == Guid.Empty)
            {
                messageResult.Success = false;
                messageResult.Message = string.Format("记录{0}不存在", id);
                return messageResult;
            }


            var session = GetSession();
            var theOne = session.Get<TEntity>(id);
            if (theOne == null)
            {
                messageResult.Success = false;
                messageResult.Message = string.Format("记录{0}不存在", id);
                return messageResult;
            }

            session.Delete(theOne);
            messageResult.Success = true;
            messageResult.Message = "删除成功";
            messageResult.Data = id;
            return messageResult;
        }

        public void Flush()
        {
            var session = GetSession();
            session.Flush();
        }

        public virtual string GetTypeName()
        {
            return this.GetType().Name;
        }
        
        public IList<TViewModel> GetAll()
        {
            var session = GetSession();
            var list = session.Query<TEntity>().ToMappedList<TViewModel>();
            return list;
        }

        public TViewModel Get(Guid id)
        {
            var session = GetSession();
            var theOne = session.Get<TEntity>(id).ToMapped<TViewModel>();
            return theOne;
        }

        public MessageResult Add(TViewModel vo)
        {
            var messageResult = new MessageResult();
            if (vo == null)
            {
                messageResult.Success = false;
                messageResult.Message = "记录不能未空";
                return messageResult;
            }

            //_session.SaveOrUpdate();

            var instance = Activator.CreateInstance(typeof (TEntity));
            vo.TryCopyTo(instance);

            var session = GetSession();
            var entity = (TEntity)instance;
            if (entity.Id == Guid.Empty)
            {
                var theOne = session.Save(entity);
            }
            else
            {
                session.Save(entity, entity.Id);
            }

            messageResult.Success = true;
            messageResult.Message = "添加成功";
            messageResult.Data = entity.Id;
            return messageResult;
        }

        public MessageResult Update(TViewModel vo)
        {
            var messageResult = new MessageResult();
            if (vo == null)
            {
                messageResult.Success = false;
                messageResult.Message = "对应记录不能未空";
                return messageResult;
            }
            if (vo.Id == Guid.Empty)
            {
                messageResult.Success = false;
                messageResult.Message = "对应记录Id不能未空: " + vo.Id;
                return messageResult;
            }

            var session = GetSession();
            var entity = session.Get<TEntity>(vo.Id);
            if (entity == null)
            {
                messageResult.Success = false;
                messageResult.Message = "没有找到记录: " + vo.Id;
                return messageResult;
            }

            vo.TryCopyTo(entity, "Id");

            session.Update(entity);

            messageResult.Success = true;
            messageResult.Message = "修改成功";
            messageResult.Data = entity.Id;
            return messageResult;
        }

        //helpers

        protected ISession GetSession()
        {
            //todo
            return null;
            //var session = CoreServiceProvider.LocateService<ISession>();
            //return session;
        }

        private IList<object> CastToObjects(IList<TViewModel> list)
        {
            var objects = list.Cast<object>().ToList();
            return objects;
        }
    }
}
