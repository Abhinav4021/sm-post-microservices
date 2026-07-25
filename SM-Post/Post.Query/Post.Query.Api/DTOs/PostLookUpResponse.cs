using Post.Common.DTOs;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.DTOs
{
    public class PostLookUpResponse : BaseResponse
    {
        public List<PostEntity> Posts { get; set; }
    }
}