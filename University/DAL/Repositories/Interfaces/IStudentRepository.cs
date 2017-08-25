using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using University.Models;

namespace University.DAL.Repositories
{
    public interface IStudentRepository : IDisposable
    {
        IEnumerable<Student> GetAllStudents();
        Task<Student> GetStudentByID(int studentId);
        void AddStudent(Student student);
        void DeleteStudent(Student student);
        void UpdateStudent(Student student);
        Task<int> Save();
    }
}