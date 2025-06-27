using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        public IStoreRepository Stores { get; }
        public IProductRepository Products { get; }
        void Dispose();
        Task<int> Save();
    }
}
