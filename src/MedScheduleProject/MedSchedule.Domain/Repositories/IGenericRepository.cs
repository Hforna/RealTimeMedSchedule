using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Domain.Repositories
{
    public interface IGenericRepository
    {
        public void UpdateRange<T>(List<T> entities) where T : Entity;
        public Task Add<T>(T entity) where T : Entity;
        public void Update<T>(T entity) where T : Entity;
    }
}
