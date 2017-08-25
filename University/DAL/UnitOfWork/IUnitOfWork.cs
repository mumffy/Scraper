using System;
using University.DAL.Repositories;
using University.Models;

namespace University.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        GenericRepository<Department> DepartmentRepository { get; }
        GenericRepository<Course> CourseRepository { get; }
        int ExecuteSqlCommand(string sql, params Object[] parameters);
    }
}
