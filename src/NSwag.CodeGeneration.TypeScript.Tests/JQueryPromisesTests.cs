﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class JQueryPromisesTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        public class DiscussionController : Controller
        {
            [HttpPost]
            public void AddMessage([FromBody]Foo message)
            {
            }
        }
        
        public class UrlEncodedRequestConsumingController: Controller
        {
            [HttpPost]
            [Consumes("application/x-www-form-urlencoded")]
            public void AddMessage([FromForm]Foo message, [FromForm]string messageId)
            {
            }
        }

        [Fact]
        public async Task When_export_types_is_true_then_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryPromises,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("export class DiscussionClient", code);
            Assert.Contains("export interface IDiscussionClient", code);
        }

        [Fact]
        public async Task When_export_types_is_false_then_dont_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryPromises,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.DoesNotContain("export class DiscussionClient", code);
            Assert.DoesNotContain("export interface IDiscussionClient", code);
        }
                
        [Fact]
        public async Task When_consumes_is_url_encoded_then_construct_url_encoded_request()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryPromises,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("content_", code);
            Assert.DoesNotContain("FormData", code);
            Assert.Contains("\"Content-Type\": \"application/x-www-form-urlencoded\"", code);
        }
    }
}
