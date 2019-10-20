using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Tamturk {
    public class RequestSigning : IRequestSigning, IDisposable {
        private readonly HashAlgorithm _provider;
        private readonly bool _doNotDispose;
        
        public RequestSigning(byte[] key) {
            _provider = new HMACSHA256(key);
            _doNotDispose = false;
        }

        public RequestSigning(string key) : this(key.ToCharArray().Select(a => (byte)a).ToArray()) {
        }

        public RequestSigning(HashAlgorithm provider, bool dispose = false) {
            _provider = provider;
            _doNotDispose = !dispose;
        }

        public string SignRequest(string method, string path,
            Dictionary<string, string> qs = null,
            DateTimeOffset? exp = null,
            Dictionary<string, string> hiddenQs = null) {
            var _qs = qs?.Where(a => !string.IsNullOrEmpty(a.Value)).ToList();

            var parameters = (_qs ?? Enumerable.Empty<KeyValuePair<string, string>>())
                             .Concat(
                                 new[] {
                                         new KeyValuePair<string, string>("method", method),
                                         new KeyValuePair<string, string>("exp",
                                             exp == null ? null : (DateTimeToUnixTimestamp(exp))?.ToString()),
                                     }
                                     .Where(a => !string.IsNullOrEmpty(a.Value))
                             )
                             .OrderBy(a => a.Key)
                             .ToList();
            
            string to_hash = path + "?" + string.Join("&", parameters
                                                           .Concat(hiddenQs?.Where(a => !string.IsNullOrEmpty(a.Value)) ??
                                                                   Enumerable.Empty<KeyValuePair<string, string>>())
                                                           .OrderBy(a => a.Key)
                                                           .Select(a => $"{a.Key}={a.Value}")
                                                            .ToList());

            return path + "?" + string.Join("&",
                       parameters
                           .Concat(new[] {
                               new KeyValuePair<string, string>("sig",
                                   string.Concat(_provider
                                                 .ComputeHash(to_hash.ToCharArray().Select(a => (byte) a).ToArray())
                                                 .Select(b => b.ToString("x2")))),
                           })
                           .Select(a => $"{Uri.EscapeDataString(a.Key)}={Uri.EscapeDataString(a.Value)}")
                           .ToList());
        }
        
        public void ValidateRequest(string method, string path, Dictionary<string, string> qs) {
            if (!qs.TryGetValue("sig", out string hash)) {
                throw new InvalidHashException();
            }

            var to_hash = path + "?" +
                             string.Join("&", qs
                                                 .Where(a => a.Key != "sig" && a.Value.Length > 0)
                                                 .OrderBy(a => a.Key)
                                                 .Select(a => $"{a.Key}={a.Value}")
                                                 .ToList());

            if (string.Concat(_provider.ComputeHash(to_hash.ToCharArray().Select(a => (byte)a).ToArray()).Select(b => b.ToString("x2"))) != hash) {
                throw new InvalidHashException();
            }

            if (qs.TryGetValue("exp", out var exp)) {
                if (DateTimeOffset.UtcNow > UnixTimeStampToDateTime(long.Parse(exp))) {
                    throw new ExpiredException();
                }
            }

            if (qs.TryGetValue("method", out var _method) && _method.IndexOf(method, StringComparison.Ordinal) == -1) {
                throw new MethodNotAllowedException();
            }
        }
        
        public bool TryValidateRequest(string method, string path, Dictionary<string, string> qs) {
            try {
                ValidateRequest(method, path, qs);
                return true;
            }
            catch (Exception e) when (e is InvalidHashException || e is ExpiredException || e is MethodNotAllowedException)
            {
                return false;
            }
        }

        public void Dispose() {
            if(!_doNotDispose)
                _provider.Dispose();
        }

        public static DateTimeOffset? UnixTimeStampToDateTime(long? unixTimeStamp) {
            if (unixTimeStamp == null)
                return null;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp.Value).ToUniversalTime();
            return dtDateTime;
        }

        public static long? DateTimeToUnixTimestamp(DateTimeOffset? dateTime) {
            return dateTime == null ? (long?)null : (long)(dateTime.Value -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}