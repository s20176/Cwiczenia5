using System.Collections.Generic;
using System.Threading.Tasks;
using Zad1.Models.DTO;

namespace Zad1.Services
{
    public interface IDbService
    {
        public Task<IEnumerable<TripDTO>> getTrips();
        public Task removeTrip(int id);
        public Task removeClient(int id);
        public Task addClientToTrip(int id, ClientTripDTO clientTrip);
    }
}
