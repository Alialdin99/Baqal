using Baqal.Application.DTOs;
using Baqal.DataAccess.Interfaces;
using Baqal.Entities.Enums;
using Baqal.Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDTO> AddAsync(AddProductDTO addProductDTO)
        {
            var product = new Product()
            {
                Name = addProductDTO.Name,
                Price = addProductDTO.Price,
                Unit = addProductDTO.Unit,
                Description = addProductDTO.Description,
                ImageUrl = addProductDTO.ImageUrl,
                StoreId = addProductDTO.StoreId,
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Save();

            ProductDTO productDTO = new ProductDTO()
            {
                Id = product.Id,
                Name = addProductDTO.Name,
                Price = addProductDTO.Price,
                Unit = addProductDTO.Unit,
                Description = addProductDTO.Description,
                ImageUrl = addProductDTO.ImageUrl,
                StoreId = addProductDTO.StoreId,
            };
            return productDTO;
        }

        public async Task<ProductDTO> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Unit = product.Unit,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                StoreId = product.StoreId
            };
        }


        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<ProductDTO> UpdateAsync(Guid id, UpdateProductDTO updateProductDTO)
        {
            var oldProduct = await _unitOfWork.Products.GetByIdAsync(id);

            if (oldProduct == null)
                return null;

            oldProduct.Name = updateProductDTO.Name;
            oldProduct.Price = updateProductDTO.Price;
            oldProduct.Unit = updateProductDTO.Unit;
            oldProduct.Description = updateProductDTO.Description;
            oldProduct.ImageUrl = updateProductDTO.ImageUrl;

            await _unitOfWork.Products.UpdateAsync(oldProduct);
            await _unitOfWork.Save();

            ProductDTO productDTO= new ProductDTO()
            {
                Id = oldProduct.Id,
                Name = oldProduct.Name,
                Price = oldProduct.Price,
                Unit = oldProduct.Unit,
                Description = oldProduct.Description,
                ImageUrl = oldProduct.ImageUrl,
                StoreId = oldProduct.StoreId,
                Store = oldProduct.Store,
            };
            return productDTO;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            if (product == null)
                return false;

            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.Save();
            return true;
        }
    }
}
