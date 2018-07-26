
using System.Data;
using QCVOC.Server.Data.ConnectionFactory;
using QCVOC.Server.Data.Model.Security;
using Dapper.Contrib.Extensions;
using Dapper;
using System;
using System.Text;
using System.Collections.Generic;
using QCVOC.Server.Data.Model;

namespace QCVOC.Server.Data.Repository
{
    public class PatronRepository : IRepository<Patron>
    {
        private IDbConnectionFactory ConnectionFactory { get; }
        
        public PatronRepository(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        public Patron Create(Patron patron)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(@"
                INSERT INTO patrons 
                (id, memberid, firstname, lastname, address, primaryphone, secondaryphone, email, enrollmentdate)
                VALUES (@id, @memberId, @firstName, @lastName, @address, @primaryPhone, @secondaryPhone, @email, @enrollmentDate)
                ", new
                {
                    id = patron.Id,
                    memberId = patron.MemberId,
                    firstName = patron.FirstName,
                    lastName = patron.LastName,
                    address = patron.Address,
                    primaryPhone = patron.PrimaryPhone,
                    secondaryPhone = patron.SecondaryPhone,
                    email = patron.Email,
                    enrollmentDate = patron.EnrollmentDate,
                });

                var inserted = Get(patron.Id);
                return inserted;
            }
        }

        public void Delete(Guid id)
        {
            using (var db = ConnectionFactory.CreateConnection())
            {
                db.Execute("DELETE FROM patrons WHERE id = @id", new { id = id });
            }
        }

        public void Delete(Patron account)
        { 
            if(account == null)
                throw new ArgumentException("patron cannot be null.", nameof(account));

            Delete(account.Id);
        }

        public Patron Get(Guid id)
        {
            using(var db = ConnectionFactory.CreateConnection())
            {
                return db.QueryFirstOrDefault<Patron>(@"
                SELECT id, memberid, firstname, lastname, address, primaryphone,
                secondaryphone, email, enrollmentdate FROM patrons WHERE id = @id;", new { id = id });
            }
        }

        public IEnumerable<Patron> GetAll()
        {
            using(var db = ConnectionFactory.CreateConnection())
            {
                return db.Query<Patron>(@"
                SELECT id, memberid, firstname, lastname, address, primaryphone, 
                secondaryphone, email, enrollmentdate FROM patrons;");
            }
        }

        public Patron Update(Patron patron)
        {
            using(var db = ConnectionFactory.CreateConnection())
            {
                db.Execute(@"
                UPDATE patrons
                SET memberid = @memberId,
                firstname = @firstName,
                lastname = @lastName,
                address = @address,
                primaryphone = @primaryPhone,
                secondaryphone = @secondaryPhone,
                email = @email,
                enrollmentdate = @enrollmentDate
                WHERE id = @id", new {
                    memberId = patron.MemberId,
                    firstName = patron.FirstName,
                    lastName = patron.LastName,
                    address = patron.Address,
                    primaryPhone = patron.PrimaryPhone,
                    secondaryPhone = patron.SecondaryPhone,
                    email = patron.Email,
                    enrollmentDate = patron.EnrollmentDate,
                    id = patron.Id
                });

                return Get(patron.Id);
            }
        }
    }
}