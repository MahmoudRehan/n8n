using Microsoft.EntityFrameworkCore;
using n8n.Data;
using n8n.Services;

namespace n8n
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IAiService, AiService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Chat}/{action=Index}/{id?}")
                .WithStaticAssets();

            await EnsureDatabaseCreatedAsync(app);

            await app.RunAsync();
        }

        private static async Task EnsureDatabaseCreatedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
