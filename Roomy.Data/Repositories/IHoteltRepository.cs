namespace Roomy.Data.Repositories
{
    public interface IHotelRepository
    {
        /// <summary>
        /// Checks if holes exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}