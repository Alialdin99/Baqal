using Baqal.Application.DTOs;
using Baqal.DataAccess.Interfaces;
using Baqal.Entities.Models;

namespace Baqal.Application.Services
{
    public class StoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreDTO> AddAsync(AddStoreDTO addStoreDTO)
        {
            var store = new Store()
            {
                Name = addStoreDTO.Name,
                Address = addStoreDTO.Address,
                City = addStoreDTO.City,
                Governorate = addStoreDTO.Governorate,
            };

            StoreDTO storeDTO = new StoreDTO()
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                City = store.City,
                Governorate = store.Governorate,
            };
            

            await _unitOfWork.Stores.AddAsync(store);
            await _unitOfWork.Save();
            return storeDTO;
        }
        
        public async Task<Store> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Stores.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            return await _unitOfWork.Stores.GetAllAsync();
        }

        public async Task<StoreDTO> UpdateAsync(Guid id,UpdateStoreDTO updateStoreDTO)
        {
            var oldStore = await _unitOfWork.Stores.GetByIdAsync(id);

            if (oldStore == null)
                return null;

            oldStore.Name = updateStoreDTO.Name;
            oldStore.Address = updateStoreDTO.Address;
            oldStore.City = updateStoreDTO.City;
            oldStore.Governorate = updateStoreDTO.Governorate;
            
            await _unitOfWork.Stores.UpdateAsync(oldStore);
            await _unitOfWork.Save();

            StoreDTO storeDTO = new StoreDTO()
            {
                Id = oldStore.Id,
                Name = oldStore.Name,
                Address = oldStore.Address,
                City = oldStore.City,
                Governorate = oldStore.Governorate,
            };
            return storeDTO;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(id);

            if (store == null)
                return false;

            await _unitOfWork.Stores.DeleteAsync(id);
            await _unitOfWork.Save();
            return true;
        }
    }
}

