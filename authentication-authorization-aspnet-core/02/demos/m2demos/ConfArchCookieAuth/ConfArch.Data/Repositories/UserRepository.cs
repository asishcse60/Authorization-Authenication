using System.Collections.Generic;
using System.Linq;
using ConfArch.Data.Models;


namespace ConfArch.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private List<User> users = new List<User>
        {
            new User { Id = 3522, Name = "ASU PAL", Password = "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=",
                FavoriteColor = "blue", Role = "Admin", GoogleId = "108484214703889794937" , Email = "thimpu.cse@gmail.com"}
        };

        public User GetByUsernameAndPassword(string username, string password)
        {
            var user = users.SingleOrDefault(u => u.Name == username &&
                u.Name == "roland");//u.Password == password.Sha256()
            return user;
        }

        public User GetByGoogleId(string googleId)
        {
            var user = users.SingleOrDefault(u => u.GoogleId == googleId);
            return user;
        }
        public User GetByGoogleEmail(string email)
        {
            var user = users.SingleOrDefault(u => u.Email== email);
            return user;
        }
    }
}
