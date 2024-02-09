﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }



        public async Task AddAsync(T item)
        {
            await _dbSet.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var requiredObjectToDelete = await GetAsync(id);

            if (requiredObjectToDelete != null)
            {
                _dbSet.Remove(requiredObjectToDelete);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // Handle the case where the entity with the given id doesn't exist.
                // For now, I'm just logging to the console as an example.
                Console.WriteLine($"Entity with id {id} not found. Delete operation aborted.");
            }
        }




        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }


        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
