using System;

namespace University.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}
