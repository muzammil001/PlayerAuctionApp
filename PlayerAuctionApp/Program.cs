using Microsoft.EntityFrameworkCore;
using PlayerAuctionApp.Components;
using PlayerAuctionApp.Data;
using PlayerAuctionApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContextFactory<AuctionDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuctionDatabase")));
builder.Services.AddScoped<AuctionTeamService>();
builder.Services.AddScoped<AuctionPlayerService>();
builder.Services.AddScoped<AuctionSaleService>();
builder.Services.AddScoped<AuctionBidService>();
builder.Services.AddScoped<AuctionClockService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AuctionDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
    await AuctionSeeder.SeedAsync(db);
    await AuctionPlayerSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
