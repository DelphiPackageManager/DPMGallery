using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Data
{
    public interface IUnitOfWork
    {

        void Rollback();

        void Commit();

    }
}
