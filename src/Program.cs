using Ax.Arguments;
using Ax.Operations;
using CommandLine;

Parser.Default.ParseArguments<PackArguments, UnpackArguments>(args)
    .WithParsed<UnpackArguments>(Unpack.Execute)
    .WithParsed<PackArguments>(Pack.Execute);