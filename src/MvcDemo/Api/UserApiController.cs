using System.Web.Http;
using MvcDemo.Api.AppServices;
using MvcDemo.Api.Libs;

namespace MvcDemo.Api
{
    public class UserApiController : ApiController
    {
        private readonly IUserAppService _userAppService = MyFactory.CreateUserAppService();
        
        public UserVo GetUser(int id)
        {
            return _userAppService.GetUser(id);
        }

        [HttpPost]
        public string SaveUser(UserVo vo)
        {
            var objectHash = ObjectHashHelper.CreateObjectHash(vo);
            return objectHash;
        }

        #region DictionaryVo OK

        public HashedVo GetUserDic(int id)
        {
            return _userAppService.GetUserDic(id);
        }

        [HttpPost]
        public string SaveUserDic(HashedVo vo)
        {
            var objectHash = ObjectHashHelper.CreateObjectHash(vo);
            return objectHash;
        }

        #endregion

        #region IDictionaryWithHash NG!

        [HttpGet]
        public IHashedObjects GetUserDicHash(int id)
        {
            //OK
            return _userAppService.GetUserDicHash(id);
        }

        [HttpPost]
        public string SaveUserDicHash(IHashedObjects vo)
        {
            //NG! post not null but receive null
            var objectHash = ObjectHashHelper.CreateObjectHash(vo);
            return objectHash;
        }

        #endregion

        #region dynamic NG!

        ////需要自行处理序列化，不合适！
        //public dynamic GetUserDynamic(int id)
        //{
        //    return _userAppService.GetUserDynamic(id);
        //}


        #endregion
    }
}
