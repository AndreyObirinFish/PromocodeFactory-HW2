using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected IEnumerable<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            var item = Data.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return Task.FromResult(false);
            }

            Data = Data.Where(x => x.Id != id);

            return Task.FromResult(true);
        }

        public Task<T> UpdateAsync(T entity)
        {

            var item = Data.FirstOrDefault(x => x.Id == entity.Id);

            if (item == null)
            {
                return Task.FromResult(item);
            }

            Data = Data.Select(x => x.Id == entity.Id ? entity : x);
            return Task.FromResult(entity);
        }

        public Task<Guid?> AddAsync(T entity)
        {
            if (!Data.Any(x => x.Id == entity.Id))
            {
                var count = Data.Count();
                var DataList = Data.ToList();
                DataList.Add(entity);
                Data = DataList;
                return Task.FromResult<Guid?>(count != Data.Count() ? entity.Id : null);
            }
            return Task.FromResult<Guid?>(null);
        }
    }
}