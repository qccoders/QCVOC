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
            PasswordHash = "",
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