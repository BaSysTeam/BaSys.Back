using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Common.Infrastructure
{
    /// <summary>
    /// Encapsulates the response sent through the API, providing a standardized structure for returning data, status, and messages. 
    /// This class is designed to wrap any type of data (T) along with a custom status code and message, facilitating clear communication between the server and clients. 
    /// The 'Status' property is intended for application-specific status codes and is separate from HTTP status codes, 
    /// allowing for more granular control over response status reporting. 
    /// This makes it easier to handle success and error states within the application while providing additional context through messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ResultWrapper<T>
    {
        private const string SuccessMessage = "OK";
        private const string UnknownErrorMessage = "Unknown error";

        public int Status { get; set; } = -1;
        public string Message { get; set; } = UnknownErrorMessage;
        public T Data { get; set; } = default(T);
        public bool IsOK => Status == 0;

        public ResultWrapper()
        {
            
        }

        public ResultWrapper(T data, int status = -1, string message = null)
        {
            Status = status;
            Data = data;
            Message = message ?? UnknownErrorMessage;
        }

        public void Success(T data)
        {
            Data = data;
            Message = SuccessMessage;
            Status = 0;
        }
        public void Error(int status, string message)
        {
            Data = default(T);
            Status = status;
            Message = message;
        }

        public void SetStatus(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public override string ToString()
        {
            return $"{Status}:{Message}";
        }
 
    }
}
