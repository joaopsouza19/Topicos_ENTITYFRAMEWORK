using Microsoft.EntityFrameworkCore;
using Loja.data;
using Loja.models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure a conexão com o BD
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 37))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/createproduto", async(LojaDbContext dbContext, Produto newProduto)=>
{
    dbContext.Produtos.Add(newProduto);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createproduto/{newProduto.Id}", newProduto);
});

app.MapGet("/produtos", async (LojaDbContext dbContext)=>
{
    var produtos = await dbContext.Produtos.ToListAsync();
    return Results.Ok(produtos);
}
);

app.MapGet("/produtos/{Id}", async (int Id, LojaDbContext dbContext)=>
{
    var produto = await dbContext.Produtos.FindAsync(Id);
    if(produto == null){
        return Results.NotFound($"Produto de ID {Id} nao encontrado");
    }

    return Results.Ok(produto);
}
);

app.MapPut("/produtos/{Id}", async (int Id, LojaDbContext dbContext, Produto updateProduto)=>
{
    var existingProduto = await dbContext.Produtos.FindAsync(Id);
    if(existingProduto == null){
        return Results.NotFound($"Produto de ID {Id} nao encontrado");
    }

    existingProduto.Nome = updateProduto.Nome;
    existingProduto.Preco = updateProduto.Preco;
    existingProduto.Fornecedor = updateProduto.Fornecedor;

    await dbContext.SaveChangesAsync();

    return Results.Ok(existingProduto);
}
);

//Parte Cliente

app.MapPost("/createcliente", async (LojaDbContext dbContext, Cliente newCliente)=>
{
    dbContext.Clientes.Add(newCliente);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createcliente/{newCliente.Id}", newCliente);
});

app.MapGet("/clientes", async (LojaDbContext dbContext)=>
{
    var clientes = await dbContext.Clientes.ToListAsync();
    return Results.Ok(clientes);
}
);

app.MapGet("/clientes/{Id}", async (int Id, LojaDbContext dbContext)=>
{
    var cliente = await dbContext.Clientes.FindAsync(Id);
    if(cliente == null){
        return Results.NotFound($"Cliente de ID {Id} nao encontrado");
    }

    return Results.Ok(cliente);
}
);

app.MapPut("/clientes/{Id}", async (int Id, LojaDbContext dbContext, Cliente updateCliente)=>
{
    var existingCliente = await dbContext.Clientes.FindAsync(Id);
    if(existingCliente == null){
        return Results.NotFound($"Cleintes de ID {Id} nao encontrado");
    }

    existingCliente.Nome = updateCliente.Nome;
    existingCliente.CPF = updateCliente.CPF;
    existingCliente.Email = updateCliente.Email;

    await dbContext.SaveChangesAsync();

    return Results.Ok(existingCliente);
});

//Parte Fornecedores

app.MapPost("/createfornecedor", async (LojaDbContext dbContext, Fornecedor newFornecedor) =>
{
    dbContext.Fornecedores.Add(newFornecedor);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/createfornecedor/{newFornecedor.Id}", newFornecedor);
});

app.MapGet("/fornecedores", async (LojaDbContext dbContext) =>
{
    var fornecedores = await dbContext.Fornecedores.ToListAsync();
    return Results.Ok(fornecedores);
});

app.MapGet("/fornecedores/{Id}", async (int Id, LojaDbContext dbContext) =>
{
    var fornecedor = await dbContext.Fornecedores.FindAsync(Id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor de ID {Id} não encontrado");
    }

    return Results.Ok(fornecedor);
});

app.MapPut("/fornecedores/{Id}", async (int Id, LojaDbContext dbContext, Fornecedor updateFornecedor) =>
{
    var existingFornecedor = await dbContext.Fornecedores.FindAsync(Id);
    if (existingFornecedor == null)
    {
        return Results.NotFound($"Fornecedor de ID {Id} não encontrado");
    }

    existingFornecedor.Nome = updateFornecedor.Nome;
    existingFornecedor.CNPJ = updateFornecedor.CNPJ;
    existingFornecedor.Endereco = updateFornecedor.Endereco;
    existingFornecedor.Email = updateFornecedor.Email;
    existingFornecedor.Telefone = updateFornecedor.Telefone;

    await dbContext.SaveChangesAsync();

    return Results.Ok(existingFornecedor);
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}