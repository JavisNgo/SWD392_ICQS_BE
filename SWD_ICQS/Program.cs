using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SWD_ICQS.BackgroundServices;
using SWD_ICQS.Mapper;
using SWD_ICQS.Repository;
using SWD_ICQS.Repository.Implements;
using SWD_ICQS.Repository.Interfaces;
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add connection string
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Add services to the container
builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IBlogsService, BlogsService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IContractorsService, ContractorsService>();
builder.Services.AddScoped<ICustomersService, CustomersService>();
builder.Services.AddScoped<IConstructProductService, ConstructProductService>();
builder.Services.AddScoped<IConstructService, ConstructService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IRequestDetailService, RequestDetailService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDepositOrdersService, DepositOrdersService>();


// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program), typeof(MappingProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Background process
builder.Services.AddHostedService<ExpiredRequestTimeoutChangeStatusToRejectedBackgroundService>();
builder.Services.AddHostedService<DeleteStatusFalseMessageBackgroundService>();
builder.Services.AddHostedService<ExpiredFirstMeetingDateBackgroundService>();
builder.Services.AddHostedService<DeleteExpiredTokenBackgroundService>();

// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim("Role", "ADMIN"));
    options.AddPolicy("RequireContractorRole", policy => policy.RequireClaim("Role", "CONTRACTOR"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireClaim("Role", "CUSTOMER"));
    options.AddPolicy("RequireAdminOrContractorRole", policy => policy.RequireClaim("Role", "ADMIN", "CONTRACTOR"));
    options.AddPolicy("RequireAdminOrCustomerRole", policy => policy.RequireClaim("Role", "ADMIN", "CUSTOMER"));
    options.AddPolicy("RequireContractorOrCustomerRole", policy => policy.RequireClaim("Role", "CONTRACTOR", "CUSTOMER"));
    options.AddPolicy("RequireAllRoles", policy => policy.RequireClaim("Role", "ADMIN", "CONTRACTOR", "CUSTOMER"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Access static file
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "img", "blogImage")),
    RequestPath = "/img/blogImage"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "img", "constructImage")),
    RequestPath = "/img/constructImage"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "img", "contractorAvatar")),
    RequestPath = "/img/contractorAvatar"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "img", "messageImage")),
    RequestPath = "/img/messageImage"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "img", "productImage")),
    RequestPath = "/img/productImage"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "pdf", "contracts")),
    RequestPath = "/pdf/contracts"
});

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
