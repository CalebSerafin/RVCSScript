using Basic.Reference.Assemblies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

string code = $$""""
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    public async Task<string> GetFrostsRawContent(string path) {
        Uri uri = new UriBuilder("https://raw.githubusercontent.com/") { Path = path }.Uri;
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(uri);
        using HttpContent content = response.Content;
        string pageContent = await content.ReadAsStringAsync();
        return pageContent;
    }
    // The script executes like top-functions, so you need to call the method here.
    // FrostPath is a magic variable passed in by CSharpScript.Create(..., globalsType: typeof(ScriptParams))
    return await GetFrostsRawContent(FrostPath);
    """";
var script = CSharpScript.Create(code, ScriptOptions.Default.AddReferences(Net60.References.All.Select(x => x.FilePath)), globalsType: typeof(ScriptParams), assemblyLoader: null);

ScriptParams model = new(FrostPath: "official-antistasi-community/A3-Antistasi/master/Tools/StreetArtist/Please%20Read%20Me.md");
var scriptResult = await script.RunAsync(model);
Console.WriteLine(scriptResult.ReturnValue);

public record class ScriptParams(string FrostPath);


