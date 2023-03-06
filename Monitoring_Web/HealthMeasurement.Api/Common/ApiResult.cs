namespace HealthMeasurement.Api.Common
{
    public class ApiResult<T>
    {
        public bool IsSuccessed { get; set; }

        public string Message { get; set; }

        //public T Token { get; set; }
    }
}
