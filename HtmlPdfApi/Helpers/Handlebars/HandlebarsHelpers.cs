using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HtmlPdfApi.Helpers.Handlebars
{
    public static class HandlebarsHelpers
    {
        public static void RegisterHelper_IfCond()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("ifCond", (writer, options, context, args) =>
            {
                if (args.Length != 3)
                {
                    writer.Write("ifCond:Wrong number of arguments");
                    return;
                }
                if (args[0] == null || args[0].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[0] undefined");
                    return;
                }
                if (args[1] == null || args[1].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[1] undefined");
                    return;
                }
                if (args[2] == null || args[2].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[2] undefined");
                    return;
                }
                if (args[0].GetType().Name == "String" || args[0].GetType().Name == "JValue")
                {
                    var val1 = args[0].ToString();
                    var val2 = args[2].ToString();

                    switch (args[1].ToString())
                    {
                        case ">":
                            if (val1.Length > val2.Length)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "=":
                        case "==":
                            if (val1 == val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "<":
                            if (val1.Length < val2.Length)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "!=":
                        case "<>":
                            if (val1 != val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                    }
                }
                else
                {
                    var val1 = float.Parse(args[0].ToString());
                    var val2 = float.Parse(args[2].ToString());

                    switch (args[1].ToString())
                    {
                        case ">":
                            if (val1 > val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "=":
                        case "==":
                            if (val1 == val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "<":
                            if (val1 < val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                        case "!=":
                        case "<>":
                            if (val1 != val2)
                            {
                                options.Template(writer, context);
                            }
                            else
                            {
                                options.Inverse(writer, context);
                            }
                            break;
                    }
                }
            });
        }

        public static void RegisterHelper_InArray()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("inArray", (writer, options, context, args) =>
            {
                if (args.Length != 2)
                {
                    writer.Write("ifCond:Wrong number of arguments");
                    return;
                }
                if (args[0] == null || args[0].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[0] undefined");
                    return;
                }
                if (args[1] == null || args[1].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[1] undefined");
                    return;
                }

                try
                {
                    List<string> val1 = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(args[0]));
                    string val2 = args[1].ToString();
                    if (val1.IndexOf(val2) > -1)
                        options.Template(writer, context);
                    else
                        options.Inverse(writer, context);
                }
                catch (Exception ex)
                {
                    options.Inverse(writer, context);
                }
            });
        }

        public static void RegisterHelper_DateTimeFormatter()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("formatDateTime", (output, context, data) =>
                {
                    DateTime.TryParse(data[0].ToString(), out DateTime date);
                    output.Write(date.ToString(data[1].ToString()));
                });
        }
    }
}
