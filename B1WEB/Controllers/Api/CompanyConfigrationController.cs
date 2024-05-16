using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B1WEB.AppModels;
using B1WEB.DBContext;
using B1WEB.Common;
using B1WEB.Models;
using System.Reflection;

namespace B1WEB.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyConfigrationController : ControllerBase
    {
        private readonly MyAppContext _context;

        public CompanyConfigrationController(MyAppContext context)
        {
            _context = context;
        }

        // GET: api/PortalUsers
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            try
            {

                var result = _context.CompanyConfiguration.Where(x => x.IsActive == true)
                                           .ToList();

                return Ok(new ApiResponse<object>
                {
                    Code = ResponseCode.Success,
                    Message = "Success",
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get(int id)
        {
            try
            {

                var result = _context.CompanyConfiguration.Where(x => x.IsActive == true && x.ID == id)
                                           .FirstOrDefault();

                return Ok(new ApiResponse<object>
                {
                    Code = ResponseCode.Success,
                    Message = "Success",
                    Data = result
                });
            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

        [HttpPost]
        [Route("AddCompanyConfigration")]
        public IActionResult AddUser(CompanyConfiguration model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.BadRequest,
                        Message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }
                if (model.DefaultCompany == true)
                {
                    // Set DefaultCompany to false for all existing records
                    var defaultCompanies = _context.CompanyConfiguration.Where(c => c.DefaultCompany).ToList();
                    foreach (var company in defaultCompanies)
                    {
                        company.DefaultCompany = false;
                        _context.CompanyConfiguration.Update(company);
                    }
                    _context.SaveChanges();
                }

                model.CreatedOn = DateTime.Now;
                model.UpdatedOn = DateTime.Now;
                model.IsActive = model.IsActive;
                model.CreatedByUserID = 1;
                model.UpdatedByUserID = 1;
                model.IsActive = true;

                _context.CompanyConfiguration.Add(model);
                _context.SaveChanges();

                return Ok(new ApiResponse<object>
                {
                    Code = ResponseCode.Success,
                    Message = "Company Configration added Successfully",
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

        [HttpPost]
        [Route("UpdateCompanyConfigration")]
        public IActionResult UpdateCompanyConfigration([FromBody] CompanyConfiguration model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.BadRequest,
                        Message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    });
                }
                if (model.DefaultCompany == true)
                {
                    // Set DefaultCompany to false for all existing records
                    var defaultCompanies = _context.CompanyConfiguration.Where(c => c.DefaultCompany).ToList();
                    foreach (var company in defaultCompanies)
                    {
                        company.DefaultCompany = false;
                        _context.CompanyConfiguration.Update(company);
                    }
                    _context.SaveChanges();
                }


                var data = _context.CompanyConfiguration.Find(model.ID);

                if (data != null)
                {

                    data.UpdatedOn = DateTime.Now;
                    data.UpdatedByUserID = 1;
                    data.DatabaseName = model.DatabaseName;
                    data.CompanyLogo = model.CompanyLogo;
                    data.ServiceLayerURL = model.ServiceLayerURL;
                    data.ServiceLayerUsername = model.ServiceLayerUsername;
                    data.ServiceLayerPassword = model.ServiceLayerPassword;
                    data.DefaultCompany = model.DefaultCompany;
                   

                    _context.Update(data);
                    _context.SaveChanges();

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Success,
                        Message = "Company Configuration Updated Successfully",
                        Data = model
                    });
                }
                else
                {
                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.NotFound,
                        Message = "Not Found",
                        Data = null
                    });
                }

            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

        [HttpDelete]
        [Route("DeleteCompanyConfigration")]
        public IActionResult DeleteCompanyConfigration(int id)
        {
            try
            {
                var data = _context.CompanyConfiguration.Find(id);
                data.UpdatedOn = DateTime.Now;
                data.IsActive = false;


                _context.SaveChanges();

                return Ok(new ApiResponse<object>
                {
                    Code = ResponseCode.Success,
                    Message = "Success",
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

    }
}
