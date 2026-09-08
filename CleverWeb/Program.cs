using CleverWeb.Data;
using CleverWeb.Features.Auth.Services;
using CleverWeb.Features.Caixa.Services;
using CleverWeb.Features.Contribuicao.Services;
using CleverWeb.Features.Despesa.Services;
using CleverWeb.Features.Membro.Validators;
using CleverWeb.Features.Users.Services;
using CleverWeb.Infrastructure.ViewLocation;
using CleverWeb.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var cultureInfo = new CultureInfo("pt-BR");
cultureInfo.NumberFormat.CurrencySymbol = "R$";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CleverWeb.Infrastructure.Tenant.ITenantAccessor, CleverWeb.Infrastructure.Tenant.TenantAccessor>();

builder.Services.AddDbContext<CleverDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<MembroValidator>();

builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// adicionar session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ContribuicaoService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<CaixaService>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CleverDbContext>();
    db.Database.Migrate();

    if (!db.Tenant.Any())
    {
        var tenantPadrao = new Tenant
        {
            Nome = "Tenant Padrão",
            Slug = "default",
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        db.Tenant.Add(tenantPadrao);
        db.SaveChanges();
    }

    var tenantAtual = db.Tenant.First();

    foreach (var usuario in db.Usuario.Where(u => u.TenantId == 0).ToList())
    {
        usuario.TenantId = tenantAtual.Id;
    }

    if (!db.Usuario.Any())
    {
        db.Usuario.Add(new Usuario
        {
            TenantId = tenantAtual.Id,
            UserName = "admin",
            PasswordHash = AuthService.HashSenha("admin123"),
            Ativo = true
        });
    }

    db.SaveChanges();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();