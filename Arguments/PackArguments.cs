using CommandLine;
using CommandLine.Text;

namespace Ax.Arguments;
[Verb("pack", HelpText = "Create archive")]
public class PackArguments
{
    [Value(0, Required = false, HelpText = "Target folder to pack", Hidden = true, Default = "")]
    public string SourceFolder { get; set; } = "";

    [Option('o', "output", Required = false, HelpText = "Output file name (defaults to axin-dd-MM-yyyy-HH-mm.axz)")]
    public string OutputFile { get; set; } = "";


    [Usage(ApplicationAlias = "ax")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Attempt to find modules folder under %ProgramFiles(x86)\\Marel\\Axin", new PackArguments());
            yield return new Example("With specifying target folder", new PackArguments { SourceFolder = "C:\\Program Files" });
        }
    }
}