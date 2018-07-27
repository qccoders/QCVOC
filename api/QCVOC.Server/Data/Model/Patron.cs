using System;
using Dapper.Contrib.Extensions;

namespace QCVOC.Server.Data.Model
{
    public class Patron : IEquatable<Patron>
    {
        #region Public Properties

        [ExplicitKey]
        public Guid Id { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PrimaryPhone { get; set;}
        public string SecondaryPhone { get; set; }
        public string Email { get; set; }
        public DateTime EnrollmentDate { get; set; }

        #endregion Public Properties
        public Patron(){}
        public Patron(
            Guid id, 
            int memberId, 
            string firstName, 
            string lastName, 
            string address, 
            string primaryPhone, 
            string secondaryPhone,
            string email,
            DateTime enrollmentDate)
            {
                Id = id;
                MemberId = memberId;
                FirstName = firstName;
                LastName = lastName;
                Address = address;
                PrimaryPhone = primaryPhone;
                SecondaryPhone = secondaryPhone;
                Email = email;
                EnrollmentDate = enrollmentDate;
            }

        public bool Equals(Patron patron)
        {
            if(patron == null)
                return this == null;

            return this.Id == patron.Id
            && this.MemberId == patron.MemberId
            && this.FirstName == patron.FirstName
            && this.LastName == patron.LastName
            && this.Address == patron.Address
            && this.PrimaryPhone == patron.PrimaryPhone
            && this.SecondaryPhone == patron.SecondaryPhone
            && this.Email == patron.Email
            && this.EnrollmentDate - patron.EnrollmentDate <= TimeSpan.FromSeconds(1);
        }
    }
}