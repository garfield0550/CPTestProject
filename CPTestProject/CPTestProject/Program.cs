using CPTestProject.Client.Pages;
using CPTestProject.Components;
using CPTestProject.Data;
using CPTestProject.Services.ImportExport;
using CPTestProject.Services.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// for MS SQL use ... change line above to UseSqlServer, update the connection string and run "dotnet add package Microsoft.EntityFrameworkCore.SqlServer" to add the SQL Server provider package. Migrations can be skipped if database is prepared manually.


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add database services
builder.Services.AddScoped<GpsImportService>();
builder.Services.AddScoped<BtsImportService>();
builder.Services.AddScoped<ImportService>();

builder.Services.AddHttpClient(); // Important for Interactive Auto due to @inject HttpClient in ImportDialog.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CPTestProject.Client._Imports).Assembly);

app.MapImportEndpoints(); //Add the import endpoints to the application
app.MapDataEndpoints(); //Add the data endpoints to the application

// Add clear all imported data endpoint. This is a dangerous operation. For preview purposes, it is left unprotected. In production, consider adding authentication and authorization checks.
app.MapPost("/api/map/clear", async (AppDbContext db) =>
{
    await using var transaction = await db.Database.BeginTransactionAsync();

    await db.GpsRecords.ExecuteDeleteAsync();
    await db.BtsRecords.ExecuteDeleteAsync();
    await db.ImportBatches.ExecuteDeleteAsync();

    await transaction.CommitAsync();

    return Results.NoContent();
});

app.Run();
