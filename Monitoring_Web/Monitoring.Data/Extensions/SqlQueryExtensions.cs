using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Monitoring.Data.Extensions
{
    // Please see the GitHub Issue for the Original Code:
    //
    //      https://github.com/dotnet/efcore/issues/1862
    //

    public static class SqlQueryExtensions
    {
        public static IList<T> SqlQuery<T>(this DbContext db, string sql, params object[] parameters) where T : class
        {
            return db.Set<T>().FromSqlRaw(sql, parameters).ToList();
        }

        public static Task<List<T>> SqlQueryAsync<T>(this DbContext db, string sql, CancellationToken cancellationToken, params object[] parameters) where T : class
        {
            return db.Set<T>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
        }

        private class ContextForQueryType<T> : DbContext where T : class
        {
            private readonly DbConnection connection;

            public ContextForQueryType(DbConnection connection)
            {
                this.connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<T>().HasNoKey();
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}
