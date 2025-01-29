using Cebelica.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Cebelica.Controllers
{
    public class AccountController : Controller
    {
        private readonly CebelicaDatabase _context;

        public AccountController(CebelicaDatabase context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register (string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "User already exists!");
                return View();
            }

            UserModel user = new UserModel
            {
                Username = username,
                //PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                PasswordHash = HashPassword(password),
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            UserModel user = _context.Users.FirstOrDefault(u => u.Username == username);

            bool verifyPass = VerifyPassword(password, user.PasswordHash);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect username or password");
                return View();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return RedirectToAction("Login", "Account");
        //}


        // Hash password using PBKDF2
        public static string HashPassword(string password)
        {
            // Salt is a random value that should be unique for each password
            using (var hmac = new HMACSHA256())
            {
                byte[] salt = new byte[16]; // 128-bit salt
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Hash the password with PBKDF2 using HMACSHA256
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000); // 10000 iterations
                byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash

                // Combine salt and hash to store them together
                byte[] hashBytes = new byte[48]; // 16 bytes for salt + 32 bytes for hash
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes); // Store this string in the database
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract the salt from the stored hash (first 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Extract the hash (next 32 bytes)
            byte[] storedHashBytes = new byte[32];
            Array.Copy(hashBytes, 16, storedHashBytes, 0, 32);

            // Hash the entered password with the same salt and number of iterations
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] enteredHash = pbkdf2.GetBytes(32);

            // Compare the two hashes
            for (int i = 0; i < 32; i++)
            {
                if (enteredHash[i] != storedHashBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
