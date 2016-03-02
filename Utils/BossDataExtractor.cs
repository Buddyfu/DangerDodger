using DangerDodger.Classes;
using log4net;
using Loki.Bot;
using Loki.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DangerDodger.Utils
{
    class BossDataExtractor
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static string __FILE_PATH;

        private static List<BossInfo> _bossInfos;

        private static string FILE_PATH
        {
            get
            {
                Log.Info("[DangerDodger] Retrieving BossData.json file path.");
                if (__FILE_PATH == null)
                {
                    var instance = ThirdPartyLoader.GetInstance("DangerDodger");
                    if(instance == null)
                    {
                        __FILE_PATH = @"Plugins\DangerDodger\Data\BossesData.json";
                    }
                    else
                    {
                        __FILE_PATH = Path.Combine(instance.ContentPath, @"Data\BossesData.json");
                    }
                }
                return __FILE_PATH;
            }
        }

        public static List<BossInfo> GetBossesInfo()
        {
            if (_bossInfos == null)
            {
                Log.Info("[DangerDodger] Extracting BossInfos from BossesData.json file");
                _bossInfos = new List<BossInfo>();
                if (File.Exists(FILE_PATH))
                {
                    string bossesDataTxt = String.Empty;
                    try
                    {
                        bossesDataTxt = File.ReadAllText(FILE_PATH);
                    }
                    catch (Exception e)
                    {
                        Log.Error("[DangerDodger] Error while reading the BossesData.json file.");
                        Log.Error(e);
                    }
                    if (!String.IsNullOrEmpty(bossesDataTxt))
                    {
                        try
                        {
                            _bossInfos = JsonConvert.DeserializeObject<List<BossInfo>>(bossesDataTxt);
                        }
                        catch (Exception e)
                        {
                            Log.Error("[DangerDodger] Error while deserializing the BossesData.json file.");
                            Log.Error(e);
                        }
                    }
                }
                else
                {
                    Log.Error("[DangerDodger] BossesData.json file not found. No boss attacks will be dodged.");
                }
            }

            return _bossInfos;
        }
    }
}
