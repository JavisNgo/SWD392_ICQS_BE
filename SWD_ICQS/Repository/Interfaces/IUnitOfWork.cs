using SWD_ICQS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWD_ICQS.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Accounts> AccountRepository { get; }

        IGenericRepository<Appointments> AppointmentRepository { get; }

        IGenericRepository<BlogImages> BlogImageRepository { get; }

        IGenericRepository<Blogs> BlogRepository { get; }

        IGenericRepository<Categories> CategoryRepository { get; }

        IGenericRepository<ConstructImages> ConstructImageRepository { get; }

        IGenericRepository<ConstructProducts> ConstructProductRepository { get; }

        IGenericRepository<Constructs> ConstructRepository { get; }

        IGenericRepository<Contractors> ContractorRepository { get; }

        IGenericRepository<Contracts> ContractRepository { get; }

        IGenericRepository<Customers> CustomerRepository { get; }

        IGenericRepository<DepositOrders> DepositOrdersRepository { get; }

        IGenericRepository<Messages> MessageRepository { get; }

        IGenericRepository<ProductImages> ProductImageRepository { get; }

        IGenericRepository<Products> ProductRepository { get; }

        IGenericRepository<RequestDetails> RequestDetailRepository { get; }

        IGenericRepository<Requests> RequestRepository { get; }
        IGenericRepository<Token> TokenRepository { get; }

        void Save();
    }
}
