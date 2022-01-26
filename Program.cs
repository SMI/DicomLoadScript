using LoadScript;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

const string ProgressFilename = "LastMonthDone.txt";
const string ConfigFile = "Config.yaml";

Config config;


if(!File.Exists(ConfigFile))
{

    try
    {

        Console.WriteLine($"{ConfigFile} not found, you will be prompted for settings now");
        config = Config.MakeUserType();

        Console.WriteLine($"Storing the settings you entered into {ConfigFile}");
        var serializer = new Serializer();
        File.WriteAllText(ConfigFile, serializer.Serialize(config));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to get or store {ConfigFile} settings from user: {ex}");
        return;
    }
}
else
{
    try
    {
        var deserialize = new Deserializer();
        config = deserialize.Deserialize<Config>(File.ReadAllText(ConfigFile));

        Console.WriteLine($"Loaded settings in {ConfigFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to deserialize {ConfigFile}: {ex}");
        return;
    }
}

string startAtMonth;

if (!File.Exists(ProgressFilename))
{
    startAtMonth = "01";
}
else
{
    var progress = int.Parse(File.ReadAllText(ProgressFilename).Trim());
    
    progress++;
    startAtMonth = progress.ToString("D2");
}


int year = int.Parse(new DirectoryInfo(config.YearDir).Name);
int month = int.Parse(startAtMonth);

GoGo:

if (month > 12)
{
    Console.WriteLine("Year is finished");
    return;
}

StringBuilder sb = new StringBuilder();
for(int i=1;i<= DateTime.DaysInMonth(year, month); i++)
{
    sb.AppendLine(Path.Combine(config.YearDir, month.ToString("D2"), i.ToString("D2")));
}


File.WriteAllText(Path.Combine(config.ForLoading, "LoadMe.txt"), sb.ToString());


int retryIdx = 0;
Retry:

var pCheck = Process.Start(config.RdmpCli, $"dle -l {config.LoadMetadataID} --command check");
pCheck.WaitForExit();

if(pCheck.ExitCode != 0)
{
    Console.WriteLine("Checking failed");
    if(config.ShouldTry(retryIdx))
    {
        config.RetrySleep(retryIdx);
        retryIdx++;
        Console.WriteLine("Retrying checks");
        goto Retry;
    }
    else
    {
        Console.WriteLine("Giving up, failed too often");
        return;
    }
}

var pRun = Process.Start(config.RdmpCli, $"dle -l {config.LoadMetadataID} --command run");
pRun.WaitForExit();

if (pRun.ExitCode != 0)
{
    Console.WriteLine("Running failed");
    if (config.ShouldTry(retryIdx))
    {
        config.RetrySleep(retryIdx);
        retryIdx++;
        Console.WriteLine("Retrying (starting with rechecking)");
        goto Retry;
    }
    else
    {
        Console.WriteLine("Giving up, failed too often");
        return;
    }
}

// we finished this month succesfully
File.WriteAllText(ProgressFilename, month.ToString("D2"));
month++;

goto GoGo;