using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LyricDownloadWebApp.Controllers
{
    public class LyricsController : ApiController
    {
        [HttpGet]
        [Route("Lyrics")]
        public IHttpActionResult Get()
        {
            var authorizeUser = AuthorizeUser(Request.Headers);

            if (authorizeUser.Result == false)
                return Unauthorized();

            var contentTask = GetContent();
            return Ok(contentTask.Result);
        }
        private static async Task<bool> AuthorizeUser(HttpHeaders headers)
        {
            HttpContext.Current.User = await GetClaimsForRoleAsync(headers)
                .ConfigureAwait(continueOnCapturedContext: false);

            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        protected async Task<string> GetContent()
        {
            var uri = new Uri("http://genius.com/Drake-hotline-bling-lyrics");

            using (var client = new HttpClient())
            {
                var getResult = await client.GetAsync(uri)
                    .ConfigureAwait(continueOnCapturedContext: false);
                var content = await getResult.Content.ReadAsStringAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
                return content;
            }
        }

        private static async Task<ClaimsPrincipal> GetClaimsForRoleAsync(HttpHeaders role)
        {
            return await Task.Run(() => new ClaimsPrincipal(new List<ClaimsIdentity>
            {
                new ClaimsIdentity("SecureAuth", "SafeType", role.GetValues("RoleType").FirstOrDefault())
            }));
        }


    }
}
