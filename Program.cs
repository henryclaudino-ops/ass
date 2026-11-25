
using Corridadecavalos.Data;
using Corridadecavalos.Dtos;
using Corridadecavalos.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CavalosDbContext>(options =>
    options.UseSqlite("Data Source=cavalos.db"));

var app = builder.Build();

// Tratamento de exceções
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            erro = "Erro interno no servidor."
        });
    });
});

// Criar o banco de dados caso não exista
using (var scope = app.Services.CreateScope())
{
    var banco = scope.ServiceProvider
        .GetRequiredService<CavalosDbContext>();

    await banco.Database.EnsureCreatedAsync();
}

// Página inicial
app.MapGet("/", () => "API da corrida de cavalos está no ar");

// Listar todos os cavalos
app.MapGet("/api/Cavalos", async (CavalosDbContext banco) =>
{
    var Cavalos = await banco.Cavalos.ToListAsync();

    return Results.Ok(Cavalos);
});

// Buscar cavalo pelo ID
app.MapGet("/api/Cavalos/{id:int}", async (int id, CavalosDbContext banco) =>
{
    var Corridas = await banco.Cavalos.FindAsync(id);

    if (Corridas is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(Corridas);
});

// Cadastrar um cavalo
app.MapPost("/api/Cavalos", async (CavalosEntradaDto dados, CavalosDbContext banco) =>
{
    if (string.IsNullOrWhiteSpace(dados.Corredor))
    {
        return Results.BadRequest("O corredor é obrigatório.");
    }

    var novoCavalo = new Cavalo
    {
        Corredor = dados.Corredor
    };

    banco.Cavalos.Add(novoCavalo);

    await banco.SaveChangesAsync();

    return Results.Created($"/api/Cavalos/{novoCavalo.Id}", novoCavalo);
});

// Atualizar um cavalo
app.MapPut("/api/Cavalos/{id:int}", async (int id, CavalosEntradaDto dados, CavalosDbContext banco) =>
{
    var CavaloAtualizado = await banco.Cavalos.FindAsync(id);

    if (CavaloAtualizado is null)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(dados.Corredor))
    {
        return Results.BadRequest("O corredor é obrigatório.");
    }

    CavaloAtualizado.Corredor = dados.Corredor;

    await banco.SaveChangesAsync();

    return Results.Ok(CavaloAtualizado);
});

// Excluir um cavalo
app.MapDelete("/api/Cavalos/{id:int}", async (int id, CavalosDbContext banco) =>
{
    var Cavalo = await banco.Cavalos.FindAsync(id);

    if (Cavalo is null)
    {
        return Results.NotFound();
    }

    banco.Cavalos.Remove(Cavalo);

    await banco.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();