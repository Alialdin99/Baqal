using Baqal.Application.DTOs;
using Baqal.DataAccess.Interfaces;
using Baqal.Entities.Models;
using Microsoft.VisualBasic;

namespace Baqal.Application.Services
{
    public class StoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddStore(AddStoreDTO addStoreDTO)
        {
            var store = new Store()
            {
                Name = addStoreDTO.Name,
                Address = addStoreDTO.Address,
                City = addStoreDTO.City,
                Governorate = addStoreDTO.Governorate,
            };
            
            await _unitOfWork.Stores.AddAsync(store);
            await _unitOfWork.Save();
            return true;
        }   

    }
}

