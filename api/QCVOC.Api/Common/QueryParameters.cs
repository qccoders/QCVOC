namespace QCVOC.Api.Common
{
    public class QueryParameters
    {
        public int Offset { get; set; }
        public int Limit { get; set; }

        public SortOrder OrderBy { get; set; }

        public QueryParameters()
        {
            Offset = 0;
            Limit = 100;
            OrderBy = SortOrder.ASC;
        }
    }
}
