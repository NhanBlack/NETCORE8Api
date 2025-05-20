# DOTNET CORE API CLEAN FOR SMALL PROJECT CRUD.
.NET 8 Clean Architecture

Công nghệ:
Backend
.NET 8,ASP.NET Core Web API, Dapper, Swagger, JWT ,Policy-based Authorization, Dependency, SQL Server

Mô hình
```
Solution/
│
├── WebAPI/        Project chính chạy API
├── Application/   Class Library chứa logic (service, interface, repo)
└── Domain/        Class Library chứa class model
```


Các bước để thêm 1 API
✅ 1. Khai báo interface và service
Interface:

public interface ICustomerService
{
    Task<IEnumerable<dynamic>> GetAllAsync();
}
Service:
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<dynamic>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}

✅ 2. Repository

public interface ICustomerRepository
{
    Task<IEnumerable<dynamic>> GetAllAsync();
}

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperContext _context;

    public CustomerRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<dynamic>> GetAllAsync()
    {
        var query = "SELECT * FROM Customers";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(query); // dynamic
    }
}

✅ 3. Inject ICustomerService vào Controller

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }
}

✅ 4. Đăng ký DI trong DependencyInjection.cs

public static IServiceCollection AddInfrastructure(this IServiceCollection services)
{
    services.AddScoped<ICustomerRepository, CustomerRepository>();
    services.AddScoped<ICustomerService, CustomerService>();
    services.AddSingleton<DapperContext>();

    return services;
}

Home main : http://localhost:9999/swagger
Login with account : admin, pass : 123456
