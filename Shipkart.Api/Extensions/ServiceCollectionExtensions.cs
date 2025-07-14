using Shipkart.Application.Interfaces;
using Shipkart.Infrastructure.Repositories;
using Shipkart.Infrastructure.Services;

namespace Shipkart.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAuthModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILoginThrottlingService, LoginThrottlingService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            return services;
        }

        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddEmailModule(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        public static IServiceCollection AddProductModule(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }

        public static IServiceCollection AddCategoryModule(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }   
    }
}
