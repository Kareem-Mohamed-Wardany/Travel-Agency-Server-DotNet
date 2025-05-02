
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Travel_Agency_Server.Errors;
using Travel_Agency_Server.Services.Auth;
using Travel_Agency_Server.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Travel_Agency_Server.Repository;
using Travel_Agency_Server.Helpers;
using Microsoft.OpenApi.Models;

namespace Travel_Agency_Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddScoped<IAuthService,AuthService>();
            builder.Services.AddScoped<ITripRepository, TripRepository>();

            builder.Services.AddAutoMapper(typeof(MappingProfiles)); 

            builder.Services.AddDbContext<Context>(
                op => {
                    op.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
                }
                );
            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();


            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudiance"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            builder.Services.Configure<ApiBehaviorOptions>(option => {
                    option.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                        var errorResponse = new ApiValidationErrorResponse()
                        {
                            Errors = errors
                        };
                        return new BadRequestObjectResult(errorResponse);
                    };
                });

            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Travel Agency API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
                }
                });

            });



            var app = builder.Build();


            #region DefineSystemRolesAndAdmin
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                string[] roles = { "Admin", "User" };

                // Create roles if they don't exist
                foreach (var role in roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Create the admin user if not exists
                string adminEmail = "admin@admin.com";
                string adminPassword = "Admin@123";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = "ADMIN",
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);
                    if (result.Succeeded)
                    {
                        // Add admin role
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
            #endregion



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }
    }
}
