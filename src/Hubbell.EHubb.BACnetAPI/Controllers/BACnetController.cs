using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hubbell.EHubb.BACnetAPI.Models;
using Hubbell.EHubb.Common.AppConstant;
using Hubbell.EHubb.Infrastructure.Messaging.Helper.PostgreHandler;
using Hubbell.EHubb.Infrastructure.Messaging.PostgreProducer;
using Newtonsoft.Json;

namespace Hubbell.EHubb.BACnetAPI.Controllers
{
    [ApiController]
    [Route(RouteTemplate.BACnetApiController)]
    public class BACnetController : ControllerBase
    {
        private readonly IPostgreProducer _postgreProducer;
        public BACnetController(IPostgreProducer postgreProducer)
        {
            _postgreProducer = postgreProducer;
        }

        [HttpPost("GetSystemValue")]
        public async Task<BACnetSystemConfigResponseModel> GetSystemValue()
        {
           
            //var context =
            //    await _postgreProducer.Send(new object[]{ JsonConvert.SerializeObject(config) }, PostgresBrokerTopic.GetSystemInformation, SubscribeTo.SystemApi);
            var context =
                await _postgreProducer.Send(new object[] { null }, PostgresBrokerTopic.GetSystemInformation, SubscribeTo.SystemApi);
            if (context.Exceptions.Any())
            {
                //TODO : Error handling
            }

            var responseModel = JsonConvert.DeserializeObject<BACnetSystemConfigResponseModel>(context.Response);

            return responseModel;
        }
    }

 
}
