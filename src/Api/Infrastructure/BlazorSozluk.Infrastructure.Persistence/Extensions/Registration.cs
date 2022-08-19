using System;
using BlazorSozluk.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BlazorSozluk.Infrastructure.Persistence.Extensions
{
    public  static class Registration
    {
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddDbContext<BlazorSozlukContext>(conf =>
            {
                var conStr = configuration["BlazorSozlukDbConnectionString"].ToString();
                conf.UseSqlServer(conStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
            });

            var seedData = new SeedData();

            seedData.SeedAsync(configuration).GetAwaiter().GetResult();

            return services;

        }
    }
}

