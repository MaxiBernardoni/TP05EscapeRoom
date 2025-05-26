var builder = WebApplication.CreateBuilder(args);

// Servicios habilitados
builder.Services.AddSession(); 
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Seguridad en HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Habilita acceso a wwwroot (img, css, etc.)

app.UseRouting();

app.UseAuthorization();
app.UseSession(); // Usar sesiones (HttpContext.Session)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

