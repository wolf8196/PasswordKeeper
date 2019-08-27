using System.Threading.Tasks;

namespace PasswordKeeper.Security.Persistence
{
    internal abstract class TransactionManagerBase
    {
        private bool isTransactionStarted;

        internal async Task BeginTransactionAsync()
        {
            if (isTransactionStarted)
            {
                return;
            }

            await BeginAsync().ConfigureAwait(false);

            isTransactionStarted = true;
        }

        internal async Task CommitTransactionAsync()
        {
            if (!isTransactionStarted)
            {
                return;
            }

            await CommitAsync().ConfigureAwait(false);

            isTransactionStarted = false;
        }

        internal async Task RollbackTransactionAsync()
        {
            if (!isTransactionStarted)
            {
                return;
            }

            await RollbackAsync().ConfigureAwait(false);

            isTransactionStarted = false;
        }

        protected virtual Task BeginAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task CommitAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task RollbackAsync()
        {
            return Task.CompletedTask;
        }
    }
}