using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;

namespace Memberships.Extensions
{
    public static class HttpContextExtensions
    {
        private const string nameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public static string GetUserId(this HttpContextBase ctx)
        {
            var uid = string.Empty;
            try
            {
                var claims = ctx.GetOwinContext().Get<ApplicationSignInManager>().AuthenticationManager.User.Claims.FirstOrDefault(claim => claim.Type.Equals(nameIdentifier));
                if(claims != default(Claim))
                {
                    uid = claims.Value;
                }
            }
            catch (Exception e){}

            return uid;
        }
    }
}