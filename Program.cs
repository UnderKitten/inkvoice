using Inkvoice.Components;
using Inkvoice.Data;
using Inkvoice.Models;
using Inkvoice.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=invoices.db")
);

var app = builder.Build();

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

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapGet(
    "/documents/{id:int}/pdf",
    async (int id, IDbContextFactory<AppDbContext> factory) =>
    {
        using var db = await factory.CreateDbContextAsync();
        var doc = await db
            .Documents.Include(d => d.Items)
            .Include(d => d.Client)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doc is null)
            return Results.NotFound();

        var biz = await db.Businesses.FirstOrDefaultAsync() ?? new Business();
        var bytes = InvoicePdf.Generate(doc, biz);
        var fileName = $"{doc.Type}_{doc.Number}.pdf".Replace(" ", "_");
        return Results.File(bytes, "application/pdf", fileName);
    }
);

app.Run();
