using System;
using QRMenuAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace QRMenuAPI.Controllers
{
	public class InitializeData
	{
		private readonly ApplicationContext _applicationContext;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;


        public InitializeData(ApplicationContext applicationContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_applicationContext = applicationContext;
			_userManager = userManager;
			_roleManager = roleManager;

            string[] roleNames = { "Administrator", "CompanyAdministrator" };
			foreach (var item in roleNames)
			{
				var roleExist = _roleManager.RoleExistsAsync(item).Result;
				if (!roleExist)
				{
                    roleManager.CreateAsync(new IdentityRole(item));
                }
			}
        }
	}
}

