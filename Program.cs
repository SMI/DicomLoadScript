using System;
using System.Diagnostics;
using System.IO;
using System.Text;

const string ProgressFilename = "LastMonthDone.txt";

if(!File.Exists("rdmp.exe"))
{
    Console.WriteLine("This file must be run from the RDMP CLI directory");
    return;
}

Console.WriteLine("Enter Year Directory e.g. G:\\2010\\");
var yearDir = new DirectoryInfo(Console.ReadLine());

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

Console.WriteLine("Enter ForLoading Directory e.g. C:\\temp\\ImageLoading\\Data\\ForLoading");
var forLoading = Console.ReadLine();

Console.WriteLine("Enter RDMP Load ID e.g. 3136");
var lmdId = Console.ReadLine();

int year = int.Parse(yearDir.Name);
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
    sb.AppendLine(Path.Combine(yearDir.FullName, month.ToString("D2"), i.ToString("D2")));
}


File.WriteAllText(Path.Combine(forLoading, "LoadMe.txt"), sb.ToString());


int retryCount = 10;
Retry:

var pCheck = Process.Start("rdmp.exe", $"dle -l {lmdId} --command check");
pCheck.WaitForExit();

if(pCheck.ExitCode != 0)
{
    Console.WriteLine("Checking failed");
    if(retryCount >0)
    {
        retryCount--;
        Console.WriteLine("Retrying checks");
        goto Retry;
    }
    else
    {
        Console.WriteLine("Giving up, failed too often");
        return;
    }
}

var pRun = Process.Start("rdmp.exe", $"dle -l {lmdId} --command run");
pRun.WaitForExit();

if (pRun.ExitCode != 0)
{
    Console.WriteLine("Running failed");
    if (retryCount > 0)
    {
        retryCount--;
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