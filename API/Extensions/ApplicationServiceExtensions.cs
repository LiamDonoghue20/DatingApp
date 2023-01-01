using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using API.Interfaces;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using API.helpers;
using API.DTOs;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,  IConfiguration config)
        {
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
                services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
                services.AddScoped<IPhotoService, PhotoService>();
                services.AddScoped<LogUserActivity>();
                services.AddScoped<ILikesRepository, LikesRepository>();
                services.AddScoped<IMessageRepository, MessageRepository>();

            return services;
        }
    }
}