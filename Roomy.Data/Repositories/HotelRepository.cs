using Microsoft.EntityFrameworkCore;
using Roomy.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roomy.Data.Repositories
{
    public class HotelRepository(IDbContextFactory<AppDbContext> contextFactory) : IHotelRepository
    {
        /// <summary>
        /// Checks if hotel exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var context = contextFactory.CreateDbContext();
            return await context.Hotels.AnyAsync(h => h.Id == id, cancellationToken);
        }
    }
}
