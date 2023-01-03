using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                //Creating a Client
                var client = httpClient.CreateClient("MagicAPI");
                //Creating a Message and on this message we have configure a few things
                HttpRequestMessage message = new HttpRequestMessage();
                //Header type "Accept"
                message.Headers.Add("Accept", "application/json");
                //Url Where we have to call the API
                message.RequestUri = new Uri(apiRequest.url);
                //Data will not be null in POST or PUT calls
                //that is why we are sending the serialized data 
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                //What is HTTP type among the list[Get,Put,Post,Delete]
                //That is why we have used Switch case 
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                //We will set the response to null 
                HttpResponseMessage apiResponse = null;
                //Here the ACtual Response will be stored 
                apiResponse = await client.SendAsync(message);
                //We neeed to Extract API content from apiResponse
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                //This ApiContent will be Deserialized 
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponse;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    Errors = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
