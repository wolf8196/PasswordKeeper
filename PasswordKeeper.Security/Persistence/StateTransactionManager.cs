using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security.Persistence
{
    internal sealed class StateTransactionManager : TransactionManagerBase
    {
        private readonly IOriginator originator;
        private IMemento state;

        internal StateTransactionManager(IOriginator originator)
        {
            this.originator = originator.ThrowIfNull(nameof(originator));
        }

        protected override Task BeginAsync()
        {
            state = originator.GetMemento();
            return base.BeginAsync();
        }

        protected override Task RollbackAsync()
        {
            state.Restore();
            return base.RollbackAsync();
        }
    }
}