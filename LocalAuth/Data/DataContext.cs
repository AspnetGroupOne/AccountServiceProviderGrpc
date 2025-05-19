using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityGrpc.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
}
