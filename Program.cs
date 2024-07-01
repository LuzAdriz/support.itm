using commons.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Utilities.Middleware;

const string Secret = "TSFZKz9iNDpOLGRXQCkmbTEoMlBoTk4wdW16MzRuYSwoVWUqVWRZY2hEMU5BJEJTZkhycjQkSygzVyVbLyxiUC9SZ2UjdzUxUWMuLXBoSmtQKDZAWkdjOEthV3orcTpAeix6YQ==";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCustomServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<UnblockUserHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(authenticationBuilder =>
{
    authenticationBuilder.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationBuilder.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    byte[] symmetricKey = Convert.FromBase64String(Secret);

    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in allAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && type.IsPublic)
                        {
                            if (type.Namespace != null && type.Namespace.StartsWith("Commons.Models"))
                            {
                                var schemaId = type.Name;
                                var schema = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = schemaId } };

                                if (!swaggerDoc.Components.Schemas.ContainsKey(schemaId))
                                {
                                    swaggerDoc.Components.Schemas.Add(schemaId, schema);
                                }
                            }
                        }
                    }
                }
            });
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            // ...
        });
    }
}
app.UseCors("AllowLocalhost4200");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.Run();
