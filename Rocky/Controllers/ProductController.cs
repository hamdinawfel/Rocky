using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Utility.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rocky.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepository,
                                 ICategoryRepository categoryRepository,
                                 IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> products = _productRepository.FindAll(null, null, "Category");
            // Eager loading 'Inculde' is used to improve perf
            //foreach(var product in products)
            //{
            //    product.Category = _db.Category.FirstOrDefault(x => x.Id == product.CategoryId);
            //}
            return View(products);
        }

        public IActionResult UpSert(int? id)
        {
            var product = new Product();

            var categories = _categoryRepository.FindAll();

            IEnumerable<SelectListItem> categoryDropDown = categories.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            //ViewBag.CategoryDropDown = categoryDropDown;
            //ViewData["CategoryDropDown"] = categoryDropDown;


            ProductVM productVM = new ProductVM
            {
                Product = product,
                Categories = categoryDropDown
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                product = _productRepository.Find(id.GetValueOrDefault());
                productVM.Product = product;
                if (product == null)
                {
                    return NotFound();
                }
                  return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ProductImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = fileName + extension;

                    _productRepository.Add(productVM.Product);
                }
                else
                {
                    //Updading
                    var objFromDb = _productRepository.FirstOrDefault(u => u.Id == productVM.Product.Id);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ProductImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.ImageUrl);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.ImageUrl = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }

                    _productRepository.Update(productVM.Product);
                }


                _productRepository.SaveChanges();
                return RedirectToAction("Index");
            }

            var categories = _categoryRepository.FindAll();

            productVM = new ProductVM
            {
                Product = new Product(),
                Categories = categories.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var product = _productRepository.FirstOrDefault(null, "Category");

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int? id)
        {
            var productToDelete = _productRepository.Find(id.GetValueOrDefault());

            if (productToDelete == null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string upload = webRootPath + WC.ProductImagePath;
            var productToDeleteImageUrl = Path.Combine(upload, productToDelete.ImageUrl);

            if (System.IO.File.Exists(productToDeleteImageUrl))
            {
                System.IO.File.Delete(productToDeleteImageUrl);
            }

            _productRepository.Remove(productToDelete);
            _productRepository.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
