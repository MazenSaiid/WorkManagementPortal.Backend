using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkManagementPortal.Backend.Logic.Responses
{
    public class CountValidationResponse : ValidationResponse
    {
        public int Count { get; set; }
        public CountValidationResponse(bool success, string message, string token = null, int count = 0) : base(success, message, token)
        {
            Count = count;
        }
    }
}
