using System;
using System.Collections.Generic;

namespace Tamturk {
    public interface IRequestSigning {
        string SignRequest(string method, string path,
            Dictionary<string, string> qs = null, DateTimeOffset? exp = null,
            Dictionary<string, string> hiddenQs = null);

        void ValidateRequest(string method, string path, Dictionary<string, string> qs);

        bool TryValidateRequest(string method, string path, Dictionary<string, string> qs);
    }
}