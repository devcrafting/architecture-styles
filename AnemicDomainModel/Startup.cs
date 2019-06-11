﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnemicDomainModel.Domain;
using AnemicDomainModel.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnemicDomainModel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var connection = "Data Source=trivia.db";
            services.AddDbContext<TriviaDbContext>(
                options => options.UseSqlite(connection))
                .AddTransient<GameServices>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IQuestionRepository, QuestionRepository>()
                .AddTransient<IPlayerRepository, PlayerRepository>()
                .AddTransient<IRollDice, RandomDice>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
                {
                    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }
}
