using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using CommunicationThreadTagExtraction.Utils;

namespace CommunicationThreadTagExtraction
{
    public class AnalyzeEmailForTags
    {
        private readonly ILogger<AnalyzeEmailForTags> _logger;

        public AnalyzeEmailForTags(ILogger<AnalyzeEmailForTags> log)
        {
            _logger = log;
        }

        [FunctionName("AnalyzeEmailForTags")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "email_body", "tag_start_marker", "replace_underscores_with_spaces", "replace_dashes_with_spaces", "capitalize_separate_words" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "email_body", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The body of the email_body to be analyzed. This can be raw HTML or plain text.")]
        [OpenApiParameter(name: "tag_start_marker", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The character sequence that should mark a tag.")]
        [OpenApiParameter(name: "replace_underscores_with_spaces", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Description = "If true underscores in tags will be replaced with spaces.")]
        [OpenApiParameter(name: "replace_dashes_with_spaces", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Description = "If true dashes in tags will be replaced with spaces.")]
        [OpenApiParameter(name: "capitalize_separate_words", In = ParameterLocation.Query, Required = false, Type = typeof(bool), Description = "If true the first letter of each word in the tag will be capitalized.")]        
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // string emailBody = req.Query["email_body"];
            // string tagStartMarker = req.Query["tag_start_marker"];
            // Boolean replaceUnderscoresWithSpaces = Boolean.Parse(req.Query["replace_underscores_with_spaces"]);
            // Boolean replaceDashesWithSpaces = Boolean.Parse(req.Query["replace_dashes_with_spaces"]);
            // Boolean capitalizeSeparateWords = Boolean.Parse(req.Query["capitalize_separate_words"]);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<AnalyzeEmailForTagsRequest>(requestBody);
            // string emailBody = data.email_body;
            // string tagStartMarker = data.tag_start_marker;
            // bool replaceUnderscoresWithSpaces = data.replace_underscores_with_spaces;
            // bool replaceDashesWithSpaces = data.replace_dashes_with_spaces;
            // bool capitalizeSeparateWords = data.capitalize_separate_words;

            var emailBodyText = HtmlUtilities.ConvertToPlainText(data.email_body);

            //Break the email body into individual emails if it is threaded
            var emails = ParseEmailUtilities.ParseEmailText(emailBodyText);

            //Parse the tags from the first email
            if (emails.Count > 0)
            {
                var tags = ParseEmailUtilities.ParseTagsFromEmailText(emails[0], data.tag_start_marker, data.replace_underscores_with_spaces, data.replace_dashes_with_spaces, data.capitalize_separate_words);
                //convert the tags to a json object
                return new OkObjectResult(new { tags = tags });
            }
            else
            {
                //Return an empty array if no tags were found
                return new OkObjectResult(new { tags = new string[] { } });
            }
        }
    }

    class AnalyzeEmailForTagsRequest
    {
        public string email_body { get; set; }
        public string tag_start_marker { get; set; }
        public bool replace_underscores_with_spaces { get; set; }
        public bool replace_dashes_with_spaces { get; set; }
        public bool capitalize_separate_words { get; set; }
    }
}

