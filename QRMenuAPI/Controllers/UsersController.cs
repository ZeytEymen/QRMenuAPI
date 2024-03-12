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

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult> LogIn(string username, string password)
        {
            //var applicationUser = await _userManager.FindByNameAsync(username);
            var applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            if (applicationUser == null)
            {
                return NotFound();
            }

            var result =  await _signInManager.PasswordSignInAsync(applicationUser, password,false,false);
            if (result.Succeeded)
            {
                return Ok("Giriş Yapıldı");
            }
            return Unauthorized("Sen yoksun aga aramızda");
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string username, string newPassword)
        {
            var applicationUser = await _signInManager.UserManager.FindByNameAsync(username);
            if (applicationUser == null)
            {
                return Unauthorized("Kullanıcı bulunamadı");
            }
            var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser);
            // _signInManager.UserManager.re
            //var result = await _signInManager.UserManager.ResetPasswordAsync(applicationUser, null, newPassword);
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            var result = await _signInManager.UserManager.AddPasswordAsync(applicationUser,newPassword);
            if (result.Succeeded)
            {
                return Ok("Şifre sıfırlama başarılı");
            }
            return BadRequest();
        }
        [HttpPost("ForgotPassword2")]
        public async Task<ActionResult> ForgotPassword2(string username, string newPassword)
        {
            var applicationUser = await _signInManager.UserManager.FindByNameAsync(username);
            if (applicationUser == null)
            {
                return Unauthorized("Kullanıcı bulunamadı");
            }
            var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser);
            var result = await _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassword);
            if (!result.Succeeded)
            {
                //return result.Errors.First().Description;
            }
            return Ok("Şifre sıfırlama başarılı");
        }



        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            //return await _context.Users.ToListAsync();
            return await _signInManager.UserManager.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {

            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult PutApplicationUser(string id, ApplicationUser applicationUser, string? password = null, string? currentPassword = null)
        {
            var existingApplicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;

            _signInManager.UserManager.UpdateAsync(existingApplicationUser).Wait();

            if (password != null && currentPassword != null)
            {
               IdentityResult identityResult = _signInManager.UserManager.ChangePasswordAsync(existingApplicationUser, currentPassword, password).Result;
            }
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostApplicationUser(ApplicationUser applicationUser, string password)
        {
            
            await _signInManager.UserManager.CreateAsync(applicationUser, password);
            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {
            var applicationUser =  await _signInManager.UserManager.FindByIdAsync(id);
            //var applicationUser = await _context.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;
            await _signInManager.UserManager.UpdateAsync(applicationUser);
            return NoContent();
        }

        private bool ApplicationUserExists(string id)
        {
            return (_signInManager.UserManager.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
