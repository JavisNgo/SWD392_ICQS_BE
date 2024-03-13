using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.Entities;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        //private IUnitOfWork unitOfWork;
        //private readonly IMapper _mapper;

        //public ProductImageController(IUnitOfWork unitOfWork, IMapper mapper)
        //{
        //    this.unitOfWork = unitOfWork;
        //    _mapper = mapper;
        //}

        //[AllowAnonymous]
        //[HttpGet("/ProductImages")]
        //public async Task<IActionResult> getAllProductImages()
        //{
        //    try
        //    {
        //        var productImages = unitOfWork.ProductImageRepository.Get();
        //        return Ok(productImages);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred while get.ErrorMessage:{ex}");
        //    }
        //}
        //[AllowAnonymous]
        //[HttpGet("/ProductImages/{id}")]
        //public IActionResult GetProductImageById(int id)
        //{
        //    try
        //    {
        //        var productImage = unitOfWork.ProductImageRepository.GetByID(id);

        //        if (productImage == null)
        //        {
        //            return NotFound($"ProductImage with ID {id} not found.");
        //        }

        //        return Ok(productImage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while getting the product image. Error message: {ex.Message}");
        //    }
        //}

        //[AllowAnonymous]
        //[HttpPost("/ProductImages")]
        //public IActionResult AddProductImage([FromBody] ProductImagesView ProductImageView)
        //{
        //    try
        //    {
        //        var productImage = _mapper.Map<ProductImages>(ProductImageView);
        //        unitOfWork.ProductImageRepository.Insert(productImage);
        //        unitOfWork.Save();

        //        return Ok(productImage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while adding the product image. Error message: {ex.Message}");
        //    }
        //}

        

        //[AllowAnonymous]
        //[HttpDelete("/ProductImages/{id}")]
        //public IActionResult DeleteProductImage(int id)
        //{
        //    try
        //    {
        //        var productImage = unitOfWork.ProductImageRepository.GetByID(id);

        //        if (productImage == null)
        //        {
        //            return NotFound($"ProductImage with ID {id} not found.");
        //        }

        //        unitOfWork.ProductImageRepository.Delete(id);
        //        unitOfWork.Save();

        //        return Ok("Delete successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"An error occurred while deleting the product image. Error message: {ex.Message}");
        //    }
        //}
    }
}
