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

app.Run();
