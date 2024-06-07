using System.Text;
using System.Text.Json;
using ProjektPaletaRGB;
using ColorHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7294")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Colour currentColour = null;
string hexColour = null;
string complementaryColour = null;
string[] analogousColours = new string[2];
string[] triadicColours = new string[2];
string[] tetradicColours = new string[3];
string[] monochromaticColours = new string[3];
string[] palette = new string[12];
string fileName = null;
string filePath = null;

app.MapPost("/api/colours", async (Colour data, HttpContext context) =>
{
    if (data == null)
    {
        await context.Response.WriteAsync("Niepoprawne dane"); // Invalid data
        return;
    }

    currentColour = data;
    hexColour = ConvertToHex(currentColour.Red, currentColour.Green, currentColour.Blue);
    complementaryColour = FindComplementaryColour(currentColour.Red, currentColour.Green, currentColour.Blue);
    analogousColours = FindAnalogousColours(currentColour.Red, currentColour.Green, currentColour.Blue);
    triadicColours = FindTriadicColours(currentColour.Red, currentColour.Green, currentColour.Blue);
    tetradicColours = FindTetradicColours(currentColour.Red, currentColour.Green, currentColour.Blue);
    monochromaticColours = FindMonochromaticColours(currentColour.Red, currentColour.Green, currentColour.Blue);
    
    palette[0] = hexColour;
    palette[1] = complementaryColour;
    palette[2] = analogousColours[0];
    palette[3] = analogousColours[1];
    palette[4] = triadicColours[0];
    palette[5] = triadicColours[1];
    palette[6] = tetradicColours[0];
    palette[7] = tetradicColours[1];
    palette[8] = tetradicColours[2];
    palette[9] = monochromaticColours[0];
    palette[10] = monochromaticColours[1];
    palette[11] = monochromaticColours[2];
    
    fileName = "paleta" + DateTime.Now.ToString("--MM-dd-yyyy--HH-mm-ss") + ".jpg";
    filePath = Path.Combine(
       Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
      fileName);
    PaletteSaving.SavePaletteImage(palette, filePath);

    await context.Response.WriteAsync("Sukces: dane otrzymane i przetworzone"); // Data received and transformed successfully.
});

app.MapGet("/api/hex-colours", () =>
{
    if (hexColour == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat kodu heksadecymalnego koloru" }); // No hexadecimal colour data available
    }

    return Results.Ok(hexColour);
});

app.MapGet("/api/complementary-colours", () =>
{
    if (complementaryColour == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat koloru komplementarnego" }); // No complementary colours data available
    }

    return Results.Ok(complementaryColour);
});

app.MapGet("/api/analogous-colours", () =>
{
    if (analogousColours[0] == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat kolorów analogicznych" }); // No analogous colorus data available
    }

    return Results.Ok(analogousColours);
});

app.MapGet("/api/triadic-colours", () =>
{
    if (triadicColours[0] == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat triady kolorów" }); // No triadic colours data available
    }

    return Results.Ok(triadicColours);
});

app.MapGet("/api/tetradic-colours", () =>
{
    if (tetradicColours[0] == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat tetrady kolorów" }); // No tetradic colours data available
    }

    return Results.Ok(tetradicColours);
});

app.MapGet("/api/monochromatic-colours", () =>
{
    if (monochromaticColours[0] == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat kolorów monochromatycznych" }); // No monochromatic colours data available
    }

    return Results.Ok(monochromaticColours);
});


app.MapGet("/api/colours", () =>
{
    if (currentColour == null)
    {
        return Results.NotFound(new { message = "Brak danych na temat koloru" }); // No color data available
    }
    return Results.Ok("Red: " + currentColour.Red + " Green: " + currentColour.Green + " Blue: " + currentColour.Blue + " Hex: " + ConvertToHex(currentColour.Red, currentColour.Green, currentColour.Blue));

});

app.UseHttpsRedirection();

app.Run();

string ConvertToHex(byte r, byte g, byte b)
{
    string hex = String.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
    return hex;
}

string FindComplementaryColour(byte r, byte g, byte b) // +180 degrees
{
    HSV baseColour = ColorConverter.RgbToHsv(new RGB(r, g, b));
    baseColour.H = (baseColour.H + 180) % 360;

    RGB complementaryRGB = ColorConverter.HsvToRgb(baseColour);
    string complementaryColour = ConvertToHex(complementaryRGB.R, complementaryRGB.G, complementaryRGB.B);

    return complementaryColour;
}

string[] FindAnalogousColours(byte r, byte g, byte b) // +30 degrees, -30 degrees
{
    HSV baseColour = ColorConverter.RgbToHsv(new RGB(r, g, b));
    HSV analogousColourA = new HSV((baseColour.H + 30) % 360, baseColour.S, baseColour.V);
    HSV analogousColourB = new HSV((baseColour.H - 30) % 360, baseColour.S, baseColour.V);

    RGB analogousARGB = ColorConverter.HsvToRgb(analogousColourA);
    RGB analogousBRGB = ColorConverter.HsvToRgb(analogousColourB);

    string[] analogousColours = { ConvertToHex(analogousARGB.R, analogousARGB.G, analogousARGB.B), ConvertToHex(analogousBRGB.R, analogousBRGB.G, analogousBRGB.B) };

    return analogousColours;
}

string[] FindTriadicColours(byte r, byte g, byte b) // +120 degrees, +240 degrees
{
    HSV baseColour = ColorConverter.RgbToHsv(new RGB(r, g, b));
    HSV triadicColourA = new HSV((baseColour.H + 120) % 360, baseColour.S, baseColour.V);
    HSV triadicColourB = new HSV((baseColour.H + 240) % 360, baseColour.S, baseColour.V);

    RGB triadicARGB = ColorConverter.HsvToRgb(triadicColourA);
    RGB triadicBRGB = ColorConverter.HsvToRgb(triadicColourB);

    string[] triadicColours = { ConvertToHex(triadicARGB.R, triadicARGB.G, triadicARGB.B), ConvertToHex(triadicBRGB.R, triadicBRGB.G, triadicBRGB.B) };

    return triadicColours;
}

string[] FindTetradicColours(byte r, byte g, byte b) // +60 degrees, +180 degrees, +240 degrees
{
    HSV baseColour = ColorConverter.RgbToHsv(new RGB(r, g, b));
    HSV tetradicColourA = new HSV((baseColour.H + 60) % 360, baseColour.S, baseColour.V);
    HSV tetradicColourB = new HSV((baseColour.H + 180) % 360, baseColour.S, baseColour.V);
    HSV tetradicColourC = new HSV((baseColour.H + 240) % 360, baseColour.S, baseColour.V);

    RGB tetradicARGB = ColorConverter.HsvToRgb(tetradicColourA);
    RGB tetradicBRGB = ColorConverter.HsvToRgb(tetradicColourB);
    RGB tetradicCRGB = ColorConverter.HsvToRgb(tetradicColourC);

    string[] tetradicColours = { ConvertToHex(tetradicARGB.R, tetradicARGB.G, tetradicARGB.B), ConvertToHex(tetradicBRGB.R, tetradicBRGB.G, tetradicBRGB.B), ConvertToHex(tetradicCRGB.R, tetradicCRGB.G, tetradicCRGB.B) };

    return tetradicColours;
}

string[] FindMonochromaticColours(byte r, byte g, byte b) // 75% of saturation, 50% of saturation, 25% of saturation
{
    HSV baseColour = ColorConverter.RgbToHsv(new RGB(r, g, b));
    HSV monochromaticColourA = new HSV(baseColour.H, (byte)((baseColour.S / 4) * 3), baseColour.V);
    HSV monochromaticColourB = new HSV(baseColour.H, (byte)(baseColour.S / 2), baseColour.V);
    HSV monochromaticColourC = new HSV(baseColour.H, (byte)(baseColour.S / 4), baseColour.V);

    RGB monochromaticARGB = ColorConverter.HsvToRgb(monochromaticColourA);
    RGB monochromaticBRGB = ColorConverter.HsvToRgb(monochromaticColourB);
    RGB monochromaticCRGB = ColorConverter.HsvToRgb(monochromaticColourC);

    string[] monochromaticColours = { ConvertToHex(monochromaticARGB.R, monochromaticARGB.G, monochromaticARGB.B), ConvertToHex(monochromaticBRGB.R, monochromaticBRGB.G, monochromaticBRGB.B), ConvertToHex(monochromaticCRGB.R, monochromaticCRGB.G, monochromaticCRGB.B) };

    return monochromaticColours;
}