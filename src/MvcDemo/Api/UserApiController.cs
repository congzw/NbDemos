using System.Web.Http;
using MvcDemo.Api.AppServices;

namespace MvcDemo.Api
{
    public class UserApiController : ApiController
    {
        private readonly IUserAppService _userAppService = MyFactory.CreateUserAppService();
        public UserVo GetUser(int id)
        {
            return _userAppService.GetUser(id);
        }

        public dynamic GetUserDynamic(int id)
        {
            return _userAppService.GetUserDynamic(id);
        }

        public DictionaryVo GetUserDic(int id)
        {
            return _userAppService.GetUserDic(id);
        }


        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}


        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
