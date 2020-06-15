using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            //FirstOrDefault will return username if it exists or else null
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username); 

            if (user == null) {
                return null; 
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null; 
            }

            return user; 
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //NC chose this hashing algorithm, but there are many options for how to do this. 
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) return false; 
                }
            }
            return true; 
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt; 
            //out keyword passes a reference to a variable instead of a value. 
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash; 
            user.PasswordSalt = passwordSalt; 

            await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync(); 

            return user; 
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //using because this all needs to be disposed after use
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key; //hmac.Key is randomly generated key
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            //Does username exist in DB?
            if (await _context.Users.AnyAsync(x => x.Username == username)) {
                return true; 
            }

            return false; 
        }
    }
}