using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetIdentitydemo.Identity
{
    public class MyUserStore : IUserStore<MyUser>, IUserPasswordStore<MyUser>
    {
        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Data Source=(LocalDb)\\MSSQLLocalDB);" +
                                    "database=AspIdentityDemo;" +
                                    "trusted_connection=yes;");
            connection.Open();

            return connection;
        }

        public async Task<IdentityResult> CreateAsync(MyUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "INSERT INTO MyUsers([Id]," +
                    "[UserName]," +
                    "[NormalizedUserName]," +
                    "[PasswordHash]" +
                    "VALUES (@id, @username, @normalizedusername, @passwordhash)",
                    new
                    {
                        id = user.Id,
                        username = user.UserName,
                        normalizedusername = user.NormalizedUserName,
                        passwordhash = user.PasswordHash
                    });
            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(MyUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<MyUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MyUser>(
                    "SELECT * FROM MyUsers WHERE [Id] = @id",
                    new
                    {
                        id = userId
                    });
            }
        }

        public async Task<MyUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MyUser>(
                    "SELECT * FROM MyUsers WHERE [NormalizedUserName] = @name",
                    new
                    {
                        name = normalizedUserName
                    });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(MyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(MyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(MyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(MyUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetNormalizedUserNameAsync(MyUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(MyUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetUserNameAsync(MyUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(MyUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "UPDATE MyUsers" +
                    "SET" +
                    "[UserName] = @username," +
                    "[NormalizedUserName] = @normalizedusername," +
                    "[PasswordHash] = @passwordhash" +
                    "WHERE [Id] = @id",
                    new
                    {
                        id = user.Id,
                        username = user.UserName,
                        normalizedusername = user.NormalizedUserName,
                        passwordhash = user.PasswordHash
                    });
            }

            return IdentityResult.Success;
        }
    }
}