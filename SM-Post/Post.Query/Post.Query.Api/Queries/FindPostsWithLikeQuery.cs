using CQRS.Core.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsWithLikeQuery : BaseQuery
    {
        public int NumberOfLikes { get; set; }
    }
}