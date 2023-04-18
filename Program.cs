using WhatsappAPI.Ioc;

namespace WhatsappAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.NativeInjector();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));
            //builder.Services.AddBrowserInitialization();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("corsapp");
            app.UseRouting();


            app.MapControllers();
            //app.UseBrowserInitialization();
            /*var logged = false;
            Console.WriteLine("Initializing client...");
            while (!logged) { logged = AuthHelper.getInstance().startProfile().Result; }*/

            //TODO: refactor this Helper as a Middleware.

            app.Run();
        }
    }
}