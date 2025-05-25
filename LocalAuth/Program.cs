using IdentityGrpc.Data;
using IdentityGrpc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options => { options.AddPolicy("AllowAll", x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });

builder.Services.AddScoped<AccountService>();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(x =>
    {
    x.SignIn.RequireConfirmedEmail = true;
    x.User.RequireUniqueEmail = true;
    x.Password.RequiredLength = 8;
    }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

var app = builder.Build();

//app.UseGrpcWeb();
//app.MapGrpcService<AccountService>().EnableGrpcWeb();
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Ventixe AuthServiceProvider API");
    x.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();