using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using University.Models;

namespace University.DAL.Repositories
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        private SchoolContext context;

        public StudentRepository(SchoolContext context)
        {
            this.context = context;
        }

        public void AddStudent(Student student)
        {
            context.Students.Add(student);
        }

        public async void DeleteStudent(Student studentId)
        {
            Student student = await context.Students.FindAsync(studentId);
            context.Students.Remove(student);
        }

        public Task<Student> GetStudentByID(int studentId)
        {
            return context.Students.FindAsync(studentId);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return context.Students.ToList();
        }

        public Task<int> Save()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateStudent(Student student)
        {
            context.Entry(student).State = EntityState.Modified;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StudentRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}