using Microsoft.EntityFrameworkCore;
using SWD_ICQS.Entities;
using SWD_ICQS.Repository.Interfaces;

namespace SWD_ICQS.Repository.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        private GenericRepository<Accounts> _accountRepository;
        private GenericRepository<Contractors> _contractorsRepository;
        private GenericRepository<Customers> _customersRepository;
        private GenericRepository<Categories> categoryRepository;
        private GenericRepository<Subscriptions> subscriptionRepository;
        private GenericRepository<Blogs> blogRepository;
        private GenericRepository<BlogImages> blogImageRepository;
        private GenericRepository<Messages> messagesRepository;
        private GenericRepository<Products> productsRepository;

        private GenericRepository<Constructs> constructsRepository;

        private GenericRepository<Orders> ordersRepository;
        private GenericRepository<ConstructProducts> constructProductsRepository;
        public GenericRepository<ConstructImages> constructImagesRepository;
        public GenericRepository<ProductImages> productImagesRepository;
        public GenericRepository<Requests> requestsRepository;
        public GenericRepository<RequestDetails> requestDetailsRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }



        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        public IGenericRepository<Accounts> AccountRepository => _accountRepository ??= new GenericRepository<Accounts>(context);

        public IGenericRepository<Appointments> AppointmentRepository => throw new NotImplementedException();

        public IGenericRepository<ConstructImages> ConstructImageRepository
        {
            get
            {
                if (constructImagesRepository == null)
                {
                    this.constructImagesRepository = new GenericRepository<ConstructImages>(context);
                }
                return constructImagesRepository;
            }
        } 
       
        public IGenericRepository<ConstructProducts> ConstructProductRepository
        {
            get
            {
                if(constructProductsRepository == null)
                {
                    this.constructProductsRepository = new GenericRepository<ConstructProducts>(context);
                }
                return constructProductsRepository;
            }
        }
        public IGenericRepository<Constructs> ConstructRepository
        {
            get
            {
                if(constructsRepository == null)
                {
                    this.constructsRepository = new GenericRepository<Constructs>(context);
                }
                return constructsRepository;
            }
        }

        public IGenericRepository<Contractors> ContractorRepository => _contractorsRepository ??= new GenericRepository<Contractors>(context);

        public IGenericRepository<Contracts> ContractRepository => throw new NotImplementedException();

        public IGenericRepository<Customers> CustomerRepository => _customersRepository ??= new GenericRepository<Customers>(context);


        public IGenericRepository<Orders> OrderRepository
        {
            get
            {
                if (ordersRepository == null)
                {
                    this.ordersRepository = new GenericRepository<Orders>(context);
                }
                return ordersRepository;
            }
        }

        public IGenericRepository<Messages> MessageRepository
        {
            get
            {
                if(messagesRepository == null)
                {
                    this.messagesRepository = new GenericRepository<Messages>(context);
                }
                return messagesRepository;
            }
        }


        public IGenericRepository<ProductImages> ProductImageRepository
        {
            get
            {
                if(productImagesRepository == null)
                {
                    this.productImagesRepository = new GenericRepository<ProductImages>(context);
                }
                return productImagesRepository;
            } 
                
        }

        public IGenericRepository<Products> ProductRepository
        {
            get
            {
                if(productsRepository == null)
                {
                    this.productsRepository = new GenericRepository<Products>(context);
                }
                return productsRepository;
            }
        }


        public IGenericRepository<RequestDetails> RequestDetailRepository
        {
            get
            {
                if (requestDetailsRepository == null)
                {
                    this.requestDetailsRepository = new GenericRepository<RequestDetails>(context);
                }
                return requestDetailsRepository;
            }
        }
        public IGenericRepository<Requests> RequestRepository
        {
            get
            {
                if(requestsRepository ==null)
                {
                    this.requestsRepository = new GenericRepository<Requests>(context);
                }
                return requestsRepository;
            }
        }

        public IGenericRepository<BlogImages> BlogImageRepository
        {
            get
            {
                if (blogImageRepository == null)
                {
                    blogImageRepository = new GenericRepository<BlogImages>(context);
                }
                return blogImageRepository;
            }
        }
        public IGenericRepository<Blogs> BlogRepository
        {
            get
            {
                if (blogRepository == null)
                {
                    blogRepository = new GenericRepository<Blogs>(context);
                }
                return blogRepository;
            }
        }
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
