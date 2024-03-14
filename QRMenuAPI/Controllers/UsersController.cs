using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        #region HttpGet

        [HttpGet]
        public ActionResult<List<ApplicationUser>> GetUsers()
        {
            return _signInManager.UserManager.Users.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<ApplicationUser> GetApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
                return NotFound();

            return applicationUser;
        }
        #endregion

        #region HttpPost

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpPost]
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord)
        {
            _signInManager.UserManager.CreateAsync(applicationUser, passWord).Wait();
            return applicationUser.Id;
        }

        [HttpPost("LogIn")]
        public bool LogIn(string userName, string passWord)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            Claim claim;

            if (applicationUser == null)
            {
                return false;
            }
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, passWord, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                claim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
                _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
            }
            return signInResult.Succeeded;
        }

        [HttpPost("RemovePassswordAddPassword")]
        public void RemovePassswordAddPassword(string userName, string passWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (applicationUser == null)
            {
                return;
            }
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser, passWord);
        }


        [HttpPost("GetPasswordResetToken")]
        public string? GetPasswordResetToken(string userName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (applicationUser == null)
            {
                return null;
            }
            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
        }

        [HttpPost("ValidateToken")]
        public ActionResult<string?> ValidateToken(string userName, string token, string newPassWord)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassWord).Result;
            if (identityResult.Succeeded == false)
            {
                return identityResult.Errors.First().Description;
            }
            return Ok();
        }

        [HttpPost("AssignRole")]
        public void AssignRole(string userId, string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleId).Result;

            _signInManager.UserManager.AddToRoleAsync(applicationUser, identityRole.Name).Wait();
        }


        #endregion

        #region HttpPut

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpPut("{id}")]
        public OkResult PutApplicationUser(ApplicationUser applicationUser)
        {
            ApplicationUser existingApplicationUser = _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;

            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;
            existingApplicationUser.UserName = applicationUser.UserName;
            _signInManager.UserManager.UpdateAsync(existingApplicationUser);
            return Ok();
        }
        #endregion

        #region HttpDelete

        [Authorize(Roles = "CompanyAdministrator")]
        [HttpDelete("{id}")]
        public ActionResult DeleteApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;
            _signInManager.UserManager.UpdateAsync(applicationUser);
            return Ok();
        }
        #endregion

    }
}

//private bool ApplicationUserExists(string id)
//{
//    return (_signInManager.UserManager.Users?.Any(e => e.Id == id)).GetValueOrDefault();
//}

//[HttpPost("LogIn")]
//public async Task<ActionResult> LogIn(string username, string password)
//{
//    //var applicationUser = await _userManager.FindByNameAsync(username);
//    var applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
//    if (applicationUser == null)
//    {
//        return NotFound();
//    }

//    var result =  await _signInManager.PasswordSignInAsync(applicationUser, password,false,false);
//    if (result.Succeeded)
//    {
//        return Ok("Giriş Yapıldı");
//    }
//    return Unauthorized("Sen yoksun aga aramızda");
//}