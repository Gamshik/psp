using Videotheque.CustomerService.Api.Application.Services.Interfaces;
using Videotheque.CustomerService.Api.Infrastructure.Persistence;
using Videotheque.CustomerService.Api.Models.Domain;
using Videotheque.CustomerService.Api.Models.DTO;

namespace Videotheque.CustomerService.Api.Application.Services.Implementations
{
    public class CustomersService : ICustomersService
    {
        private readonly AppDbContext _context;

        public CustomersService(AppDbContext context)
        {
            _context = context;
        }

        public CustomersService() : this(new AppDbContextFactory().CreateDbContext(Array.Empty<string>()))
        {
        }

        public IEnumerable<CustomerDTO> GetAll()
        {
            return _context.Customers.Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                RegisteredAt = c.RegisteredAt
            }).ToList();
        }

        public CustomerDTO? GetById(Guid id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            return customer != null ? MapToDTO(customer) : null;
        }

        public CustomerDTO Create(CustomerDTO customerDTO)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = customerDTO.Name,
                Email = customerDTO.Email,
                PhoneNumber = customerDTO.PhoneNumber,
                RegisteredAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();
            return MapToDTO(customer);
        }

        public bool Update(CustomerDTO customerDTO)
        {
            var existing = _context.Customers.FirstOrDefault(c => c.Id == customerDTO.Id);
            if (existing == null) return false;

            existing.Name = customerDTO.Name;
            existing.Email = customerDTO.Email;
            existing.PhoneNumber = customerDTO.PhoneNumber;
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var existing = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (existing == null) return false;

            _context.Customers.Remove(existing);
            _context.SaveChanges();
            return true;
        }

        private CustomerDTO MapToDTO(Customer customer)
        {
            return new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                RegisteredAt = customer.RegisteredAt
            };
        }
    }
}