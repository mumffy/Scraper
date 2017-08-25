using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using University.Logging;

namespace University.DAL
{
    public class SchoolInterceptorTransientErrors : DbCommandInterceptor
    {
        const int COUNTER_MAX = 4;
        private int _counter = 0;
        private ILogger _logger = new Logger();

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%THROW%")
            {
                command.Parameters[0].Value = "%k%";
                command.Parameters[1].Value = "%k%";

                if (_counter < COUNTER_MAX)
                {
                    _logger.Info("Returning transient error for command: {0}", command.CommandText);
                    _counter++;
                    interceptionContext.Exception = CreateDummySqlException();
                }
                else
                {
                    // reset counter without causing Exception to be set/thrown
                    _counter = 0;
                }
            }
        }

        private SqlException CreateDummySqlException()
        {
            // The instance of SQL Server you attempted to connect to does not support encryption
            var sqlErrorNumber = 20;

            var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
            var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

            var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
            addMethod.Invoke(errorCollection, new[] { sqlError });

            var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
            var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

            return sqlException;
        }
    }
}