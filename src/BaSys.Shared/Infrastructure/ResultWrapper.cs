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

        public int Status { get; set; }
        public string Message { get; set; } = SuccessMessage;
        public T Data { get; set; } = default(T);
        public bool IsOK => Status == 0;

        public ResultWrapper(T data, int status = 0, string message = null)
        {
            Status = status;
            Data = data;
            Message = message ?? SuccessMessage;
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

        public static ResultWrapper<T> Success(T data){

            return new ResultWrapper<T>(data);

        }

        public static ResultWrapper<T> Error(int status, string message)
        {
            return new ResultWrapper<T>(default(T), status, message);
        }
    }
}
