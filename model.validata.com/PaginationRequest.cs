
namespace model.validata.com
{
    public class PaginationRequest
    {
        public readonly int pageNumber;
        public readonly int pageSize;
        public readonly int offset;
        public PaginationRequest(int pageNumber, int pageSize)
        {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            this.offset = (pageNumber - 1) * pageSize;
        }


        
    }
}
