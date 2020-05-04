﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlexRipper.Application.Common.Interfaces;
using PlexRipper.Infrastructure.API.Plex;
using PlexRipper.Infrastructure.Common.Interfaces;
using PlexRipper.Infrastructure.Files;
using PlexRipper.Infrastructure.Persistence;
using PlexRipper.Infrastructure.Services;
using System.Reflection;

namespace PlexRipper.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<PlexRipperDbContext>(options =>
                    options.UseInMemoryDatabase("PlexRipperDb"));
            }
            else
            {
                services.AddDbContext<PlexRipperDbContext>(options =>
                    options.UseSqlite(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(PlexRipperDbContext).Assembly.FullName)));
            }

            services.AddScoped<IPlexRipperDbContext>(provider => provider.GetService<PlexRipperDbContext>());

            //services.AddDefaultIdentity<ApplicationUser>()
            //    .AddEntityFrameworkStores<PlexRipperDbContext>();

            // services.AddIdentityServer().AddApiAuthorization<ApplicationUser, PlexRipperDbContext>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            // Add services
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
            // Services
            services.AddTransient<IPlexService, PlexService>();
            services.AddTransient<IAccountService, AccountService>();
            // Api
            services.AddTransient<IPlexApi, PlexApi>();


            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}