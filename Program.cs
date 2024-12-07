using cinemate.Data;
using cinemate.Middleware;
using cinemate.Services.DataInitializer;
using cinemate.Services.Hash;
using cinemate.Services.MovieDurationsService;
using cinemate.Services.TokenValidation;
using cinemate.Services.YouTubeService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ��������� JWT ��������������
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddSingleton<IHashService, Md5HashService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<TokenValidationService>(); // ������������ ������
//builder.Services.AddScoped<AddDurationService>();  // ����������� ������ �������

//// ����������� HttpClient ��� ������������� � AddDurationService (���� ����������)
//builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ���������� ������������ �� �����
builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

SqlConnection connection = new SqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connection, sqlOptions =>
    {
        sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema: "cinemate");
    }));


// ���������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.SetIsOriginAllowed(o=>true)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

// ���������� ������������ � ���������������
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<AddFromYouTubeService>();

var app = builder.Build();
//���������� ����������� ������ �� ������ ����������

/*using (var scope = app.Services.CreateScope())
{
    var durationService = scope.ServiceProvider.GetRequiredService<AddDurationService>();    try
   {
       await durationService.UpdateMovieDurationsAsync();  // ����������� ���������� ������ ���������� ����������������� �������
    }
   catch (Exception ex)
   {
       // ����������� ������ ��� ���������
        Console.WriteLine($"������ ��� ���������� ����������������� �������: {ex.Message}");
    }
}*/

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

    await next();
});
using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

    // ����������� ������ � Guid
    Guid categoryId = new Guid("61D20472-5017-48B9-B976-07E36A396A95");
  //  await DataInitializerService.InitializeAsync(context, categoryId);

    Guid subCategoryId1 = new Guid("24161DC2-2BB3-4E46-AA1F-848B986F28EC");
    List<String> videoId1 = new() { "RZya6omSSDw", "aCSu6iGJoX4" };

    /* Guid subCategoryId2 = new Guid("7CACAB9C-C471-4E13-BE8F-A6F73F7678D7");
      List<String> videoId2 = new() { "MfA5zpfG1fI", "as-6afI4vL8&t", "By4kCIfpjGE", "pVAWKrtJEMo"};

      Guid subCategoryId3 = new Guid("0966A6CA-BE90-4AFF-BE04-CF4EFF71236E");
      List<String> videoId3 = new() { "", "", "", "", "" };
      // �������� Guid � �����
      //"", "", "", "", ""
    */
    // �������� ��������� �������
    var youTubeService = serviceScope.ServiceProvider.GetRequiredService<AddFromYouTubeService>();

    // �������� ����� (�������� VIDEO_ID �� �������� ������������� �����)
    foreach (var id1 in videoId1)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id1, categoryId, subCategoryId1); }
}
    /* foreach (var id2 in videoId2)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id2, categoryId, subCategoryId2); }
   foreach (var id3 in videoId3)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id3, categoryId, subCategoryId3); }  
}*/

// ������������ HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// ������������� �������� CORS
app.UseCors("AllowSpecificOrigins");

app.UseSession();

app.UseMiddleware<AuthSessionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
