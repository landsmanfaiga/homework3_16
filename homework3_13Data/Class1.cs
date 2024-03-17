using BCrypt.Net;
using System.Data.SqlClient;

namespace homework3_13Data
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
        public DateTime Date { get; set; }


    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    public class AdsWebRepository
    {
        private readonly string _connectionString;
        public AdsWebRepository(string connectionString)
        {
            _connectionString = connectionString; 
        }

        public List<Ad> GetAds()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"SELECT * FROM Ads";
            List<Ad> ads = new List<Ad>();
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                Ad ad = new Ad();
                ad.Id = (int)reader["Id"];
                ad.Title = (string)reader["Title"];
                ad.PhoneNumber = (string)reader["PhoneNumber"];
                ad.Description = (string)reader["Description"];
                ad.AccountId = (int)reader["AccountId"];
                ad.Date = (DateTime)reader["Date"];
              ads.Add(ad);

            }
            return ads;
        }

        public void AddAccount(Account account, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Accounts (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", account.Name);
            cmd.Parameters.AddWithValue("@email", account.Email);
            cmd.Parameters.AddWithValue("@hash", hash);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public Account Login(string email, string password)
        {
            var account = GetByEmail(email);
            if (account == null)
            {
                return null;
            }

            var isMatch = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
            if (!isMatch)
            {
                return null;
            }

            return account;
        }

        public Account GetByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Accounts WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Account
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }

        public void NewAd(Ad ad)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"INSERT INTO Ads(Title, PhoneNumber, Description, AccountId, Date)
                                    VALUES(@title, @phoneNumber, @description, @accountId, @date)";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@accountId", ad.AccountId);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteAd(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"DELETE FROM Ads
                                    WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    
}


       

    }
