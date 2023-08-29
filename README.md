# html-pdf-api

Print your signable PDF starting from an Handlebars' HTML template.

You can place an empty signature simply including this custom tag:

`<signture name="test" reason="Myreason" style="display: inline-block" required>&nbsp;</signature>`

(NB: required attribute is optional)

This tag can be wrapped inside any other div tag in order to give an appropriate dimension.

Optionally you can include a custom html meta tag as following to define page orientation:

`<meta name="orientation" content="landscape" />`

## Installation

This software uses .Net Core 5.0, please install the appropriate SDK before.

Change settings inside HtmlPdfApi/.env file in order to customize url and auth params

To build this application digit `dotnet build` inside the project folder **HtmlPdfApi**.

To run this application in **PRODUCTION MODE** simply digit `dotnet run` inside the project folder **HtmlPdfApi**.

To enable **DEVELOPMENT MODE** set an environment variable `ASPNETCORE_ENVIRONMENT=Development`.

If using vscode, you can enable DEVELOPMNET MODE by simply running the debug option `.NET Core Launch` in sidebar menu.

## Run tests

To run unit tests type the following command `dotnet test` from the root folder or inside the project folder **HtmlPdfApi.Tests**.

To run tests with coverage run `dotnet test --collect:"XPlat Code Coverage"`
