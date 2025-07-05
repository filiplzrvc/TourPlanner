using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourPlanner.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TourPlannerContext dbContext;
        private readonly DbSet<T> dbSet;

        public Repository(TourPlannerContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet
                .Include("TourLogs")
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
