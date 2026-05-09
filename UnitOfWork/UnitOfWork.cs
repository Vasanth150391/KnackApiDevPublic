using Knack.API.Data;

namespace Knack.API.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KnackContext _knackContext;

        public UnitOfWork(KnackContext knackContext)
        {
            _knackContext = knackContext;
        }
        public void Commit()
        {
            _knackContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _knackContext.SaveChangesAsync();
        }
    }
}
