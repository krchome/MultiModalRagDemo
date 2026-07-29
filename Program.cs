using Microsoft.Extensions.Options;
using MultiModalRagDemo.Options;
using MultiModalRagDemo.Services;
using MultiModalRagDemo.Services.Embedding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<QdrantOptions>(
    builder.Configuration.GetSection(QdrantOptions.SectionName));

builder.Services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();

builder.Services.AddScoped<ITextChunkingService, TextChunkingService>();

builder.Services.AddHttpClient<IEmbeddingClient, FastApiEmbeddingClient>(
    (serviceProvider, httpClient) =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var baseUrl = configuration["EmbeddingService:BaseUrl"]
            ?? "http://localhost:8000";

        var timeoutSeconds = configuration.GetValue<int>(
            "EmbeddingService:TimeoutSeconds",
            60);

        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
    });

builder.Services.AddHttpClient<IVectorStoreService, VectorStoreService>(
    (serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<QdrantOptions>>()
            .Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IVectorSearchService, VectorSearchService>(
    (serviceProvider, client) =>
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<QdrantOptions>>()
            .Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection(
        AzureOpenAIOptions.SectionName));

builder.Services.AddSingleton<
    IAnswerGenerationService,
    AzureOpenAIAnswerGenerationService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
//using Microsoft.Extensions.Options;
//using MultiModalRagDemo.Options;
//using MultiModalRagDemo.Services;

//using MultiModalRagDemo.Services.Embedding;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IDocumentTextExtractor, DocumentTextExtractor>();
//builder.Services.AddScoped<ITextChunkingService, TextChunkingService>();
//builder.Services.AddHttpClient<IEmbeddingClient, FastApiEmbeddingClient>(
//    (serviceProvider, httpClient) =>
//    {
//        var configuration =
//            serviceProvider.GetRequiredService<IConfiguration>();

//        var baseUrl = configuration["EmbeddingService:BaseUrl"]
//            ?? "http://localhost:8000";

//        var timeoutSeconds = configuration.GetValue<int>(
//            "EmbeddingService:TimeoutSeconds",
//            60);

//        httpClient.BaseAddress = new Uri(baseUrl);
//        httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
//    });
//builder.Services.AddHttpClient<IVectorStoreService, VectorStoreService>((serviceProvider, client) =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

//    var baseUrl = configuration["Qdrant:BaseUrl"]
//                  ?? throw new InvalidOperationException("Qdrant base URL is missing.");

//    client.BaseAddress = new Uri(baseUrl);
//});
//builder.Services.AddHttpClient<IVectorSearchService, VectorSearchService>((serviceProvider, client) =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

//    var baseUrl = configuration["Qdrant:BaseUrl"]
//                  ?? throw new InvalidOperationException("Qdrant base URL is missing.");

//    client.BaseAddress = new Uri(baseUrl);
//});
//var app = builder.Build();


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseRouting();

//app.UseAuthorization();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();


//app.Run();
