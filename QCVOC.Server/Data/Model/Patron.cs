using System;

namespace QCVOC.Server.Data.Model
{
    public class Patron
    {
        #region Public Properties

        public Guid Id { get; set; }
        public string Name { get; set; }
        // todo: find a standard list of properties for people, e.g. address1, address2, etc.
        
        #endregion Public Properties
    }
}