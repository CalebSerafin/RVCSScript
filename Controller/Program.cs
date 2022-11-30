using Basic.Reference.Assemblies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

string code = $$""""
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    string trustedBase = "https://raw.githubusercontent.com/CalebSerafin/";
    public async Task<string> GetFrostsRawContent(string path) {
        if (!Uri.TryCreate(trustedBase.TrimEnd('/') + '/' + path.TrimStart('/'), UriKind.Absolute, out Uri? uri))
            Console.WriteLine($"Failed to combine base {trustedBase} with relative {path}");
        using HttpClient client = new();
        Console.WriteLine($"Downloading content from {uri}");
        using HttpResponseMessage response = await client.GetAsync(uri);
        Console.WriteLine($"Finished downloading content from {uri}");
        using HttpContent content = response.Content;
        string pageContent = await content.ReadAsStringAsync();
        return pageContent;
    }
    // The script executes like top-functions, so you need to call the method here.
    // RawRepoContentPath is a magic variable passed in by CSharpScript.Create(..., globalsType: typeof(ScriptParams))
    return await GetFrostsRawContent(RawRepoContentPath);
    """";
var script = CSharpScript.Create(code, ScriptOptions.Default.AddReferences(Net70.References.All.Select(x => x.FilePath)), globalsType: typeof(ScriptParams), assemblyLoader: null);

ScriptParams model = new(RawRepoContentPath: "/RVCSScript/master/README.md");
var scriptResult = await script.RunAsync(model);
Console.WriteLine(
    $"\n\n========WEBPAGE========\n{
    scriptResult.ReturnValue
    }\n========WEBPAGE========\n");

public record class ScriptParams(string RawRepoContentPath);


