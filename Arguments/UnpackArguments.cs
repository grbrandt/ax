using CommandLine;
using CommandLine.Text;

namespace Ax.Arguments;

[Verb("unpack", HelpText = "Extract archive")]
public class UnpackArguments
{
    [Option('o', "output", Required = true, HelpText = "Destination folder")]
    public string OutputFolder { get; set; } = "";

    [Value(index: 0, Required = true, HelpText = "File to unpack")]
    public string InputFile { get; set; } = "";

    [Usage(ApplicationAlias = "ax")]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Extract input file to current working folder", new UnpackArguments { InputFile = "c:\\temp\\inputfile.axz" });
            yield return new Example("Extract input file to specified output folder", new UnpackArguments { InputFile = "c:\\temp\\inputfile.axz", OutputFolder  = "C:\\temp\\output"});
        }
    }
}