using System;
using HtmlPdfApi.Helpers.Handlebars;
using Xunit;

namespace HtmlPdfApi.Tests.Helpers.Handlebars
{
    public class HandlebarsHelpersTests
    {
        [Fact]
        public void IfCondition_ReturnsCorrectValues_WhenMatches()
        {
            HandlebarsHelpers.RegisterHelper_IfCond();

            var template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond arg1 '>' arg2}}{{arg1}} is greater than {{arg2}}{{else}}{{arg1}} is less than {{arg2}}{{/ifCond}}");
            var data = new { arg1 = 2, arg2 = 1 };
            var result = template(data);
            Assert.Equal("2 is greater than 1", result);

            data = new { arg1 = 1, arg2 = 2 };
            result = template(data);
            Assert.Equal("1 is less than 2", result);

            template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond arg1 '<' arg2}}{{arg1}} is less than {{arg2}}{{else}}{{arg1}} is greater than {{arg2}}{{/ifCond}}");
            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is greater than 1", result);

            data = new { arg1 = 1, arg2 = 2 };
            result = template(data);
            Assert.Equal("1 is less than 2", result);

            template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond arg1 '=' arg2}}{{arg1}} is eq to {{arg2}}{{else}}{{arg1}} is not eq to {{arg2}}{{/ifCond}}");
            data = new { arg1 = 1, arg2 = 1 };
            result = template(data);
            Assert.Equal("1 is eq to 1", result);

            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is not eq to 1", result);

            template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond arg1 '!=' arg2}}{{arg1}} is not eq to {{arg2}}{{else}}{{arg1}} is eq to {{arg2}}{{/ifCond}}");
            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is not eq to 1", result);

            template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond str '!=' ''}}not empty{{else}}empty{{/ifCond}}");
            var datastr = new { str = "abc" };
            result = template(datastr);
            Assert.Equal("not empty", result);

            template = HandlebarsDotNet.Handlebars.Compile(@"{{#ifCond str '==' ''}}empty{{else}}not empty{{/ifCond}}");
            datastr = new { str = "" };
            result = template(datastr);
            Assert.Equal("empty", result);
        }

        [Fact]
        public void FormatDateTime_ReturnsCorrectValues_WhenMatches()
        {
            HandlebarsHelpers.RegisterHelper_DateTimeFormatter();

            var template = HandlebarsDotNet.Handlebars.Compile(@"{{formatDateTime arg1 'dd/MM/yyyy'}}");
            var data = new { arg1 = "2021-12-02T09:00:00.0000000Z" };
            var result = template(data);
            Console.WriteLine(result);
            Assert.Equal("02/12/2021", result);
        }
    }
}