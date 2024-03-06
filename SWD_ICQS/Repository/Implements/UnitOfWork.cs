using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Repository.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private GenericRepository<Categories> categoryRepository;
        private GenericRepository<Subscriptions> subscriptionRepository;


        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }



        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        public IGenericRepository<Accounts> AccountRepository => throw new NotImplementedException();

        public IGenericRepository<Appointments> AppointmentRepository => throw new NotImplementedException();

        public IGenericRepository<BlogImages> BlogImageRepository => throw new NotImplementedException();

        public IGenericRepository<Blogs> BlogRepository => throw new NotImplementedException();

        public IGenericRepository<ConstructImages> ConstructImageRepository => throw new NotImplementedException();

        public IGenericRepository<ConstructProducts> ConstructProductRepository => throw new NotImplementedException();

        public IGenericRepository<Constructs> ConstructRepository => throw new NotImplementedException();

        public IGenericRepository<Contractors> ContractorRepository => throw new NotImplementedException();

        public IGenericRepository<Contracts> ContractRepository => throw new NotImplementedException();

        public IGenericRepository<Customers> CustomerRepository => throw new NotImplementedException();

        public IGenericRepository<Messages> MessageRepository => throw new NotImplementedException();

        public IGenericRepository<Orders> OrderRepository => throw new NotImplementedException();

        public IGenericRepository<ProductImages> ProductImageRepository => throw new NotImplementedException();

        public IGenericRepository<Products> ProductRepository => throw new NotImplementedException();

        public IGenericRepository<RequestDetails> RequestDetailRepository => throw new NotImplementedException();

        public IGenericRepository<Requests> RequestRepository => throw new NotImplementedException();


        public IGenericRepository<Categories> CategoryRepository
        {
            get
            {
                if (categoryRepository == null)
                {
                    categoryRepository = new GenericRepository<Categories>(context);
                }
                return categoryRepository;
            }
        }
        public IGenericRepository<Subscriptions> SubscriptionRepository
        {
            get
            {
                if (subscriptionRepository == null)
                {
                    subscriptionRepository = new GenericRepository<Subscriptions>(context);
                }
                return subscriptionRepository;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
