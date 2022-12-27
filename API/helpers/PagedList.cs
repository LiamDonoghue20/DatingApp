

using Microsoft.EntityFrameworkCore;

namespace API.helpers
{   
    //<T> is a generic type so this helper works with any types, when its called T is replaced with the actual type
    public class PagedList<T> : List<T>
    {
        public PagedList( IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            //we use ceiling because if we have a page size of 4 and 10 records,
            // we'd want 3 pages instead of 2 to show the extra 2 records
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;

            //returns a list of items
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(
            IQueryable<T> source, 
            int pageNumber,
            int pageSize)
        {
            //gives us the count of our query to the database
            var count = await source.CountAsync();
            //filtering the items based on page number - 1 (so it doesn't skip the first page of items)
            //then multiply it by page size to skip that amount when retrieving the next page (so the same items arent shown twice on a diff page)
            //then return the pageSize after skipping and return as a list
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}