using Ardalis.Specification.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GymDogs.Infrastructure.Persistence;
using GymDogs.Application.Interfaces;
using GymDogs.Domain;

namespace GymDogs.Infrastructure.Persistence
{
    public class EfRepository<T> : RepositoryBase<T>, IRepository<T>, IReadRepository<T>
        where T : BaseEntity
    {
        public EfRepository(AppDbContext db) : base(db) { }
    }
}
