using FluentValidation;
using RpslsGameService.Application.Behaviors;
using RpslsGameService.Application.Mappings;
using RpslsGameService.Application.UseCases.Commands;
using RpslsGameService.Application.Validators;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Services;
using System.Reflection;

namespace RpslsGameService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "RPSLS Game API", 
                Version = "v1",
                Description = "Rock, Paper, Scissors, Lizard, Spock Game API"
            });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlayGameCommandHandler).Assembly));

        // Register AutoMapper
        services.AddAutoMapper(typeof(GameMappingProfile));

        // Register FluentValidation
        services.AddValidatorsFromAssemblyContaining<PlayGameRequestValidator>();

        // Register Pipeline Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IGameLogicService, GameLogicService>();
        services.AddScoped<IChoiceGenerationService, ChoiceGenerationService>();

        return services;
    }
}