// See https://aka.ms/new-console-template for more information

// args[0] BDS根目录
// args[1] BDS更新压缩包目录
// args[2] LL更新包目录

using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using Updata_BDS_AND_LL;
using Update_BDS_AND_LL;

bool hasBDS_Update = false;
string work_path = Functions.CheckPathEnd(System.IO.Directory.GetCurrentDirectory());
string updatepack_path = "UpdatePack/";
bool localNoFoundBDSTag = !File.Exists("bedrock_server.exe");

Console.OutputEncoding = System.Text.Encoding.UTF8;

Logger.Info("开发者: {0}, 项目创建时间: {1}, GitHub: {2}", "CNGEGE", "2022-07-27", "https://github.com/cngege");
//Logger.Info("程序版本: {0}\n", FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);

Logger.Info("[更新说明] 请务必确保将要更新的BDS进程已经关闭,升级程序不会检测这些");
Logger.Info("[更新说明] 将本程序放置于BDS根目录下");
Logger.Info("[更新说明] 将更新包BDS、LL压缩包，放在[UpdatePack]文件夹中,程序将会解压所有更新包的根目录");
Logger.Info("[更新说明] 程序自动判断压缩包中是否含有BDS配置文件,本地存在则不覆盖");
Logger.Info("[更新说明] 运行程序自动解压缩压缩包,如果检测到更新了BDS,且存在LLPeEditor.exe");
Logger.Info("[更新说明] 则自动运行LLPeEditor.exe生成bedrock_server_mod.exe");
Logger.Info("[更新说明] 更新后自动删除更新包");
Logger.Info("[更新说明] *请确保运行环境目录就是本程序存在的目录");
Logger.Info("[下载地址] BDS下载地址：{0}", "https://www.minecraft.net/zh-hans/download/server/bedrock");
Logger.Info("[下载地址] LL下载地址：{0}", "https://github.com/LiteLDev/LiteLoaderBDS/releases");

//不接受传参自定义上传文件文件夹
Functions.CheckPath(updatepack_path);


/* 检测所有压缩包并解压 */
string[] UpdatePackList = Directory.GetFiles(updatepack_path, "*.zip");

if(UpdatePackList.Length > 0)
{
    Logger.Info("[更新包] 找到了{0}个更新包,开启解压...", UpdatePackList.Length);
    for (int i = 0; i < UpdatePackList.Length; i++)
    {
        Logger.Info("[{0}/{1}] 正在更新: {2}", i+1, UpdatePackList.Length, Path.GetFileName(UpdatePackList[i]));
        (new FastZip()).ExtractZip(UpdatePackList[i], ".", FastZip.Overwrite.Prompt, zipOverwrite, null, null, true);
        File.Delete(UpdatePackList[i]);
        Logger.Info("[{0}/{1}] {2} 解压完成", i+1, UpdatePackList.Length, Path.GetFileName(UpdatePackList[i]));
    }

}
else
{
    Logger.Warn("[更新包] 没有找到更新包,跳过解压");
}

//将更新文件夹里的[plugins]文件夹下的文件文件移动到插件目录下
Functions.CheckPath(updatepack_path + "plugins/");
//TODO  进行文件的移动

//更新前本地没有BDS
if (localNoFoundBDSTag)
{
    //更新后本地有BDS
    if (File.Exists("bedrock_server.exe"))
    {
        hasBDS_Update = true;
    }
}

//表示进行过 BDS更新
if (hasBDS_Update && File.Exists("LLPeEditor.exe"))
{
    Logger.Info("[LLPeEditor] 检测到进行过BDS更新");
    Logger.Info("[LLPeEditor] 正在生成 bedrock_server_mod.exe");

    Process process = new Process();
    ProcessStartInfo startInfo = new ProcessStartInfo("LLPeEditor.exe", "--noPause");
    startInfo.CreateNoWindow = true;
    startInfo.UseShellExecute = false;
    startInfo.WorkingDirectory = work_path;       //设置工作目录为BDS目录
    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo = startInfo;
    process.Start();
    process.WaitForExit(50 * 1000);
    process.Close();
}

Logger.Info("更新全部结束");


//覆盖询问
bool zipOverwrite(string filepath){
    string filename = Path.GetFileName(filepath);
    //如果这些文件已经存在 则不会更新
    if (filename == "server.properties" || filename == "permissions.json" || filename == "allowlist.json")
        return false;
    if (filename == "bedrock_server.exe")
        hasBDS_Update = true;
    return true;
};