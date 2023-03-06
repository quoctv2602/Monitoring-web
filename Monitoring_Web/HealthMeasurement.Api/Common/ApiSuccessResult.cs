namespace HealthMeasurement.Api.Common
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T resultObj)
        {
            IsSuccessed = true;
            //Token = resultObj;
        }

        public ApiSuccessResult()
        {
            IsSuccessed = true;
        }
    }
}
