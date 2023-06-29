using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;

namespace DynamicApiServer.Extensions
{
    public static class StaticFilesApplicationBuilderExtensions
    {
        public static void UseCustomStaticFiles(this IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".yaml"] = "application/yaml";
            provider.Mappings[".yml"] = "application/yaml";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }
    }
}
