// using Halomakes.Blog.Models;
// using JavaScriptEngineSwitcher.Core;
// using JavaScriptEngineSwitcher.V8;
//
// namespace Halomakes.Blog.Services;
//
// public class CodeHighlighterService()
// {
//     public string Highlight(string code, CodeLanguage language)
//     {
//         using IJsEngine engine = new V8JsEngine();
//         var langAsString = language.ToString().ToLower();
//
//         engine.ExecuteResource("Halomakes.Blog.JsLib.highlight.js", typeof(Program).Assembly);
//         engine.ExecuteResource($"Halomakes.Blog.JsLib.languages.{langAsString}.min.js", typeof(Program).Assembly);
//
//         engine.SetVariableValue("input", code);
//
//         engine.SetVariableValue("lang", langAsString);
//
//         engine.Execute("highlighted = hljs.highlight(input,{ language: lang' }).value");
//
//         return engine.Evaluate<string>("highlighted");
//     }
// }