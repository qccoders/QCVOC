// <copyright file="Repository{T}.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper.Contrib.Extensions;
    using QCVOC.Server.Data.ConnectionFactory;
    using QCVOC.Server.Data.Model.Security;
    using Dapper;
    using System.Linq;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    ///     A repository for application user Accounts.
    /// </summary>
    public class Repository<T> : IRepository<T>
        where T : class
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Repository{T}"/> class with the specified <paramref name="dbConnectionFactory"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection with which to persist Account information.</param>
        public Repository(IDbConnectionFactory dbConnectionFactory)
        {
            ConnectionFactory = dbConnectionFactory;

            Dapper.SqlMapper.SetTypeMap(
    typeof(RefreshToken),
    new CustomPropertyTypeMap(
        typeof(RefreshToken),
        (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
                prop.GetCustomAttributes(false)
                    .OfType<ColumnAttribute>()
                    .Any(attr => attr.Name == columnName))));
        }

        #endregion Public Constructors

        #region Private Properties

        private IDbConnectionFactory ConnectionFactory { get; set; }

        #endregion Private Properties

        #region Public Methods

        public void Create(T resource)
        {

            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                var id = db.Insert<T>(resource);
                id = id;
            }
        }

        public void Delete(Guid id)
        {
            Delete(Get(id));
        }

        public void Delete(T resource)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                db.Delete<T>(resource);
            }
        }

        public T Get(Guid id)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                return db.Get<T>(id);
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                return db.GetAll<T>();
            }
        }

        public void Update(T resource)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                db.Update<T>(resource);
            }
        }

        #endregion Public Methods
    }
}