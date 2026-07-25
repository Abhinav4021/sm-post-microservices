using CQRS.Core.Commands;

namespace CQRS.Core.Infrastructure
{
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches the command to the appropriate handler.
        /// And Func is a delegate that points to the method that will handle the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;

        /// <summary>
        /// Sends the command to the appropriate handler.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task SendAsync(BaseCommand command);
    }
}