using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using PatternRecogniser.ThreadsComunication;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using FluentValidation;
using PatternRecogniser.Models.Validators;
using PatternRecogniser.Messages.Authorization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Lib.AspNetCore.ServerSentEvents;
using PatternRecogniser.Middleware;
using PatternRecogniser.Messages.Validators;
using PatternRecogniser.Services.NewFolder;
using PatternRecogniser.Services.Repos;
using Microsoft.AspNetCore.Http.Features;
using PatternRecogniser.UnitsOfWorks;

namespace PatternRecogniser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void BindClass<BindedClassType>(IServiceCollection services, BindedClassType bindedClass, string sectionName) where BindedClassType : class
        {
            Configuration.GetSection(sectionName).Bind(bindedClass);
            services.AddSingleton(bindedClass);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            var trainingSettings = new TrainingSettings();
            BindClass(services, trainingSettings, "TrainingSettings");

            var authenticationSettings = new AuthenticationSettings();
            BindClass(services, authenticationSettings, "Authentication");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(authenticationScheme: JwtBearerDefaults.AuthenticationScheme, configureOptions: cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });

            services.Configure<FormOptions>(options =>
            {
                // 1MB = 1024^2
                options.MultipartBodyLengthLimit = 52428800;
            });
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<SignUp>, AuthentycationValidatorSingUp>();
            services.AddScoped<IValidator<LogIn>, AuthentycationValidatorLogIn>();
            services.AddScoped<ITokenCreator, TokenCreator>();
            services.AddRazorPages();
            services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Pattern Recogniser Api",
                    Description = "Design and creation by Ewa Pasterz, Piotr Skibiński, Paulina Szostek",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Przed tokenem nalezy umiescic slowo \"Bearer\". Token i \"Bearer\" powinny byc oddzielone pojedyncza spacja",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            services.AddSingleton<IBackgroundTaskQueue, BackgroundQueueLurchTable>();
            services.AddSingleton<ITrainingUpdate>(a => new SimpleComunicationOneToMany());
            services.AddSingleton<IHostedServiceDBConection, HostedServiceDBConection>();

            services.AddHostedService<TrainingModelQueuedHostedService>();

            var conectionType = Configuration["DbContextSettings:ConectionType"];
            var connectionString = Configuration[$"DbContextSettings:{conectionType}"];

            services.AddDbContext<PatternRecogniserDBContext>(
                opts => opts.UseNpgsql(connectionString)
            );
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
            services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            services.AddScoped<IGenericRepository<ExtendedModel>, GenericRepository<ExtendedModel>>();
            services.AddScoped<IGenericRepository<ExperimentList>, GenericRepository<ExperimentList>>();
            services.AddScoped<IGenericRepository<PatternRecognitionExperiment>, GenericRepository<PatternRecognitionExperiment>>();
            services.AddScoped<IGenericRepository<ModelTrainingExperiment>, GenericRepository<ModelTrainingExperiment>>();
            services.AddScoped<IGenericRepository<Experiment>, GenericRepository<Experiment>>();
            services.AddScoped<IGenericRepository<RecognisedPatterns>, GenericRepository<RecognisedPatterns>>();
            services.AddScoped<IExperimentListUnitOfWork, ExperimentListUnitOfWork>();

            services.Configure<TrainingInfoSettings>(
            Configuration.GetSection("TrainingInfoDB"));
            services.AddSingleton<ItrainingInfoService, TrainingInfoMongoCollection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                                                    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            //app.UseMiddleware<ServerSentEventsMiddleware>()

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
