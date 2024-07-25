using MediatR;
using System.Collections.Concurrent;

namespace Notes.Application.Common.Behaviors
{
    public class OperationsLockingBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        // key - userId; value - object for locking
        static ConcurrentDictionary<int, SemaphoreSlim> _operationsLockers = new ConcurrentDictionary<int, SemaphoreSlim>();

        const string _userIdPropertyName = "UserId";

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!HasUserIdProperty(request))
                return await next();

            int userId = GetUserId(request);

            var operationResult = await LockAndInvokeOperation(next, userId);
            return operationResult;
        }

        bool HasUserIdProperty(TRequest request) =>
            request.GetType().GetProperty(_userIdPropertyName) != null;

        int GetUserId(TRequest request) =>
            (int)request.GetType().GetProperty(_userIdPropertyName)!.GetValue(request)!;

        async Task<TResponse> LockAndInvokeOperation(RequestHandlerDelegate<TResponse> next, int userId)
        {
            var semaphore = _operationsLockers.GetOrAdd(userId, new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();
            try
            {
                return await next();
            }
            finally
            {
                RemoveLock(semaphore, userId);
            }
        }

        void RemoveLock(SemaphoreSlim semaphore, int userId)
        {
            semaphore.Release();
            if ( semaphore.CurrentCount == 1)
            {
                _operationsLockers.TryRemove(userId, out _);
                semaphore.Dispose();
            }
        }
    }
}
