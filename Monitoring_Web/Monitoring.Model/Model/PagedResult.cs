namespace Monitoring.Model.Model
{
    public class PagedResult<T> : PagedResultBase
    {
        public List<T> listItem { set; get; }
    }
}
