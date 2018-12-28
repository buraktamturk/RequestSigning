using System;

namespace Tamturk {
    public class SigningException : Exception {
        public SigningException(string message) : base(message) {
            
        }
    }

    public class InvalidHashException : SigningException {
        public InvalidHashException() : base("Hash is empty or not valid!") {
            
        }
    }
    
    public class ExpiredException : SigningException {
        public ExpiredException() : base("Link is expired!") {
            
        }
    }
    
    public class MethodNotAllowedException : SigningException {
        public MethodNotAllowedException() : base("This method is not allowed!") {
            
        }
    }
    
    public class HashRevokedException : SigningException {
        public HashRevokedException() : base("You may only do this action only once.") {
            
        }
    }
}