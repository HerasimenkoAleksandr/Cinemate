using cinemate.Data;
using cinemate.Middleware;
using cinemate.Services.DataInitializer;
using cinemate.Services.Hash;
using cinemate.Services.YouTubeService;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHashService, Md5HashService>();

builder.Services.AddDistributedMemoryCache();



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
            builder.WithOrigins("http://frontend.cinemate.pp.ua")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});




// ���������� ������������ � ���������������
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<AddFromYouTubeService>();


var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

    // ����������� ������ � Guid
    Guid categoryId = new Guid("D236F550-55FF-4377-BE70-70625F224043");
    Guid subCategoryId = new Guid("90F6BDAB-C59D-42FE-979E-0280DB629B38");
    List<String> videoId = new() { "A-5OpVpJTVM", "EfouvrxyuOQ" };
    // �������� Guid � �����
    await DataInitializerService.InitializeAsync(context, categoryId);

    // �������� ��������� �������
    //var youTubeService = serviceScope.ServiceProvider.GetRequiredService<AddFromYouTubeService>();

    // �������� ����� (�������� VIDEO_ID �� �������� ������������� �����)
    foreach (var id in videoId)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id, categoryId, subCategoryId); }

   
}



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

app.UseAuthorization();

app.UseSession();

app.UseMiddleware<AuthSessionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
