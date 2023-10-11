// See https://aka.ms/new-console-template for more information

// args[0] BDS根目录
// args[1] BDS更新压缩包目录
// args[2] LL更新包目录

using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using Updata_BDS_AND_LL;
using Tools.Logger;
using Tools.Address;
using Tools.Fileoperate;
using Tools.Net;
using System.Net;
using Newtonsoft.Json;
using Update_BDS_AND_LL;

Logger logger = new();

bool hasBDS_Update = false;
string work_path = Functions.CheckPathEnd(Directory.GetCurrentDirectory());

#if DEBUG
work_path = Functions.CheckPathEnd(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) + "BDSDebug\\";
Functions.CheckPath(work_path);
logger.Warn("当前正在使用DEBUG模式,此模式仅用于开发调试,正式生产工作禁用!!!");
#endif

string updatepack_path = work_path + "UpdatePack/";
bool localNoFoundBDSTag = !File.Exists(work_path + "bedrock_server.exe");
string? fileVer = String.Empty;
string? BDSVer = String.Empty;
string Serveraddr = "https://www.minecraft.net/en-us/download/server/bedrock/";
string BDSPilterA = "https://minecraft.azureedge.net/bin-win/bedrock-server-";
string BDSPilterB = ".zip";
string BDSDownaddr = "";

string RemoteBDSVersion = String.Empty;

string LLInfoaddr = "https://api.github.com/repos/LiteLDev/LiteLoaderBDSv2/releases/latest";
string LLproxyDown = "https://ghproxy.com/{0}";

string? thispath = Process.GetCurrentProcess().MainModule?.FileName;



if(thispath != null)
{
    fileVer = FileVersionInfo.GetVersionInfo(thispath).FileVersion;
}

if (!localNoFoundBDSTag)
{
    BDSVer = FileVersionInfo.GetVersionInfo(work_path + "bedrock_server.exe").FileVersion;
}

Console.OutputEncoding = System.Text.Encoding.UTF8;



logger.Info("开发者: {0}, 项目创建时间: {1}, GitHub: {2}", "CNGEGE", "2022-07-27", "https://github.com/cngege/Update_BDS_AND_LL");
if(fileVer != String.Empty)
{
    logger.Info("程序版本: {0}", fileVer);
}
if (BDSVer != String.Empty && BDSVer != null)
{
    logger.Info("BDS程序版本: {0}", BDSVer);
}
logger.SubTitle("关于").Color(ConsoleColor.Green).Info("本程序使用的是LL2.0版本, 3.0版本尚不成熟,暂不支持");
logger.SubTitle("说明").Info("将本程序放置于BDS根目录下");
logger.SubTitle("说明").Info("将更新包BDS、LL压缩包，放在[UpdatePack]文件夹中,程序将会解压所有更新包的根目录");
logger.SubTitle("说明").Info("将BDS插件放到[UpdatePack/plugins]文件夹中,程序自动将该文件夹的所有内容移动到BDS插件目录");
logger.SubTitle("说明").Info("程序自动判断压缩包中是否含有BDS配置文件,本地存在则不覆盖");
logger.SubTitle("说明").Info("运行程序自动解压缩压缩包,如果检测到更新了BDS,且存在LLPeEditor.exe");
logger.SubTitle("说明").Info("则自动运行LLPeEditor.exe生成bedrock_server_mod.exe");
logger.SubTitle("说明").Info("更新后自动删除更新包");
logger.SubTitle("说明").Info("*请确保运行环境目录就是本程序存在的目录");
logger.SubTitle("下载地址").Info("BDS下载地址：{0}", "https://www.minecraft.net/zh-hans/download/server/bedrock");
logger.SubTitle("下载地址").Info("LL下载地址：{0}", "https://github.com/LiteLDev/LiteLoaderBDSv2/releases");

//不接受传参自定义上传文件文件夹
Functions.CheckPath(updatepack_path);

//检测MC进程
if(Address.GetPid("bedrock_server") != 0 || Address.GetPid("bedrock_server_mod") != 0)
{
    logger.Warn("[进程冲突] 检测到系统有BDS进程,无法判断是否是将升级的BDS,请自行决定是否手动关闭BDS");
    Console.Write("[暂停] 按回车继续...");
    Console.ReadLine();
}

//TODO 下载BDS和LL
//y重试/确认下载 n退出 直接回车跳过这步

while (true)
{
    logger.Info("开始获取BDS官网下载相关信息");
    var headers = new WebHeaderCollection();
    headers.Set("Pragma", "no-cache");
    headers.Set("Upgrade-Insecure-Requests", "1");
    headers.Set("Cache-Control", "no-cache");
    headers.Set("Cookie", "ApplicationGatewayAffinityCORS=1bfc026d9c4b7d17a636076dd33a8622");
    string Httpdata = Download.GetHttpData(Serveraddr, headers);
    int BDSstartposition = Httpdata.IndexOf(BDSPilterA);
    if (Httpdata == "" || BDSstartposition == -1)
    {
        logger.Error("[下载BDS] 官网获取BDS信息失败,请输入选择一项更改下面的工作");
        logger.Info("输入 y 回车 重新获取bds下载信息");
        logger.Info("输入 n 回车 结束本程序");
        logger.Info("直接 回车 跳过BDS下载,开始下一项");
        string? input = Console.ReadLine();
        if (input == null || input == "")
        {
            break;
        }
        if(input.ToLower() == "y")
        {
            continue;
        }
        if(input.ToLower() == "n")
        {
            logger.Warn("本次更新到此结束");
            return;
        }
    }
    else
    {
        //TagShow("寻找Windows下载地址特征结束位置");
        int zipstartposition_BDS = Httpdata.IndexOf(BDSPilterB, BDSstartposition);
        //TagShow("过滤出Windows服务器最新版本");
        RemoteBDSVersion = Httpdata.Substring(BDSstartposition + BDSPilterA.Length, zipstartposition_BDS - BDSstartposition - BDSPilterA.Length);
        //TagShow("计算Windows下载地址长度");
        int BDSaddrLong = zipstartposition_BDS + BDSPilterB.Length - BDSstartposition;
        //TagShow("找出Windows下载地址");
        BDSDownaddr = Httpdata.Substring(BDSstartposition, BDSaddrLong);

        logger.Info("获取到BDS下载地址: {0}", BDSDownaddr);
        logger.Info("BDS最新版本号: {0}", RemoteBDSVersion);
        logger.Color(ConsoleColor.Yellow).Info("请输入选择一项以决定下面的工作");
        logger.Info("输入 y 回车 进行BDS下载");
        logger.Info("输入 n 回车 结束本程序");
        logger.Info("直接 回车 跳过BDS下载,开始下一项");
        string? input = Console.ReadLine();
        if (input == null || input == "")
        {
            break;
        }
        if (input.ToLower() == "n")
        {
            logger.Warn("本次更新到此结束");
            return;
        }
        if (input.ToLower() == "y")
        {
            //下载：
            logger.Info("开始下载,请稍候...");
            bool downok = false;
            long filesize = 0;
            long downsize = 0;

            var DownBDS = new Download(BDSDownaddr, updatepack_path, $"BDS{RemoteBDSVersion}.zip");
            DownBDS.Suffix = ".bds";
            DownBDS.Downprogress += (long _filesize, long _downsize, bool waft) =>
            {
                filesize = _filesize;
                downsize = _downsize;

                if (waft)
                {
                    downok = true;
                }
            };
            int DownStatus = DownBDS.Start();
            if(DownStatus == 0)
            {
                logger.Error("BDS{0}.zip 下载失败,本次更新到此结束", RemoteBDSVersion);
                return;
            }
            if(DownStatus == 1)
            {
                logger.Info("BDS{0}.zip 已经创建线程下载,下载中", RemoteBDSVersion);
                while (true)
                {
                    // 显示下载进度
                    if (filesize != 0)
                    {
                        Console.Write(new String(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        if (downok)
                        {
                            Console.WriteLine("100% 完成.");
                            break;
                        }
                        Console.Write(string.Format("{0}% 下载中...", Download.GetintPercent(filesize, downsize)));
                        Console.SetCursorPosition(0, Console.CursorTop);
                    }
                    Thread.Sleep(1000);
                }
                logger.Info("BDS{0}.zip 下载完成. ", RemoteBDSVersion);
                break;
            }
            if(DownStatus == 2)     // 文件已经存在,或文件很小一次请求就下载完成
            {
                logger.Info("BDS{0}.zip 下载完成.", RemoteBDSVersion);
                break;
            }
        }
    }
}



//下载LL
while (true)
{
    logger.Info("开始获取GitHub LL下载相关信息");
    // 如果存在 LiteLoader.dll 则显示文件版本
    if (File.Exists(work_path + "LiteLoader.dll"))
    {
        string? llver = FileVersionInfo.GetVersionInfo(work_path + "LiteLoader.dll").FileVersion;
        if (llver != null)
        {
            logger.Info("已安装LL的版本: {0}", llver);
        }
    }


    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
    string Httpdata = Download.GetHttpData(LLInfoaddr);
    LLJson? LL = JsonConvert.DeserializeObject<LLJson>(Httpdata);
    if(Httpdata == "" || Httpdata.IndexOf("{") != 0 || LL == null || LL.assets == null || LL.assets.Count == 0)
    {
        logger.Error("[下载LL] GitHub获取LL信息失败,请输入选择一项更改下面的工作");
        logger.Info("输入 y 回车 重新获取LL下载信息");
        logger.Info("输入 n 回车 结束本程序");
        logger.Info("直接 回车 跳过LL下载,开始下一项");
        string? input = Console.ReadLine();
        if (input == null || input == "")
        {
            break;
        }
        if (input.ToLower() == "y")
        {
            continue;
        }
        if (input.ToLower() == "n")
        {
            logger.Warn("本次更新到此结束");
            return;
        }
    }
    else
    {
        logger.Info("获取到LL下载地址: {0}", LL.assets.First().browser_download_url);
        logger.Info("LL版本标签号: {0}", LL.tag_name);
        logger.Info("LL版本号: {0}", LL.name);
        if(LL.body != null)
        {
            string[] bodys = LL.body.Split("\n");
            foreach (string body in bodys)
            {
                if (body.ToLower().IndexOf("bds-") != -1)
                {
                    logger.Color(ConsoleColor.DarkGreen).Info("和BDS有关介绍:{0}", body);
                    break;
                }
            }
        }
        logger.Color(ConsoleColor.Yellow).Info("请输入选择一项以决定下面的工作");
        logger.Info("输入 y 回车 进行LL下载");
        logger.Info("输入 p 回车 使用代理链接加速下载({0})", LLproxyDown);
        logger.Info("输入 n 回车 结束本程序");
        logger.Info("直接 回车 跳过LL下载,开始下一项");
        string? input = Console.ReadLine();
        if (input == null || input == "")
        {
            break;
        }
        if (input.ToLower() == "n")
        {
            logger.Warn("本次更新到此结束");
            return;
        }
        if (input.ToLower() == "y" || input.ToLower() == "p")
        {
            //下载：
            logger.Info("开始下载,请稍候...");
            string? url = (input.ToLower() == "p") ? string.Format(LLproxyDown, LL.assets.First().browser_download_url) : LL.assets.First().browser_download_url;
            bool downok = false;
            long filesize = 0;
            long downsize = 0;

            var DownBDS = new Download(url, updatepack_path, LL.assets.First().name);
            DownBDS.Suffix = ".ll";
            DownBDS.Downprogress += (long _filesize, long _downsize, bool waft) =>
            {
                filesize = _filesize;
                downsize = _downsize;

                if (waft)
                {
                    downok = true;
                }
            };
            int DownStatus = DownBDS.Start();
            if (DownStatus == 0)
            {
                logger.Error("{0} 下载失败,本次更新到此结束", LL.assets.First().name);
                return;
            }
            if (DownStatus == 1)
            {
                logger.Info("{0} 已经创建线程下载,下载中", LL.assets.First().name);
                while (true)
                {
                    // 显示下载进度
                    if (filesize != 0)
                    {
                        Console.Write(new String(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, Console.CursorTop);
                        if (downok)
                        {
                            Console.WriteLine("100% 完成.");
                            break;
                        }
                        Console.Write(string.Format("{0}% 下载中...", Download.GetintPercent(filesize, downsize)));
                        Console.SetCursorPosition(0, Console.CursorTop);
                    }
                    Thread.Sleep(1000);
                }
                logger.Info("{0} 下载完成. ", LL.assets.First().name);
                break;
            }
            if (DownStatus == 2)
            {
                logger.Info("{0} 下载完成.", LL.assets.First().name);
                break;
            }
        }
    }
}



/* 检测所有压缩包并解压 */
string[] UpdatePackList = Directory.GetFiles(updatepack_path, "*.zip");

if(UpdatePackList.Length > 0)
{
    logger.Info("[更新包] 找到了{0}个更新包,开启解压...", UpdatePackList.Length);
    for (int i = 0; i < UpdatePackList.Length; i++)
    {
        logger.Info("[{0}/{1}] 正在更新: {2}", i+1, UpdatePackList.Length, Path.GetFileName(UpdatePackList[i]));
        (new FastZip()).ExtractZip(UpdatePackList[i], work_path, FastZip.Overwrite.Prompt, zipOverwrite, null, null, true);
        File.Delete(UpdatePackList[i]);
        logger.Info("[{0}/{1}] {2} 解压完成", i+1, UpdatePackList.Length, Path.GetFileName(UpdatePackList[i]));
    }
}
else
{
    logger.Warn("[更新包] 没有找到更新包,跳过解压");
}

// LL更新后压缩包根目录存在 LiteLoaderBDS 文件夹
// 解压之后得将这个文件夹中的内容移动出来
if (Directory.Exists(work_path + "LiteLoaderBDS/"))
{
    logger.SubTitle("LiteLoaderBDS").Info("解压后处理");
    Folder.DirMoveAllItem(work_path + "LiteLoaderBDS/", work_path);
    Directory.Delete(work_path + "LiteLoaderBDS");
    logger.SubTitle("LiteLoaderBDS").Info("完成");
}

//将更新文件夹里的[plugins]文件夹下的文件文件移动到插件目录下
Functions.CheckPath(updatepack_path + "plugins/");
//TODO  进行文件的移动

if (Directory.Exists(work_path + "plugins/"))
{
    logger.SubTitle("插件更新").Info("检测到BDS目录有插件文件夹,开始更新插件");
    Folder.DirMoveAllItem(updatepack_path + "plugins/", work_path + "plugins/");
    logger.SubTitle("插件更新").Info("完成");
}


//更新前本地没有BDS
if (localNoFoundBDSTag)
{
    //更新后本地有BDS
    if (File.Exists(work_path + "bedrock_server.exe"))
    {
        hasBDS_Update = true;
    }
}

// 表示进行过BDS更新, 向BDS写入版本号 （没法写入，因为要修改exe
/*
if (hasBDS_Update)
{
    if (BDSVer == String.Empty || BDSVer == null && RemoteBDSVersion != string.Empty)
    {
       
    }
}
*/

//表示进行过 BDS更新
if (File.Exists(work_path + "PeEditor.exe") && (hasBDS_Update || !File.Exists(work_path + "bedrock_server_mod.exe")))
{
    logger.SubTitle("PeEditor").Info("检测到进行过BDS更新");
    logger.SubTitle("PeEditor").Info("正在生成 bedrock_server_mod.exe");

    var process = new Process();
    var startInfo = new ProcessStartInfo(work_path + "PeEditor.exe", "-m -b");
    startInfo.CreateNoWindow = true;
    startInfo.UseShellExecute = false;
    startInfo.WorkingDirectory = work_path;             //设置工作目录为BDS目录
    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo = startInfo;
    process.Start();
    process.WaitForExit(50 * 1000);
    process.Close();
}

logger.Info("更新全部结束");

// 延迟3s关闭
Thread.Sleep(3000);

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
