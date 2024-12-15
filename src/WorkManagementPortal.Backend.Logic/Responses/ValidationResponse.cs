using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class ValidationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        

        public ValidationResponse(bool success, string message, string token = null)
        {
            Success = success;
            Message = message;
            Token = token;
            
        }
    }
}
