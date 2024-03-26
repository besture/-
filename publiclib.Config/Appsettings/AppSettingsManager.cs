using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Runtime.InteropServices;

namespace publiclib.Config
{
    public static class AppSettingsManager
    {
        public static IConfiguration GetAppSettings()
        {
            SetAppSettingValue();


            IConfiguration config = new ConfigurationBuilder()
                                .SetBasePath(GetPath())
                                .Add(new JsonConfigurationSource
                                {
                                    Path = "appsettings.json",
                                    Optional = false,
                                    ReloadOnChange = true
                                })
                                .Build();
            return config;
        }

        public static IConfigurationSection GetValue(this IConfiguration config, string key)
        {
            return config.GetSection(key);
        }

        /// <summary>
        /// 升级后保存json信息
        /// </summary>
        public static void SetAppSettingValue()
        {
            try
            {
                string sOldPath = GetPath(false) + "appsettings_old.json";
                string sOldContent = string.Empty;
                if (File.Exists(sOldPath))
                {
                    using (var stream = new FileStream(sOldPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var fileReader = new StreamReader(stream))
                    {
                        sOldContent = fileReader.ReadToEnd();
                    }
                }

                if (!string.IsNullOrWhiteSpace(sOldContent))
                {
                    string sNewPath = GetPath(false) + "appsettings.json";
                    if (!File.Exists(sNewPath))
                    {
                        File.Copy(sOldPath, sNewPath);
                    }
                    else
                    {
                        string sNewContent = string.Empty;
                        using (var stream = new FileStream(sNewPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                        using (var fileReader = new StreamReader(stream))
                        {
                            sNewContent = fileReader.ReadToEnd();
                        }
                        if (string.IsNullOrWhiteSpace(sNewContent))
                        {
                            System.IO.File.WriteAllText(sNewPath, sOldContent);
                        }
                        else
                        {
                            Newtonsoft.Json.Linq.JObject jsonObjOld = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(sOldContent);
                            Newtonsoft.Json.Linq.JObject jsonObjNew = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(sNewContent);

                            var v1 = GetChild(jsonObjNew.Properties().ToList());
                            var v2 = GetChild(jsonObjOld.Properties().ToList());

                            foreach (var op in v2)
                            {
                                var temp = v1.FirstOrDefault(m => m.Path == op.Path);
                                if (temp!=null&&!temp.Path.Contains("AppSettings.ApiVersion"))
                                {
                                    temp.Value = op.Value;
                                }
                            }

                            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObjNew, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText(sNewPath, output);
                        }
                    }


                    File.Delete(sOldPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("还原配置失败..." + ex);
            }
        }

        private static List<Newtonsoft.Json.Linq.JValue> GetChild(List<Newtonsoft.Json.Linq.JProperty> v1)
        {
            List<Newtonsoft.Json.Linq.JValue> ls = new List<Newtonsoft.Json.Linq.JValue>();
            foreach (var op in v1)
            {
                if (op.Value is Newtonsoft.Json.Linq.JObject jb)
                {
                    ls.AddRange(GetChild(jb.Properties().ToList()));
                }
                else if (op.Value is Newtonsoft.Json.Linq.JValue jv)
                {
                    ls.Add(jv);
                }
            }
            return ls;
        }

        /// <summary>
        /// 获取当前配置文件的目录
        /// </summary>
        /// <returns></returns>
        public static string GetPath(bool isExists = true)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var baseDir1 = baseDir;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (isExists)
                {
                    if (!File.Exists(baseDir1 + "appsettings.json"))
                    {
                        if (!File.Exists(baseDir1 + "appsettings.json"))
                        {
                            throw new Exception("未找到配置文件appsettings.json");
                        }
                    }
                }
            }
            else
            {
                if (isExists)
                {
                    if (!File.Exists(baseDir1 + "appsettings.json"))
                    {
                        if (!File.Exists(baseDir1 + "appsettings.json"))
                        {
                            throw new Exception("未找到配置文件appsettings.json");
                        }
                    }
                }
            }
            return baseDir1;
        }
    }
}