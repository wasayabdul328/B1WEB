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
               var checkuserName= _context.PortalUsers.Where(x=>x.UserName== model.UserName).FirstOrDefault();
                if (checkuserName != null)
                {

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Conflict,
                        Message = "Username Already Exsist"
                    });
                }
               var checkusercnic= _context.PortalUsers.Where(x=>x.CNIC== model.CNIC).FirstOrDefault();
                if (checkusercnic != null)
                {

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Conflict,
                        Message = "CNIC Already Exsist"
                    });
                }

                model.CreatedOn = DateTime.Now;
                model.UpdatedOn = DateTime.Now;
                model.IsActive = model.IsActive;
                model.CreatedBy = 1;
                model.UpdatedBy = 1;
                model.IsActive = true;

                _context.PortalUsers.Add(model);
                _context.SaveChanges();

                return Ok(new ApiResponse<object>
                {
                    Code = ResponseCode.Success,
                    Message = "User added Successfully",
                    Data = null
                });
            }
            catch (System.Exception ex)
            {
                return Ok(UHelper.ApiExceptionResponse(ex.ToString()));
            }
        }

        [HttpPost]
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
                    data.UpdatedBy = 1;
                    data.UserName = model.UserName;
                    data.FirstName = model.FirstName;
                    data.LastName = model.LastName;
                    data.Image = model.Image;
                    data.DOB = model.DOB;
                    data.Phone = model.Phone;
                    data.CNIC = model.CNIC;
                    data.Email = model.Email;
                    data.Password = model.Password;
                    data.Gender = model.Gender;
                    data.IsAdmin = model.IsAdmin;
                    data.IsActive = true;

                    _context.Update(data);
                    _context.SaveChanges();

                    return Ok(new ApiResponse<object>
                    {
                        Code = ResponseCode.Success,
                        Message = "User Updated Successfully",
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
