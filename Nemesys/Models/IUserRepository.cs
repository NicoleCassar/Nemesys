using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nemesys.Models
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> GetReportersByNumberOfReports();
    }
}
