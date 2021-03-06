﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.Models.DAL.Interfaces;
using Mobile.Models.Entities;
using System.Linq;
using Mobile.Models.ViewModels;
using System.Xml.Linq;
using System.Data.Entity;

namespace Mobile.Models.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(MobileDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SearchProductViewModel>> SearchByKeyword(string keyword, int? topNumer = null)
        {
            return await Select(
                p => new SearchProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    MetaTitle = p.MetaTitle,
                    Image = p.Image,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                },
                filter: p => p.Status && p.Name.Contains(keyword) || p.MetaTitle.Contains(keyword),
                orderBy: list => list.OrderByDescending(p => p.ViewCount),
                topNumber: topNumer ?? int.MaxValue);
        }

        public async Task<IEnumerable<ProductViewModel>> GetBestOutstanding(int topNumer)
        {
            return await Select(
                p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    MetaTitle = p.MetaTitle,
                    Image = p.Image,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                    DiscountAccompanying = p.DiscountAccompanying,
                    LargeImage = p.LargeImage
                },
                filter: p => p.Status,
                orderBy: list => list.OrderByDescending(p => p.TopHot),
                topNumber: topNumer);
        }

        public async Task<IEnumerable<ProductViewModel>> GetBestSelling(int topNumer)
        {
            return await Select(
                p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    MetaTitle = p.MetaTitle,
                    Image = p.Image,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                    DiscountAccompanying = p.DiscountAccompanying,
                    LargeImage = p.LargeImage
                },
                includeProperties: "OrderDetails",
                filter: p => p.Status,
                orderBy: list => list.OrderByDescending(p => p.OrderDetails.Sum(o => o.Order.Total)),
                topNumber: topNumer);
        }

        public async Task<IEnumerable<ProductViewModel>> GetAll()
        {
            return await Select(
                p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    MetaTitle = p.MetaTitle,
                    Image = p.Image,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                    DiscountAccompanying = p.DiscountAccompanying,
                    CategoryName = p.Category.Name
                });
        }

        public async Task<ProductDetailViewModel> GetDetail(string metaTitle)
        {
            var product = await _dbSet.SingleOrDefaultAsync(p => p.MetaTitle == metaTitle);

            return new ProductDetailViewModel
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                MetaTitle = product.MetaTitle,
                CategoryName = product.Category.Name,
                Image = product.Image,
                MoreImages = GetMoreImages(product.MoreImages),
                Price = product.Price,
                PromotionPrice = product.PromotionPrice,
                IncludeVAT = product.IncludeVAT,
                Quantity = product.Quantity,
                Detail = product.Detail,
                Description = product.Description,
                Status = product.Status,
            };
        }

        public async Task<IEnumerable<RelatedProductViewModel>> GetRelatedProducts(int id, int topNumber)
        {
            int categoryId = await (from p in _dbSet
                                    where p.Id == id
                                    select p.CategoryId
                                    ).SingleOrDefaultAsync();

            if (categoryId == 0)
                throw new System.ArgumentNullException("id", "The product isn't exists.");

            return await Select(
                p => new RelatedProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    MetaTitle = p.MetaTitle,
                    Image = p.Image,
                    Price = p.Price,
                    PromotionPrice = p.PromotionPrice,
                    Screen = p.Specification.Screen,
                    CameraAfter = p.Specification.CameraAfter,
                    CameraBefore = p.Specification.CameraBefore,
                    PinCapacity = p.Specification.PinCapacity
                },
                filter: p => p.Status && p.Id != id && p.CategoryId == categoryId,
                orderBy: list => list.OrderByDescending(p => p.Price),
                topNumber: topNumber);
        }

        private IEnumerable<string> GetMoreImages(string moreImages)
        {
            XElement xImages = XElement.Parse(moreImages);

            foreach (XElement element in xImages.Elements())
            {
                yield return element.Value;
            }
        }
    }
}
