// <copyright file="AccountsController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>
namespace QCVOC.Api.Data.DTO
{
    using System;
    using QCVOC.Api.Data.Model;
    using QCVOC.Api.Data.Model.Security;

    public class PatronResponse
    {
        #region Public Properties

        public Guid Id { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Email { get; set; }
        public DateTime EnrollmentDate { get; set; }

        #endregion Public Properties

        public PatronResponse(
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
    }
}