using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class CommentAddedEvent : BaseEvent
    {
        public CommentAddedEvent(): base(nameof(CommentAddedEvent))
        {
        }

        public Guid CommentId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
    }
}