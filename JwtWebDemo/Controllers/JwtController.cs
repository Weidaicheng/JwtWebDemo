using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JwtWebDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JwtWebDemo.Controllers
{
    public class JwtController : Controller
    {
        private const string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

        private readonly IJwtEncoder _jwtEncoder;
        private readonly IJwtDecoder _jwtDecoder;

        public JwtController(IJwtEncoder jwtEncoder, IJwtDecoder jwtDecoder)
        {
            _jwtEncoder = jwtEncoder;
            _jwtDecoder = jwtDecoder;
        }

        /// <summary>
		/// 获取时间戳
		/// </summary>
		/// <returns></returns>
		private static long getTimeStamp()
        {
            DateTime startUtc = new DateTime(1970, 1, 1);
            DateTime nowUtc = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);
            return (long)(nowUtc - startUtc).TotalSeconds;
        }

        public IActionResult GetJwt()
        {
            var timeStamp = getTimeStamp();
            var payload = new Payload
            {
                iat = timeStamp,
                exp = timeStamp + 30,
                OpenId = "This is an openId"
            };

            var token = _jwtEncoder.Encode(payload, secret);

            return Content(token);
        }

        [JwtCheck]
        public IActionResult VerifyJwt(string jwt)
        {
            try
            {
                var json = _jwtDecoder.DecodeToObject<Payload>(jwt, secret, true);
                return Json(json);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public IActionResult Fail()
        {
            return Content("Fail");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var attributes = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(JwtCheckAttribute), false);
            if(attributes == null || attributes.Count() == 0)
            {
                base.OnActionExecuted(context);
            }
            else
            {
                var jwt = context.HttpContext.Request.Query["jwt"];
                try
                {
                    var json = _jwtDecoder.DecodeToObject<Payload>(jwt, secret, true);
                    base.OnActionExecuted(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    context.HttpContext.Response.Redirect("/Jwt/Fail");
                }
            }
        }
    }
}