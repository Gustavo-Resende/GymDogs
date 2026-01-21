using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GymDogs.Presentation.Configuration;

/// <summary>
/// Transformer customizado para adicionar o esquema de segurança JWT Bearer ao documento OpenAPI
/// </summary>
public class JwtBearerOpenApiTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        const string schemeId = "Bearer";

        // Garante que Components existe
        if (document.Components == null)
        {
            document.Components = new OpenApiComponents();
        }

        // Garante que SecuritySchemes existe
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        // Cria o esquema de segurança JWT Bearer
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header
        };

        // Adiciona o esquema aos componentes do documento
        document.Components.SecuritySchemes[schemeId] = bearerScheme;

        // Cria a referência usando OpenApiSecuritySchemeReference (nova API do .NET 10)
        var schemeReference = new OpenApiSecuritySchemeReference(schemeId, document);
        
        // Cria o requisito de segurança
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [schemeReference] = new List<string>()
        };

        // Adiciona o requisito de segurança global ao documento
        if (document.Security == null)
        {
            document.Security = new List<OpenApiSecurityRequirement>();
        }
        
        document.Security.Add(securityRequirement);

        // Aplica o requisito de segurança em todas as operações
        // O Scalar detecta automaticamente endpoints com [AllowAnonymous] e não exige token neles
        // Mas ainda mostra o botão de autenticação quando há um security scheme definido
        foreach (var pathItem in document.Paths.Values)
        {
            if (pathItem.Operations == null) continue;
            
            foreach (var operation in pathItem.Operations.Values)
            {
                // Inicializa Security se não existir
                operation.Security ??= new List<OpenApiSecurityRequirement>();
                
                // Adiciona o requisito de segurança à operação
                // Endpoints com [AllowAnonymous] terão isso sobrescrito pelo ASP.NET Core
                operation.Security.Add(securityRequirement);
            }
        }

        return Task.CompletedTask;
    }
}