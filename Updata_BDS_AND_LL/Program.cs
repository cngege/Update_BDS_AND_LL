// See https://aka.ms/new-console-template for more information

// args[0] BDS根目录
// args[1] BDS更新压缩包目录
// args[2] LL更新包目录

using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using Updata_BDS_AND_LL;

bool hasBDS_Update = false;

Console.WriteLine("开发者: {0}, TIME: {1}, GitHub: {2}\n", "CNGEGE", "2022-07-27", "https://github.com/cngege");

ConsoleColor currentForeColor = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("[进程] [info] [更新说明] 请务必确保将要更新的BDS进程已经关闭,升级程序不会检测这些");
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("[进程] [info] [更新说明] 请先将BDS和LL的zip压缩包放入对应文件夹中,然后运行本程序,传入对应参数");
Console.WriteLine("[进程] [info] [更新说明] 本程序将解压更新包到BDS中,但并不会覆盖某些关键文件");
Console.WriteLine("[进程] [info] [更新说明] 如果检测到BDS更新包,将在最后运行LLPeEditor.exe");
Console.WriteLine("[进程] [info] [更新说明] 更新后自动删除更新包");
//Console.WriteLine("\n");
Console.ForegroundColor = currentForeColor;
Console.WriteLine("[进程] [info] [url] BDS下载地址：{0}", "https://www.minecraft.net/zh-hans/download/server/bedrock");
Console.WriteLine("[进程] [info] [url] LL下载地址：{0}", "https://github.com/LiteLDev/LiteLoaderBDS/releases");
Console.WriteLine("\n");

if (args.Length < 3)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[进程] [Error] 启动时必须要传入三个参数:");
    Console.WriteLine("[进程] [Error] args[0]: BDS根目录,最好是绝对目录,最后带/");
    Console.WriteLine("[进程] [Error] args[1]: BDS更新压缩包目录,最后带/");
    Console.WriteLine("[进程] [Error] args[2]: LL更新包目录,最后带/");
    Console.WriteLine("\n");
    Console.ForegroundColor = currentForeColor;
    return;
}
//Console.ForegroundColor = currentForeColor;


/* 检测参数中的文件夹是否存在 */

Functions.CheckPath(args[0]);
Functions.CheckPath(args[1]);
Functions.CheckPath(args[2]);

/* 检测BDS更新包并解压 */

string[] BDS_UpdateList = Directory.GetFiles(args[1], "*.zip");

if(BDS_UpdateList.Length > 0)
{
    string? BDS_Update = null;
    if (BDS_UpdateList.Length > 1)
    {
        Console.WriteLine("[进程] [info] [BDS] 找到了多个BDS升级包,请选择一个进行更新");
        for (int i = 0; i < BDS_UpdateList.Length; i++)
        {
            Console.WriteLine("[{0}] {1}", i, Path.GetFileName(BDS_UpdateList[i]));
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("请输入一个序号(错误输入则不更新):");
        Console.ForegroundColor = currentForeColor;

        String? input = Console.ReadLine();
        if(input != null && int.TryParse(input,out int input_int) && input_int >= 0 && input_int < BDS_UpdateList.Length)
        {
            BDS_Update = BDS_UpdateList[input_int];
        }
    }
    else   // Length == 1
    {
        BDS_Update = BDS_UpdateList.First();
    }

    /* 更新BDS */
    if(BDS_Update != null)
    {
        hasBDS_Update = true;
        Console.WriteLine("[进程] [info] [BDS] 开始更新 {0}", Path.GetFileName(BDS_Update));
        (new FastZip()).ExtractZip(BDS_Update, args[0], FastZip.Overwrite.Prompt, zipOverwrite, null, null, true);

        File.Delete(BDS_Update);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[进程] [info] [BDS] BDS更新完成");
        Console.ForegroundColor = currentForeColor;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[进程] [info] [BDS] 跳过BDS更新");
        Console.ForegroundColor = currentForeColor;
    }
}
else
{
    Console.WriteLine("[进程] [info] [BDS] 没有找到BDS更新包");
}


/* 检测LL更新包并解压 */

string[] LL_UpdateList = Directory.GetFiles(args[2], "*.zip");
if(LL_UpdateList.Length > 0)
{
    string? LL_Update = null;
    //如果有多个LL更新压缩包的情况
    if(LL_UpdateList.Length > 1)
    {
        Console.WriteLine("[进程] [info] [LL] 找到了多个LL升级包,请选择一个进行更新");
        for (int i = 0; i < LL_UpdateList.Length; i++)
        {
            Console.WriteLine("[{0}] {1}", i, Path.GetFileName(LL_UpdateList[i]));
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("请输入一个序号(错误输入则不更新):");
        Console.ForegroundColor = currentForeColor;

        String? input = Console.ReadLine();
        if (input != null && int.TryParse(input, out int input_int) && input_int >= 0 && input_int < LL_UpdateList.Length)
        {
            LL_Update = BDS_UpdateList[input_int];
        }
    }
    else   //Length == 1
    {
        LL_Update = LL_UpdateList.First();
    }

    /* 更新LL */
    if(LL_Update != null)
    {
        Console.WriteLine("[进程] [info] [LL] 开始更新 {0}", Path.GetFileName(LL_Update));
        (new FastZip()).ExtractZip(LL_Update, args[0], null);

        File.Delete(LL_Update);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[进程] [info] [LL] LL更新完成");
        Console.ForegroundColor = currentForeColor;
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[进程] [info] [LL] 跳过LL更新");
        Console.ForegroundColor = currentForeColor;
    }
}
else
{
    Console.WriteLine("[进程] [info] [LL] 没有找到LL更新包");
}

//表示进行过 BDS更新
if (hasBDS_Update && File.Exists(args[0] + "LLPeEditor.exe"))
{
    Console.WriteLine("[进程] [info] [LLPeEditor] 检测到进行过BDS更新");
    Console.WriteLine("[进程] [info] [LLPeEditor] 正在生成 bedrock_server_mod.exe");

    Process process = new Process();
    ProcessStartInfo startInfo = new ProcessStartInfo(args[0] + "LLPeEditor.exe", "--noPause");
    startInfo.CreateNoWindow = true;
    startInfo.UseShellExecute = false;
    startInfo.WorkingDirectory = args[0];       //设置工作目录为BDS目录
    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo = startInfo;
    process.Start();
    process.WaitForExit(50 * 1000);
    process.Close();
}

Console.WriteLine("[进程] [info] 更新全部结束");


bool zipOverwrite(string filepath){
    string filename = Path.GetFileName(filepath);
    //如果这些文件已经存在 则不会更新
    if (filename == "server.properties" || filename == "permissions.json" || filename == "allowlist.json")
        return false;
    return true;
};