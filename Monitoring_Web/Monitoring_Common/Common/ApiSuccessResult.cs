namespace Monitoring_Common.Common
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T resultObj)
        {
            IsSuccessed = true;
        }

        public ApiSuccessResult()
        {
            IsSuccessed = true;
        }
    }
}
