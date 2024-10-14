// See https://aka.ms/new-console-template for more information

using System.IO.Compression;

if (args.Length < 2){
    Console.WriteLine("1. Put all your zipped photos in a directory. ");
    Console.WriteLine("Need target path.  \nUsage: $ iPhotoGrabr <path+zipFile> <target-folder>");
    return;
}

var zipFile = args[0];
var targetPath = args[1];
string [] allZipFiles;

Console.WriteLine($"{zipFile}");

if (Directory.Exists(zipFile)){
    allZipFiles = Directory.GetFiles(zipFile,"*.zip");
}
else{
    allZipFiles = [$"{zipFile}"];
}

foreach (string f in allZipFiles){

    using (ZipArchive archive = ZipFile.OpenRead(f))
    {
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (!entry.Name.Contains(".mov",StringComparison.InvariantCultureIgnoreCase)){
                var destinationPath = Path.Combine(targetPath,entry.Name);
                Console.WriteLine($"copying {entry.Name} to {destinationPath}");
                entry.ExtractToFile(destinationPath);
            }
        }
    }
}
