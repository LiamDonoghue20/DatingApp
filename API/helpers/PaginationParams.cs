
namespace API.helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            //if the user sets a value above the max (50) then set the PageSize to max 
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
    }
}