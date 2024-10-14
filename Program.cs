// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

if (args.Length < 2){
    Console.WriteLine("1. Put all your zipped photos in a directory. ");
    Console.WriteLine("Need target path.  \nUsage: $ iPhotoGrabr <path+zipFile> <target-folder>");
    return;
}

if (args[0] == "find-dups"){
    FindDuplicateFiles(args[1]);
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
                if (File.Exists(destinationPath)){
                    destinationPath = Path.Combine(targetPath,$"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}{Path.GetExtension(entry.Name)}");
                }
                Console.WriteLine($"Extracting {entry.Name} to {destinationPath}");
                entry.ExtractToFile(destinationPath);
            }
        }
    }
}

void FindDuplicateFiles(String path){
    var allFiles = Directory.GetFiles(path);
    Console.WriteLine($"Examining {allFiles.Length} files for duplicates.");
    // SHA256-hash, filename
    Dictionary<string, string> hashedFiles = new();
    List<(string,string)> dupFiles = new List<(string,string)>();
    int fileCounter = 0;
    foreach (string f in allFiles){
        Byte [] allBytes = File.ReadAllBytes(f);
        var sha256 = GenSha256(allBytes);

        // Console.WriteLine($"{sha256} : {Path.GetFileName(f)}");
        if (!hashedFiles.TryAdd(sha256, Path.GetFileName(f))){
            // Could not add the file because it's hash is already found
            // which indicates that this is a duplicate of another which
            // is already in hashedFiles
            string origFile = "";
            // get original file that this is a duplicate of
            hashedFiles.TryGetValue(sha256,out origFile);
            // add the dup to the dup file list
            dupFiles.Add(($"{Path.GetFileName(f)}",$"{origFile}"));
        }        
        Console.Write($"{++fileCounter} ");
    }
    Console.WriteLine($"{Environment.NewLine} Examined {fileCounter} files.");
    if (dupFiles.Count == 0){
        Console.WriteLine("There are no duplicate image files.");
        return;
    }
    foreach ((string,string)d in dupFiles){
        Console.WriteLine("Duplicate file list...");
        Console.WriteLine($"{d.Item1} same as {d.Item2}");
    }
}

string GenSha256(Byte[] allFileBytes) 
{ 
    var sha = SHA256.Create();
    byte[] hash = sha.ComputeHash(allFileBytes); 
    return String.Concat(Array.ConvertAll(hash, x => x.ToString("x2"))); 
}
