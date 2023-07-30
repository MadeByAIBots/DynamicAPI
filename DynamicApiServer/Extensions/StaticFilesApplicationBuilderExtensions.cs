using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public static class StaticFilesApplicationBuilderExtensions
{
    public static void UseInjectableStaticFiles(this IApplicationBuilder app)
    {
        var handler = app.ApplicationServices.GetRequiredService<InjectableStaticFileHandler>();

        app.UseRouting();
        
        app.Use(async (context, next) =>
        {
            await handler.HandleRequest(context);
            if (!context.Response.HasStarted)
            {
                await next();
            }
        });
    }
}