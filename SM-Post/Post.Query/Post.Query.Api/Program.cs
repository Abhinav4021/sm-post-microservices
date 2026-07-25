using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repsitories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext = (o
=> o.UseLazyLoadingProxies()
.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

// Create Database and tables from code
//var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
//dataContext.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));

builder.Services.AddScoped<IEventConsumer, EventConsumer>();

// Register QueryHandler Methods
// var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
// var dispacther = new QueryDispatcher();
// dispacther.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
// dispacther.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
// dispacther.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
// dispacther.RegisterHandler<FindPostsWithCommentQuery>(queryHandler.HandleAsync);
// dispacther.RegisterHandler<FindPostsWithLikeQuery>(queryHandler.HandleAsync);
// builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispacther);

builder.Services.AddScoped<IQueryDispatcher<PostEntity>>(sp =>
{
    var queryHandler = sp.GetRequiredService<IQueryHandler>();
    var dispatcher = new QueryDispatcher();
    dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithCommentQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithLikeQuery>(queryHandler.HandleAsync);
    return dispatcher;
});

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Create Database and tables from code
using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

// Console.WriteLine($"Conn: {dataContext.Database.GetDbConnection().ConnectionString}");
// Console.WriteLine($"CanConnect: {dataContext.Database.CanConnect()}");
// var created = dataContext.Database.EnsureCreated();
// Console.WriteLine($"Database created: {created}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();