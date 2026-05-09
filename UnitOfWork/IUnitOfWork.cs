namespace Knack.API.UnitOfWork
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commit all the changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Commit all the changes.
        /// </summary>
        Task CommitAsync();

    }
}
