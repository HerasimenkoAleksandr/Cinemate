using cinemate.Data;
using cinemate.Middleware;
using cinemate.Services.DataInitializer;
using cinemate.Services.Hash;
using cinemate.Services.YouTubeService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHashService, Md5HashService>();

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

// Добавление конфигурации из файла
builder.Configuration.AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

SqlConnection connection = new SqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connection, sqlOptions =>
    {
        sqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema: "cinemate");
    }));


// Добавление CORS
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

// Добавление контроллеров с представлениями
builder.Services.AddControllersWithViews();

//builder.Services.AddTransient<AddFromYouTubeService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

    await next();
});
/*using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

    // Преобразуем строку в Guid
    Guid categoryId = new Guid("6FCD873E-ACEE-4CFB-982B-BE425D9FD751");
    //await DataInitializerService.InitializeAsync(context, categoryId);

    Guid subCategoryId1 = new Guid("5934CBF4-91E4-458B-A7E2-FB27561F139E");
    List<String> videoId1= new() { "9_UEYrO1QYk", "67rlpf3RpFk", "53DgKqq9C84", "HQGPDRIVfOE", "wcKkTaP4pcY", "NGFAydiCAKs", "hnV7quaKB-U" };

   Guid subCategoryId2 = new Guid("7CACAB9C-C471-4E13-BE8F-A6F73F7678D7");
    List<String> videoId2 = new() { "MfA5zpfG1fI", "as-6afI4vL8&t", "By4kCIfpjGE", "pVAWKrtJEMo"};
    
    Guid subCategoryId3 = new Guid("0966A6CA-BE90-4AFF-BE04-CF4EFF71236E");
    List<String> videoId3 = new() { "", "", "", "", "" };
    // Передаем Guid в метод
    //"", "", "", "", ""

    // Получите экземпляр сервиса
    var youTubeService = serviceScope.ServiceProvider.GetRequiredService<AddFromYouTubeService>();

    // Добавьте фильм (замените VIDEO_ID на реальный идентификатор видео)
     foreach (var id1 in videoId1)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id1, categoryId, subCategoryId1); }
     foreach (var id2 in videoId2)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id2, categoryId, subCategoryId2); }
   foreach (var id3 in videoId3)
    { await AddFromYouTubeService.GetMovieFromYouTubeAsync(context, id3, categoryId, subCategoryId3); }

   
}*/

// Конфигурация HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Использование настроек CORS
app.UseCors("AllowSpecificOrigins");

app.UseSession();

app.UseMiddleware<AuthSessionMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
