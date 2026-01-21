using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json;

namespace GymDogs.Presentation.Middleware;

/// <summary>
/// Middleware que adiciona o esquema de segurança JWT Bearer ao documento OpenAPI
/// </summary>
public class OpenApiSecurityMiddleware
{
    private readonly RequestDelegate _next;

    public OpenApiSecurityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Intercepta apenas requisições ao endpoint OpenAPI
        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            // Chama o próximo middleware para gerar o documento
            var originalBodyStream = context.Response.Body;
            
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Se a resposta foi bem-sucedida e é JSON
            if (context.Response.StatusCode == 200 && 
                context.Response.ContentType?.Contains("application/json") == true)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var jsonDocument = await JsonDocument.ParseAsync(responseBody);
                
                // Modifica o JSON para adicionar securitySchemes e security
                var root = jsonDocument.RootElement;
                var modifiedJson = AddSecurityToOpenApiJson(root);

                // Escreve o JSON modificado na resposta
                context.Response.Body = originalBodyStream;
                context.Response.ContentLength = null;
                await context.Response.WriteAsync(modifiedJson);
                return;
            }

            // Se não modificou, copia a resposta original
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
            return;
        }

        await _next(context);
    }

    private string AddSecurityToOpenApiJson(JsonElement root)
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();

        // Copia todas as propriedades existentes
        foreach (var property in root.EnumerateObject())
        {
            if (property.Name == "components")
            {
                // Modifica components para adicionar securitySchemes
                writer.WritePropertyName("components");
                writer.WriteStartObject();
                
                // Copia schemas se existir
                if (property.Value.TryGetProperty("schemas", out var schemas))
                {
                    writer.WritePropertyName("schemas");
                    schemas.WriteTo(writer);
                }

                // Adiciona securitySchemes
                writer.WritePropertyName("securitySchemes");
                writer.WriteStartObject();
                writer.WritePropertyName("Bearer");
                writer.WriteStartObject();
                writer.WriteString("type", "http");
                writer.WriteString("scheme", "bearer");
                writer.WriteString("bearerFormat", "JWT");
                writer.WriteString("description", "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"");
                writer.WriteString("name", "Authorization");
                writer.WriteString("in", "header");
                writer.WriteEndObject();
                writer.WriteEndObject();
                
                writer.WriteEndObject();
            }
            else
            {
                // Copia outras propriedades (incluindo paths) normalmente
                property.WriteTo(writer);
            }
        }

        // Adiciona security global após components
        writer.WritePropertyName("security");
        writer.WriteStartArray();
        writer.WriteStartObject();
        writer.WritePropertyName("Bearer");
        writer.WriteStartArray();
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.WriteEndArray();

        writer.WriteEndObject();
        writer.Flush();

        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
