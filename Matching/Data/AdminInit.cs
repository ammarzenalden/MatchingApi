using Matching.Model;

namespace Matching.Data
{
    public static class AdminInit
    {
        public static WebApplication Seed(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    context.Database.EnsureCreated();
                    var users = context.Users.FirstOrDefault(x => x.Email=="admin@gmail.com" && x.Role=="admin");
                    if (users==null)
                    {
                        User user = new()
                        {
                            Email = "admin@gmail.com",
                            Name = "admin",
                            Password = BCrypt.Net.BCrypt.HashPassword("admin12345"),
                            Role = "admin",


                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                }
                catch
                {

                }
                return app;
            }
        }
    }
}
