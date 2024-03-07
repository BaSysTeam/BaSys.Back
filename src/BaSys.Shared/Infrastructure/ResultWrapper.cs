using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

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

        /// <summary>
        ///The status code.Note that this is NOT an HTTP status code, but an application-specific status code.A status of 0 indicates no errors.
        /// </summary>
        public int Status { get; set; } = -1;
        /// <summary>
        /// The message intended for user display.
        /// </summary>
        public string Message { get; set; } = UnknownErrorMessage;
        /// <summary>
        /// Additional technical information. This should not be displayed to the user but printed to the console for debugging purposes.
        /// </summary>
        public string Info { get; set; } = string.Empty;
        /// <summary>
        /// The result data.
        /// </summary>
        public T Data { get; set; } = default(T);
        /// <summary>
        /// Indicates the absence of errors.
        /// </summary>
        public bool IsOK => Status == 0;
        /// <summary>
        /// Combines all information into a single string.
        /// </summary>
        public string Presentation
        {
            get
            {
                var presentation = $"{Status}";

                if (!string.IsNullOrEmpty(Message))
                    presentation += " : " + Message;

                if (!string.IsNullOrWhiteSpace(Info))
                    presentation += " : " + Info;

                return presentation;
            }
        }

        public ResultWrapper()
        {

        }

        public ResultWrapper(T data, int status = -1, string message = null, string info = null)
        {
            Status = status;
            Data = data;
            Message = message ?? UnknownErrorMessage;
            Info = info ?? string.Empty;
        }

        public void Success(T data)
        {
            Data = data;
            Message = SuccessMessage;
            Status = 0;
        }
        public void Error(int status, string message, string info = null)
        {
            Data = default(T);
            Status = status;
            Message = message;
            Info = info ?? string.Empty;
        }

        public void SetStatus(int status, string message, string info = null)
        {
            Status = status;
            Message = message;
            Info = info ?? string.Empty;
        }

        public override string ToString()
        {
            return Presentation;
        }

    }
}
