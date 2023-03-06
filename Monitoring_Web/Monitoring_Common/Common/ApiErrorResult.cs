namespace Monitoring_Common.Common
{
    public class ApiErrorResult<T> : ApiResult<T>
    {
        public ApiErrorResult()
        {
        }

        public ApiErrorResult(string message)
        {
            IsSuccessed = false;
            Message = message;
        }
    }
}
