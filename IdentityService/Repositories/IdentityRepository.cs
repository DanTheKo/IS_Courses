﻿using IdentityService.Models;
using IdentityService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace IdentityService.Repositories
{

    public class IdentityRepository
    {
        private readonly IdentityDbContext _context;

        public IdentityRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public virtual async Task<Identity?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public virtual async Task<Identity?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Login == username);
        }

        public virtual async Task<Identity?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public virtual async Task AddAsync(Identity user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(Identity user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}