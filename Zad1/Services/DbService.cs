using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zad1.Models;
using Zad1.Models.DTO;

namespace Zad1.Services
{
    public class DbService : IDbService
    {

        private readonly MasterContext _dbContext;

        public DbService(MasterContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TripDTO>> getTrips()
        {
            return await _dbContext.Trips
                .Select(e=>new TripDTO
                {
                    Name= e.Name,
                    Description= e.Description,
                    DateFrom= e.DateFrom,
                    DateTo= e.DateTo,
                    MaxPeople= e.MaxPeople,
                    Countries=e.CountryTrips.Select(e=>new CountryDTO { Name=e.IdCountryNavigation.Name}).ToList(),
                    Clients=e.ClientTrips.Select(e=> new ClientDTO { FirstName=e.IdClientNavigation.FirstName, LastName=e.IdClientNavigation.LastName }).ToList()
                })
                .OrderByDescending(e=>e.DateFrom)
                .ToListAsync();
        }

        public async Task removeTrip(int id)
        {
            Trip trip = new Trip
            {
                IdTrip=id
            };
            _dbContext.Attach(trip);
            _dbContext.Remove(trip);
            await _dbContext.SaveChangesAsync(); 

        }

        public async Task removeClient(int id)
        {
            Client client = await _dbContext.Clients.Include(e=>e.ClientTrips).Where(e => e.IdClient == id).FirstOrDefaultAsync();
            if(client == null)
            {
                throw new Exception("Nie znaleziono klienta");
            }
            if (client.ClientTrips.Count > 0)
            {
                throw new Exception("Nie można usunąć klienta");
            }
            _dbContext.Remove(client);
            await _dbContext.SaveChangesAsync();
        }

        public async Task addClientToTrip(int id, ClientTripDTO clientTrip)
        {
            Client client = await _dbContext.Clients.Include(e=>e.ClientTrips).Where(e => e.Pesel.Equals(clientTrip.Pesel)).FirstOrDefaultAsync();
            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientTrip.FirstName,
                    LastName = clientTrip.LastName,
                    Email = clientTrip.Email,
                    Pesel = clientTrip.Pesel,
                    Telephone = clientTrip.Telephone
                };
                _dbContext.Clients.Add(client);
                await _dbContext.SaveChangesAsync();
            }
            bool tripExists = false;
            foreach(ClientTrip ct in client.ClientTrips)
            {
                if (ct.IdTrip == clientTrip.IdTrip)
                {
                    tripExists = true;
                }
            }
            if (tripExists)
            {
                throw new Exception("Klient jest już zapisany na tą wycieczkę");
            }
            Trip trip = await _dbContext.Trips.Where(e => e.IdTrip == clientTrip.IdTrip).FirstOrDefaultAsync();
            if(trip == null)
            {
                throw new Exception("Wycieczka nie istnieje");
            }
            DateTime now = DateTime.Now;
            ClientTrip clientTrip1 = new ClientTrip
            {
                IdClient = client.IdClient,
                IdTrip = clientTrip.IdTrip,
                PaymentDate = clientTrip.PaymentDate,
                RegisteredAt = now
            };
            _dbContext.ClientTrips.Add(clientTrip1);
            await _dbContext.SaveChangesAsync();
        }
    }
}
