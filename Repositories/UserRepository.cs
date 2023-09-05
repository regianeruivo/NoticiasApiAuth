using Microsoft.AspNetCore.Identity;
using NoticiasApiAuth.Models;

namespace NoticiasApiAuth.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "Regiane", Password = "123", Role="Manager"}
        };
            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();

        }

    }
}
