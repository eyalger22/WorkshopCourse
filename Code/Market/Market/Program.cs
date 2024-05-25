using Market.ORM;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Market.DomainLayer.Hubs;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.AspNet.SignalR;
//using SignalRAuthenticationSample.Data;
//using SignalRAuthenticationSample.Hubs;
using var db = new MarketContext();
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Services.AddControllersWithViews();

//builder.Services.AddRazorPages();
builder.Services.AddSignalR();


//builder.Services.AddSqlite<MarketContext>("Data Source=database.db");
//builder.Services.AddDbContext<MarketContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MarketContext")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapSignalR("/chatHub", new HubConfiguration());
//app.MapRazorPages();
app.MapHub<Market.DomainLayer.Hubs.ChatHub>("/chatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{mode?}/{shop?}/{product?}");



app.Use(async (context, next) =>
{
    var hubContext = context.RequestServices
                            .GetRequiredService<IHubContext<ChatHub>>();
    ChatController chat = new ChatController(hubContext);
    AlertsManager.chatController = chat;

    if (next != null)
    {
        await next.Invoke();
    }
});

app.Run();