using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyManagerApi.Services.Auth;
using MoneyManagerApi.Services.Db;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
#if DEBUG
#else
var port = Environment.GetEnvironmentVariable("PORT");
builder.WebHost.UseUrls("http://*:" + port);
#endif
builder.Services.AddControllers();

//JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// Auth initialization
builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(jwtOptions =>
   {
       var jwtKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
       var keyBytes = Encoding.ASCII.GetBytes(jwtKey);
       jwtOptions.SaveToken = true;
       jwtOptions.TokenValidationParameters = new TokenValidationParameters
       {
           IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
           ValidateIssuer = false,
           ValidateAudience = false,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ClockSkew = TimeSpan.Zero
       };
   });

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsapp",
        builder =>
        {
            builder.SetIsOriginAllowed(origin => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials(); //TODO change to url
        });
});

builder.Services.AddDbContext<PostgreSqlDbContext>();
builder.Services.AddTransient<IAuthService, BearerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTRefreshTokens", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "This site uses Bearer token and you have to pass" +
        "it as Bearer<<space>>Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Scheme="oauth2",
                        Name="Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                    }
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
