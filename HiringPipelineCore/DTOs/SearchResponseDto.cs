namespace HiringPipelineCore.DTOs
{
    public class SearchResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool HasMore => Skip + Items.Count() < TotalCount;
    }
}
