using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security.Persistence
{
    internal sealed class CompositeTransactionManager : TransactionManagerBase, IDisposable
    {
        private readonly IEnumerable<TransactionManagerBase> transactionManagers;

        internal CompositeTransactionManager(IEnumerable<TransactionManagerBase> transactionManagers)
        {
            this.transactionManagers = transactionManagers.ThrowIfNull(nameof(transactionManagers));
        }

        internal event Action OnBegan;

        internal event Action OnCommmited;

        internal event Action OnRollbacked;

        public void Dispose()
        {
            foreach (var item in transactionManagers)
            {
                (item as IDisposable)?.Dispose();
            }
        }

        protected override async Task BeginAsync()
        {
            foreach (var item in transactionManagers)
            {
                await item.BeginTransactionAsync().ConfigureAwait(false);
            }

            await base.BeginAsync().ConfigureAwait(false);
            OnBegan?.Invoke();
        }

        protected override async Task CommitAsync()
        {
            foreach (var item in transactionManagers)
            {
                await item.CommitTransactionAsync().ConfigureAwait(false);
            }

            await base.CommitAsync().ConfigureAwait(false);
            OnCommmited?.Invoke();
        }

        protected override async Task RollbackAsync()
        {
            foreach (var item in transactionManagers)
            {
                await item.RollbackTransactionAsync().ConfigureAwait(false);
            }

            await base.RollbackAsync().ConfigureAwait(false);
            OnRollbacked?.Invoke();
        }
    }
}