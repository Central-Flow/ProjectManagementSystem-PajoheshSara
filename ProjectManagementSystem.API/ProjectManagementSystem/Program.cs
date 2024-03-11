
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.ServiceExtension;

var builder = WebApplication.CreateBuilder(args);

//                                                                  Services Configuration
//Controllers
builder.Services.ConfigureMvc();

//DbContext
builder.Services.ConfigureDbContext(builder);

//Swagger
builder.Services.ConfigureSwagger();

//Auth
builder.Services.ConfigureAuth(builder);

//InternalServices
builder.Services.ConfigureAuthenticationservice();




var app = builder.Build();
//                                                                  HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
