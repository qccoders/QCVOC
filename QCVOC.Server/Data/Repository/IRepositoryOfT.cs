using System;
using System.Collections.Generic;

namespace QCVOC.Server.Data.Repository
{
    internal interface IRepository<T>
    {
        #region Public Methods

        void Create(T newResource);

        void Delete(Guid id);

        void Delete(T resource);

        T Get(Guid id);

        IList<T> GetAll();

        void Update(Guid id, T updatedResource);

        void Update(T resource, T updatedResource);

        #endregion Public Methods
    }
}