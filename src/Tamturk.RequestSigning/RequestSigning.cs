using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Tamturk {
    public class RequestSigning : IDisposable {
        private readonly HashAlgorithm _provider;
        private readonly bool _doNotDispose;

        public RequestSigning(string key) {
            _provider = new HMACSHA256(key.ToCharArray().Select(a => (byte)a).ToArray());
            _doNotDispose = false;
        }

        public RequestSigning(HashAlgorithm provider) {
            _provider = provider;
            _doNotDispose = true;
        }

        public string SignRequest(string method, string path, Dictionary<string, string> qs, DateTimeOffset? exp, Dictionary<string, string> hiddenQS = null) {
            var _qs = qs.Where(a => !string.IsNullOrEmpty(a.Value)).ToList();

            string to_hash = path + "?" + String.Join("&", _qs
                .Concat(hiddenQS ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .Concat(
                    new[] {
                        new KeyValuePair<string, string>("method", method),
                        new KeyValuePair<string, string>("exp", (DateTimeToUnixTimestamp(exp))?.ToString()),
                    }
                    .Where(a => !string.IsNullOrEmpty(a.Value))
                )
                .OrderBy(a => a.Key)
                .Select(a => $"{a.Key}={a.Value}")
                .ToList());

            return path + "?" + string.Join("&", _qs.Select(a => a.Key + "=" + a.Value)) + (qs.Count > 0 ? "&" : "") + $"method={method}&exp={(DateTimeToUnixTimestamp(exp))}&sig={string.Concat(_provider.ComputeHash(to_hash.ToCharArray().Select(a => (byte)a).ToArray()).Select(b => b.ToString("x2")))}";
        }
        
        public void ValidateRequest(string method, string path, Dictionary<string, string> qs) {
            if (!qs.TryGetValue("sig", out string hash)) {
                throw new InvalidHashException();
            }

            string to_hash = path + "?" +
                             String.Join("&", qs
                                                 .Where(a => a.Key != "sig")
                                                 .OrderBy(a => a.Key)
                                                 .Select(a => $"{a.Key}={a.Value}")
                                                 .ToList());

            if (string.Concat(_provider.ComputeHash(to_hash.ToCharArray().Select(a => (byte)a).ToArray()).Select(b => b.ToString("x2"))) != hash) {
                throw new InvalidHashException();
            }

            if (qs.TryGetValue("exp", out string exp)) {
                if (DateTimeOffset.UtcNow > UnixTimeStampToDateTime(long.Parse(exp))) {
                    throw new ExpiredException();
                }
            }

            if (qs.TryGetValue("method", out string _method) && _method.IndexOf(method, StringComparison.Ordinal) == -1) {
                throw new MethodNotAllowedException();
            }
        }
        
        public bool TryValidateRequest(string method, string path, Dictionary<string, string> qs) {
            if (!qs.TryGetValue("sig", out string hash)) {
                return false;
            }

            string to_hash = path + "?" +
                             String.Join("&", qs
                                              .Where(a => a.Key != "sig")
                                              .OrderBy(a => a.Key)
                                              .Select(a => $"{a.Key}={a.Value}")
                                              .ToList());

            if (string.Concat(_provider.ComputeHash(to_hash.ToCharArray().Select(a => (byte)a).ToArray()).Select(b => b.ToString("x2"))) != hash) {
                return false;
            }

            if (qs.TryGetValue("exp", out string exp)) {
                if (DateTimeOffset.UtcNow > UnixTimeStampToDateTime(long.Parse(exp))) {
                    return false;
                }
            }

            if (qs.TryGetValue("method", out string _method) && _method.IndexOf(method, StringComparison.Ordinal) == -1) {
                return false;
            }

            return true;
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