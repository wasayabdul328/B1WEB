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
    public class PortalUsersController : ControllerBase
    {
        private readonly MyAppContext _context;

        public PortalUsersController(MyAppContext context)
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

                var result = _context.PortalUsers.Where(x => x.IsActive==true)
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

                var result = _context.PortalUsers.Where(x => x.IsActive == true && x.ID==id)
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
        [Route("AddUser")]
        public IActionResult AddUser(PortalUsers model)
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
               var checkuserexist= _context.PortalUsers.Where(x=>x.Email== model.Email).FirstOrDefault();
                if (checkuserexist != null)
                {

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Conflict,
                        Message = "Email Already Exsist"
                    });
                }

                model.CreatedOn = DateTime.Now;
                model.UpdatedOn = DateTime.Now;
                model.IsActive = model.IsActive;
                model.CreatedBy = model.CreatedBy;
                model.UpdatedBy = model.UpdatedBy;
                model.IsActive = true;

                _context.PortalUsers.Add(model);
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

        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult UpdateUser([FromBody] PortalUsers model)
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

                var data = _context.PortalUsers.Find(model.ID);

                if (data != null)
                {

                    data.UpdatedOn = DateTime.Now;
                    data.UpdatedBy = model.UpdatedBy;

                    _context.SaveChanges();

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Success,
                        Message = "Success",
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
        [Route("DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var data = _context.PortalUsers.Find(id);
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
