using CourseService.Services;
using System;
using CourseService.Data;
using Microsoft.EntityFrameworkCore;
using CourseService.RabbitMq;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using CourseService.Models.Quizes;
using CourseService.Repositories.Quizes;
using AutoMapper;

namespace CourseService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);




        //builder.Services.AddScoped<IEntityRepository<Question, Guid>, GenericRepository<Question, Guid>>();
        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<CourseDbContext>();

        builder.Services.AddScoped<IEntityRepository<Feedback, Guid>, GenericRepository<Feedback, Guid>>();
        builder.Services.AddScoped<IEntityRepository<Quiz, Guid>, GenericRepository<Quiz, Guid>>();
        builder.Services.AddScoped<IEntityRepository<QuizResponse, Guid>, GenericRepository<QuizResponse, Guid>>();
        builder.Services.AddScoped<IEntityRepository<Question, Guid>, GenericRepository<Question, Guid>>();
        builder.Services.AddScoped<IEntityRepository<QuestionAnswer, Guid>, GenericRepository<QuestionAnswer, Guid>>();


        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<Feedback, Grpc.Feedback>().ReverseMap();
            cfg.CreateMap<Quiz, Grpc.Quiz>().ReverseMap();
            cfg.CreateMap<QuizResponse, Grpc.QuizResponse>().ReverseMap();
            cfg.CreateMap<Question, Grpc.Question>().ReverseMap();
            cfg.CreateMap<QuestionAnswer, Grpc.QuestionAnswer>();
            cfg.CreateMap<Grpc.QuestionAnswer, QuestionAnswer>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            /*ConstructUsing(src => new QuestionAnswer(src.Id, src.QuestionId, src.QuizResponseId, src.AnswerText, src.SelectedOptions)).IgnoreAllPropertiesWithAnInaccessibleSetter();*/


        });


        builder.Services.AddScoped<QuizService<Quiz, Guid, Grpc.Quiz>>();
        builder.Services.AddScoped<QuizResponseService<QuizResponse, Guid, Grpc.QuizResponse>>();
        builder.Services.AddScoped<QuestionService<Question, Guid, Grpc.Question>>();
        builder.Services.AddScoped<QuestionAnswerService<QuestionAnswer, Guid, Grpc.QuestionAnswer>>();
        builder.Services.AddScoped<FeedbackService<Feedback, Guid, Grpc.Feedback>>();

/*        builder.Services.AddScoped<QuizService>();
        builder.Services.AddScoped<QuizResponceService>();
        builder.Services.AddScoped<QuestionService>();
        builder.Services.AddScoped<QuestionAnswerService>();
        builder.Services.AddScoped<FeedbackService>();*/

        builder.Services.AddScoped<Services.CourseService>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
/*            .WriteTo.File("logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)*/
        .CreateLogger();
        builder.Host.UseSerilog();

/*        builder.Services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("CourseService"))
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(opt =>
                    {
                        opt.Endpoint = new Uri("http://localhost:14268/api/traces");
                        opt.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift;
                    });
            });*/


/*        builder.Services.AddHostedService<RabbitMqConsumerService>(provider =>
        {
            return new RabbitMqConsumerService(provider.GetRequiredService<IServiceScopeFactory>(),queue: "CoursesQueue");
        });*/

        var app = builder.Build();

/*        app.MapGrpcService<GenericCrudService<Feedback, Guid, Grpc.Feedback>>();
        app.MapGrpcService<GenericCrudService<Quiz, Guid, Grpc.Quiz>>();
        app.MapGrpcService<GenericCrudService<QuizResponse, Guid, Grpc.QuizResponse>>();
        app.MapGrpcService<GenericCrudService<Question, Guid, Grpc.Question>>();
        app.MapGrpcService<GenericCrudService<QuestionAnswer, Guid, Grpc.QuestionAnswer>>();*/

/*        app.MapGrpcService<QuizService>();
        app.MapGrpcService<QuizResponceService>();
        app.MapGrpcService<QuestionService>();
        app.MapGrpcService<QuestionAnswerService>();
        app.MapGrpcService<FeedbackService>();*/

        app.MapGrpcService<QuizService<Quiz, Guid, Grpc.Quiz>>();
        app.MapGrpcService<QuizResponseService<QuizResponse, Guid, Grpc.QuizResponse>>();
        app.MapGrpcService<QuestionService<Question, Guid, Grpc.Question>>();
        app.MapGrpcService<QuestionAnswerService<QuestionAnswer, Guid, Grpc.QuestionAnswer>>();
        app.MapGrpcService<FeedbackService<Feedback, Guid, Grpc.Feedback>>();


        app.MapGrpcService<Services.CourseService>();


        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }


}