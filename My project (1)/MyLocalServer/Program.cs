using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dodaj usługi do kontenera
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Włącz Swagger w trybie dewelopera
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Włącz obsługę HTTPS
// app.UseHttpsRedirection();

// Zmienna do przechowywania ostatniego wyniku
int lastScore = 0;

// Definiowanie endpointa do odbierania wyników
app.MapPost("/sendscore", (ScoreData scoreData) =>
{
    lastScore = scoreData.Score; // Zapisz ostatni wynik
    Console.WriteLine($"Otrzymano wynik: {lastScore}");
    return Results.Ok(scoreData);
});

// Definiowanie endpointa do pobierania ostatniego wyniku
app.MapGet("/getscore", () =>
{
    return Results.Ok(new ScoreData { Score = lastScore });
});

app.Run();

// Klasa modelu danych
public class ScoreData
{
    public int Score { get; set; }
}

