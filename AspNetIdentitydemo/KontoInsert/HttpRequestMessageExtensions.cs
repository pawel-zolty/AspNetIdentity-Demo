using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.KontoInsert
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        ///     With basic konto insert authentication.
        /// </summary>
        /// <param name="message">Request message.</param>
        /// <param name="clientId">Client id.</param>
        /// <param name="clientSecret">Client secret.</param>
        /// <returns>Request message.</returns>
        public static HttpRequestMessage WithBasicKiAuthentication(this HttpRequestMessage message, string clientId, string clientSecret)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(clientId, ":", clientSecret)));

            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            return message;
        }

        public static HttpRequestMessage WithUserAgent(this HttpRequestMessage message, string userAgent)
        {
            message.Headers.UserAgent.Add(ProductInfoHeaderValue.Parse(userAgent));
            return message;
        }

        public static HttpRequestMessage WithFormUrlEncodedContent(this HttpRequestMessage message, Dictionary<string, string> parameters)
        {
            message.Content = new FormUrlEncodedContent(parameters);

            return message;
        }

        public static void HandleErrors(this HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
                return;

            switch (responseMessage.StatusCode)
            {
                //case HttpStatusCode.BadRequest: throw new BadRequestException();
                //case HttpStatusCode.Unauthorized: throw new UnauthorizedException();
                //case HttpStatusCode.NotFound: throw new NotFoundException();
                //default: throw await responseMessage.HandleUnexpectedResultException();
                case HttpStatusCode.BadRequest: throw new Exception();
                case HttpStatusCode.Unauthorized: throw new Exception();
                case HttpStatusCode.NotFound: throw new Exception();
                default: throw new Exception();
            }
        }
    }
}