using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;
using System.Security.Claims;

namespace Shopping_Tutorial.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManage;
		private SignInManager<AppUserModel> _signInManager;
		//private readonly IEmailSender _emailSender;
		private readonly DataContext _dataContext;
		public AccountController(
			UserManager<AppUserModel> userManage,
			SignInManager<AppUserModel> signInManager,
			DataContext context)
		{
			_dataContext = context;
			_userManage = userManage;
			_signInManager = signInManager;
			
		}
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}
		public async Task<IActionResult> NewPass(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgetPass", "Account");
			}
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
		{
			var checkuser = await _userManage.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();

			if (checkuser != null)
			{
				//update user with new password and token
				string newtoken = Guid.NewGuid().ToString();
				// Hash the new password
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

				checkuser.PasswordHash = passwordHash;
				// -- Hash the new password
				checkuser.Token = newtoken;

				await _userManage.UpdateAsync(checkuser);
				TempData["success"] = "Password updated successfully.";
				return RedirectToAction("Login", "Account");
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgetPass", "Account");
			}
			return View();
		}
		[HttpPost]
		//public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
		//{
		//	var checkMail = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

		//	if (checkMail == null)
		//	{ //ko có email trả về
		//		TempData["error"] = "Email not found";
		//		return RedirectToAction("ForgetPass", "Account");
		//	}
		//	else //có
		//	{
		//		string token = Guid.NewGuid().ToString(); //242682e8 - c293 - 4121 - a05b - e441a9c65d48
		//												  //update token to user
		//		checkMail.Token = token;
		//		_dataContext.Update(checkMail);
		//		await _dataContext.SaveChangesAsync();
		//		//--update token to user
		//		var receiver = checkMail.Email;
		//		var subject = "Change password for user " + checkMail.Email; //Change password for user nguyenducan1526@gmail.com
		//		var message = "Click on link to change password " +
		//			"<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

				
		//	}


		//	TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
		//	return RedirectToAction("ForgetPass", "Account");
		//}
		public async Task<IActionResult> ForgetPass(string returnUrl)
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					TempData["success"] = "Đăng nhập thành công";
					var receiver = "danhphong20041222@gmail.com";
					var subject = "Đăng nhập trên thiết bị thành công.";
					var message = "Đăng nhập thành công, trải nghiệm dịch vụ nhé.";

					//await _emailSender.SendEmailAsync(receiver, subject, message);
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Sai tài khoản hặc mật khẩu");
			}
			return View(loginVM);
		}

		public IActionResult Create()
		{
			return View();
		}
		public async Task<IActionResult> History()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				// User is not logged in, redirect to login
				return RedirectToAction("Login", "Account"); // Replace "Account" with your controller name
			}
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var Orders = await _dataContext.Orders
			   .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();

			ViewBag.UserEmail = userEmail;
			return View(Orders);
		}
		public async Task<IActionResult> CancelOrder(string ordercode)
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				// User is not logged in, redirect to login
				return RedirectToAction("Login", "Account");
			}

			try
			{
				var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
				order.Status = 3;
				_dataContext.Update(order);
				await _dataContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{

				return BadRequest("An error occurred while canceling the order.");
			}


			return RedirectToAction("History", "Account");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName = user.Username, Email = user.Email };
				IdentityResult result = await _userManage.CreateAsync(newUser, user.Password);
				if (result.Succeeded)
				{
					TempData["success"] = "Tạo thành viên thành công";
					return Redirect("/account/login");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}

		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}
	}
}
