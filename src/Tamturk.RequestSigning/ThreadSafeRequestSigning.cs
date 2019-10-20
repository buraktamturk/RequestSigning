using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Tamturk {
    public class ThreadSafeRequestSigning : IRequestSigning {
        private readonly Func<RequestSigning> factory;
        
        public ThreadSafeRequestSigning(byte[] key) {
            factory = () => new RequestSigning(new HMACSHA256(key), true);
        }

        public ThreadSafeRequestSigning(string key) : this(key.ToCharArray().Select(a => (byte)a).ToArray()) {
        }

        public string SignRequest(string method, string path, Dictionary<string, string> qs = null, DateTimeOffset? exp = null,
            Dictionary<string, string> hiddenQs = null) {
            using(var requestSigning = factory())
                return requestSigning.SignRequest(method, path, qs, exp, hiddenQs);
        }

        public void ValidateRequest(string method, string path, Dictionary<string, string> qs) {
            using(var requestSigning = factory())
                requestSigning.ValidateRequest(method, path, qs);
        }

        public bool TryValidateRequest(string method, string path, Dictionary<string, string> qs) {
            using(var requestSigning = factory())
                return requestSigning.TryValidateRequest(method, path, qs);
        }
    }
}