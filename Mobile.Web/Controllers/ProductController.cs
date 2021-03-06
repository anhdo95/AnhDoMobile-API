﻿using Mobile.Common;
using Mobile.Models.DAL.Interfaces;
using Mobile.Models.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using Mobile.Web.Helpers;
using Mobile.Models.Entities;

namespace Mobile.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<JsonResult> Index()
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var products = Enumerable.Empty<SearchProductViewModel>();
            try
            {
                products = await _unitOfWork.ProductRepo.GetAll();
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new
            {
                Products = products
            }, status, statusMessage, products.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 3600, VaryByParam = "keyword")]
        public async Task<JsonResult> Search(string keyword)
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var products = Enumerable.Empty<SearchProductViewModel>();
            try
            {
                products = await _unitOfWork.ProductRepo.SearchByKeyword(keyword, Instances.PRODUCT_SEARCH_NUMBER_USED_TO_DISPLAY);
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new {
                Products = products
            }, status, statusMessage, products.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<JsonResult> GetBestOutstanding()
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var products = Enumerable.Empty<SearchProductViewModel>();
            try
            {
                products = await _unitOfWork.ProductRepo.GetBestOutstanding(Instances.PRODUCT_BEST_OUTSTANDING_NUMBER_USED_TO_DISPLAY);
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new
            {
                Products = products
            }, status, statusMessage, products.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<JsonResult> GetBestSelling()
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var products = Enumerable.Empty<SearchProductViewModel>();
            try
            {
                products = await _unitOfWork.ProductRepo.GetBestSelling(Instances.PRODUCT_BEST_SELLING_NUMBER_USED_TO_DISPLAY);
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new
            {
                Products = products
            }, status, statusMessage, products.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "metaTitle")]
        public async Task<JsonResult> GetDetail(string metaTitle)
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var product = new ProductDetailViewModel();
            var specification = new ProductSpecification();
            try
            {
                product = await _unitOfWork.ProductRepo.GetDetail(metaTitle);
                specification = await _unitOfWork.SpecificationRepo.GetByIdAsync(product.Id);
                specification.Product = null; // Avoid a circular reference to product instance
                
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }
            var results = APIHelper.Instance.GetApiResult(new {
                Product = product,
                Specification = specification
            }, status, statusMessage, 2);
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "id")]
        public async Task<JsonResult> GetRelated(int id)
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var products = Enumerable.Empty<RelatedProductViewModel>();
            try
            {
                products = await _unitOfWork.ProductRepo.GetRelatedProducts(id, Instances.PRODUCT_RELATED_PRODUCT_NUMBER_USED_TO_DISPLAY);
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new
            {
                Products = products
            }, status, statusMessage, products.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 86400, VaryByParam = "None")]
        public async Task<JsonResult> GetCategories()
        {
            string status = Instances.ERROR_STATUS;
            string statusMessage = string.Empty;
            var categories = Enumerable.Empty<CategoryViewModel>();
            try
            {
                categories = await _unitOfWork.CategoryRepo.Select(
                    c => new CategoryViewModel {
                        Id = c.Id,
                        Name = c.Name,
                        MetaTitle = c.MetaTitle
                    }, filter: c => c.Status,
                    orderBy: list => list.OrderBy(c => c.DisplayOrder));
                status = Instances.SUCCESS_STATUS;
            }
            catch (Exception ex)
            {
                statusMessage = ex.Message;
            }

            var results = APIHelper.Instance.GetApiResult(new
            {
                Categories = categories
            }, status, statusMessage, categories.Count());
            return Json(results, JsonRequestBehavior.AllowGet);
        }
    }
}