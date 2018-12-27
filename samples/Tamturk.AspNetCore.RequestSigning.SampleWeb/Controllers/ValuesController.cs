using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Tamturk.AspNetCore;

namespace Tamturk.AspNetCore.RequestSigning.SampleWeb.Controllers {
    public class ResetPasswordModel {
        public string password { get; set; }
    }
    
    public class ValuesController : ControllerBase {
        private Tamturk.RequestSigning requestSigning;

        public ValuesController(Tamturk.RequestSigning requestSigning) {
            this.requestSigning = requestSigning;
        }
        
        [HttpGet("forgotpw")]
        public dynamic get(string email) {
            if (email == null) {
                throw new Exception("Please enter an e-mail");
            }
            
            int randomNumber = new Random().Next(1000, 9999);

            // sign request for 10 minutes hash it with hidden query string values and make
            // client estimate correct code in 10 minutes

            var expire_date = DateTimeOffset.UtcNow.AddMinutes(10);
            
            var linkWithoutCode = requestSigning.SignRequest(
                "GET,POST",
                "/reset",
                new Dictionary<string, string>() {
                    { "email", email }
                },
                expire_date,
                new Dictionary<string, string>() {
                    { "code", randomNumber.ToString() }
                }
            );
            
            // sign request for 10 minutes, include the code here also, and sent to
            // user e-mail so he can go there and reset
            var linkWithCode = requestSigning.SignRequest(
                "GET,POST",
                "/reset",
                new Dictionary<string, string>() {
                    { "email", email },
                    { "code", randomNumber.ToString() }
                },
                expire_date
            );
            
            // we do not have to store code, or this request, anywhere else!
            
            return new {
                message = "Assume 'has_sent' was sent to e-mail address. And has_returned returned to the client. These links valid only for 10 minutes!",
                has_sent = new {
                    code = randomNumber,
                    link = linkWithCode
                },
                has_returned = linkWithoutCode
            };
        }

        [HttpGet("reset")]
        public bool reset(string email) {
            // is any of the query parameters (incl. code) invalid?
            if (!Request.TryValidateRequest()) {
                return false;
            }
            
            // or you can use this instead, which will throw exception
            // Request.ValidateRequest();

            // this link used before?
            if (Request.IsRevoked()) {
                return false;
            }
            
            // or use Request.ThrowIfRevoked();
            
            return true; // congrats, code is correct and not used, so redirect user to ask new password page.
        }

        // we return void because we return 2xx on success 4xx or 5xx on failure.
        [HttpPost("reset")]
        public async Task<string> reset(
            // we can get email as plain text from query string, because of the hash, the user
            // can not alter the email address that we set it from forgetpw endpoint
            [FromQuery] string email,
            
            [FromBody] ResetPasswordModel model) {
            Request.ValidateRequest(); // if code is incorrect, or link is timeout, this will throw exception
            
            // you may check your password strength here
            // if you check it later, 
            // use will not be able to submit with a new password

            await Request.RevokeAsync(); 
            // this will throw exception if the same link used for password reset twice
            // you may also use Sync version of this (Revoke), 
            // some backends may take advantage of async functions but all of them can be called synchronously also
            
            /*
             * If you omit calling Revoke function (which is optional anyway), the same link
             * may be used to reset password as many times
             * till the expiration date that is set in
             * forgotpw endpoint (10 minutes)
             */
            
            // update user in db and save hashed & salted password to db.
            
            return "YOUR PASSWORD OF " + email + " IS SUCCESSFULLY RESET.";
        }
    }
}