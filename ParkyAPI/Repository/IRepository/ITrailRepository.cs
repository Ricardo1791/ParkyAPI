using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrail();
        ICollection<Trail> GetTrailsInNationalPark(int npId);
        Trail GetTrail(int id);
        bool TrailExist(string name);
        bool TrailExist(int id);
        bool CreateTrail(Trail trail);
        bool UpdateTrail(Trail trail);
        bool DeleteTrail(Trail trail);
        bool Save();
    }
}
