var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Criar nossa lista
var Cavalos = new List<CavalosDto>
{
    new CavalosDto(1, "Meisho Doto"),
    new CavalosDto(2, "TM Opera O")
};


app.MapGet("/", () => "API da corrida de cavalos está no ar");

app.MapGet("/api/Corridas", () =>
{
    return Results.Ok(Cavalos);
});

app.MapGet("/api/Cavalos/{id:int}", (int id) =>
{
    var Corridas = Cavalos.Find(x =>x.Id == id);

    if (Corridas is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(Corridas);
});

app.MapPost("/api/Cavalos", (CavalosEntradaDto dados) =>
{
    int proximoId = Cavalos.Count + 1;

    var novoCavalo = new CavalosDto(proximoId, dados.Corredor);

    Cavalos.Add(novoCavalo);

    return Results.Created($"/api/Cavalos/{novoCavalo.Id}", novoCavalo);

});


app.MapPut("/api/Cavalos/{id:int}", (int id, CavalosEntradaDto dados) => 
{
    int indice = Cavalos.FindIndex(ListaDeCavalos => ListaDeCavalos.Id == id);
    if(indice == -1)
    {
        return Results.NotFound();
    }

    var CavaloAtualizado = new CavalosDto(id, dados.Corredor);

    Cavalos[indice] = CavaloAtualizado;

    return Results.Ok(CavaloAtualizado);
});

app.MapDelete("/api/Corridas/{id:int}", (int id) => 
{
    int indice = Cavalos.FindIndex(x => x.Id == id);
    if(indice == -1)
    {
        return Results.NotFound();
    }
    Cavalos.RemoveAt(indice);
    return Results.NoContent();
});

app.Run();


record CavalosDto(int Id, string Corredor);
record CavalosEntradaDto(string Corredor);
