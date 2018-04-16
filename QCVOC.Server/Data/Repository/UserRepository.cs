using QCVOC.Server.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QCVOC.Server.Data.Repository
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        #region Private Fields

        private User testUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = "test",
            PasswordHash = "EE26B0DD4AF7E749AA1A8EE3C10AE9923F618980772E473F8819A5D4940E0DB27AC185F8A0E1D5F84F88BC887FD67B143732C304CC5FA9AD8E6F57F50028A8FF", // test
            Role = Role.Administrator
        };

        #endregion Private Fields

        #region Public Constructors

        public UserRepository()
        {
            Users = new List<User>();
            Users.Add(testUser); // remove this later
        }

        #endregion Public Constructors

        #region Private Properties

        private List<User> Users { get; set; }

        #endregion Private Properties

        #region Public Methods

        public void Create(User user)
        {
        }

        public void Delete(Guid id)
        {
        }

        public void Delete(User user)
        {
        }

        public User Get(Guid id)
        {
            return Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public User Get(string name)
        {
            return Users.Where(u => u.Name == name).FirstOrDefault();
        }

        public IList<User> GetAll()
        {
            return Users;
        }

        public void Update(Guid id, User updatedUser)
        {
        }

        public void Update(User user, User updatedUser)
        {
        }

        #endregion Public Methods
    }
}