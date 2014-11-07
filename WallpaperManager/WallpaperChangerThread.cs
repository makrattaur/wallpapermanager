using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Collections.Concurrent;
using System.IO;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Data;
using System.Data.SQLite;

namespace WallpaperManager
{
    public class WallpaperChangerThread
    {
        public WallpaperChangerThread()
        {
            messageQueue = new ConcurrentQueue<MessageType>();
            messageEvent = new AutoResetEvent(false);

            monitors = Utils.GetMonitors();

            InitSettingsAndState();
            LoadState();
            LoadConfig();
        }


        private void InitSettingsAndState()
        {
            monitorStates = new MonitorState[monitors.Length];
            for (int i = 0; i < monitorStates.Length; i++)
            {
                monitorStates[i] = new MonitorState();
            }

            monitorSettings = new MonitorSettings[monitorStates.Length];
            for (int i = 0; i < monitorSettings.Length; i++)
            {
                monitorSettings[i] = new MonitorSettings();
            }
        }

        string MakePath(string subPath)
        {
            return Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), "WallpaperManager", subPath);
        }

        string GetStateFilePath()
        {
            return MakePath("data\\state.json");
        }

        string GetConfigFilePath()
        {
            return MakePath("config\\config.json");
        }

        string GetDatabasePath()
        {
            return MakePath("data\\history.sqlite");
        }

        string GetGeneratedWallpaperPath()
        {
            return MakePath("data\\wallpaper.png");
        }

        const string STATE_FILE = "state.json";
        void SaveState()
        {
            File.WriteAllText(GetStateFilePath(), new JObject(
                new JProperty("multi", new JArray(
                    monitorStates.Select(ms => new JObject(
                        new JProperty("last-change", ms.LastChange.Ticks),
                        new JProperty("current", ms.CurrentWallpaper)
                    )
                ))
            )).ToString());
        }

        void LoadState()
        {
            if (!File.Exists(GetStateFilePath()))
            {
                return;
            }

            JObject jo = JObject.Parse(File.ReadAllText(GetStateFilePath()));
            monitorStates = jo["multi"]
                .Children<JObject>()
                .Select(sj => new MonitorState()
                {
                    CurrentWallpaper = (string)sj["current"],
                    LastChange = new DateTime((long)sj["last-change"])
                })
                .ToArray();
        }

        static Color ParseColor(JObject jo)
        {
            return Color.FromArgb((int)jo["r"], (int)jo["g"], (int)jo["b"]);
        }

        static TEnum ParseEnum<TEnum>(Dictionary<string, TEnum> lookup, string value, TEnum defaultValue)
        {
            return lookup.ContainsKey(value) ? lookup[value] : defaultValue;
        }

        Dictionary<string, WallpaperMode> modeLookup = new Dictionary<string, WallpaperMode>()
        {
            { "single", WallpaperMode.Single },
            { "random", WallpaperMode.Random },
            { "fair-random", WallpaperMode.FairRandom },
            { "sequential", WallpaperMode.Sequential }
        };

        Dictionary<string, WallpaperDisposition> dispositionLookup = new Dictionary<string, WallpaperDisposition>()
        {
            { "fill", WallpaperDisposition.Fill },
            { "fit", WallpaperDisposition.Fit },
            { "center", WallpaperDisposition.Center },
            { "stretch", WallpaperDisposition.Stretch },
            { "tile", WallpaperDisposition.Tile },
            { "match-width", WallpaperDisposition.MatchWidth },
            { "match-height", WallpaperDisposition.MatchHeight }
        };

        static FileOrDirectory[] ParseSources(JArray ja)
        {
            return ja
                .Children<JObject>()
                .Select(jo => new FileOrDirectory() 
                {
                    IsDirectory = (string)jo["type"] == "directory",
                    IsRecursive = ((IDictionary<string, JToken>)jo).ContainsKey("recursive") ? (bool)jo["recursive"] : false,
                    Path = (string)jo["value"]
                })
                .ToArray();
        }

        void LoadConfig()
        {
            if (!File.Exists(GetConfigFilePath()))
            {
                return;
            }

            JObject jo = JObject.Parse(File.ReadAllText(GetConfigFilePath()));
            monitorSettings = jo["multi"]
                .Children<JObject>()
                .Select(ms => new MonitorSettings() 
                {
                    ChangeInterval = (int)ms["interval"],
                    SingleImage = (string)ms["file"] ?? "",
                    BackgroundColor = ms["background-color"] != null ? ParseColor((JObject)ms["background-color"]) : default(Color),
                    Mode = ParseEnum(modeLookup, (string)ms["mode"], WallpaperMode.Single),
                    Disposition = ParseEnum(dispositionLookup, (string)ms["disposition"], WallpaperDisposition.Stretch),
                    RandomSources = ParseSources((JArray)ms["sources"])
                })
                .ToArray();
        }


        public void Start()
        {
            if (changerThread != null)
            {
                return;
            }

            changerThread = new Thread(ThreadProc);
            changerThread.SetApartmentState(ApartmentState.STA);
            changerThread.Start();
        }

        public void Stop()
        {
            if (!exited)
            {
                QueueMessage(MessageType.Stop);
            }
        }

        public void ReloadConfig()
        {
            QueueMessage(MessageType.ReloadConfig);
        }

        public void PauseCycle()
        {
            QueueMessage(MessageType.PauseCycle);
        }

        public void ResumeCycle()
        {
            QueueMessage(MessageType.ResumeCycle);
        }

        public bool IsCyclePaused()
        {
            return cyclePaused;
        }

        public void SkipCycle()
        {
            QueueMessage(MessageType.SkipToNext);
        }

        private void ThreadProc()
        {
            while (running)
            {
                int nextInterval = ComputeNextInterval();
                if (messageEvent.WaitOne(nextInterval))
                {
                    MessageType msg;

                    while (messageQueue.TryDequeue(out msg))
                    {
                        switch (msg)
                        {
                            case MessageType.ReloadConfig:
                            {
                                LoadConfig();
                                break;
                            }
                            case MessageType.Stop:
                            {
                                running = false;
                                break;
                            }
                            case MessageType.PauseCycle:
                            {
                                cyclePaused = true;
                                break;
                            }
                            case MessageType.ResumeCycle:
                            {
                                cyclePaused = false;
                                break;
                            }
                            case MessageType.SkipToNext:
                            {
                                skipCycle = true;

                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (NeedsGeneration())
                    {
                        RandomWallpaperSwitch();
                        SaveState();
                        GenerateWallpaper();

                        if (skipCycle)
                        {
                            skipCycle = false;
                        }
                    }
                }
            }

            exited = true;
        }

        static int RoundToInt(double v)
        {
            return (int)Math.Round(v);
        }

        private int ComputeNextInterval()
        {
            if (skipCycle)
            {
                return 0;
            }

            if (cyclePaused)
            {
                return int.MaxValue;
            }

            int nextInterval = int.MaxValue;

            DateTime current = DateTime.Now;
            for (int i = 0; i < monitorStates.Length; i++)
            {
                if (monitorSettings[i].ChangeInterval == 0)
                {
                    continue;
                }

                if (MonitorNeedsGeneration(i, current))
                {
                    nextInterval = 0;
                }
                else
                {
                    nextInterval = Math.Min(nextInterval,
                        (monitorSettings[i].ChangeInterval - RoundToInt((current - monitorStates[i].LastChange).TotalSeconds)) * 1000
                    );
                }
            }

            return nextInterval;
        }

        private bool NeedsGeneration()
        {
            DateTime current = DateTime.Now;
            for (int i = 0; i < monitorStates.Length; i++)
            {
                if (MonitorNeedsGeneration(i, current))
                {
                    return true;
                }
            }

            return false;
        }

        private bool MonitorNeedsGeneration(int index, DateTime time)
        {
            return (monitorSettings[index].Mode == WallpaperMode.Random && skipCycle) ||
                (time - monitorStates[index].LastChange).TotalSeconds > monitorSettings[index].ChangeInterval;
        }

        private void QueueMessage(MessageType msg)
        {
            messageQueue.Enqueue(msg);
            messageEvent.Set();
        }

        enum MessageType
        {
            Stop,
            ReloadConfig,
            PauseCycle,
            ResumeCycle,
            SkipToNext
        }

        string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".tif", ".tiff", ".bmp" };
        IEnumerable<string> GetImagesInDirectory(string path, bool recursive)
        {
            return Directory.EnumerateFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(p => imageExtensions.Contains(Path.GetExtension(p)));
        }

        string GetMonitorWallpaper(MonitorSettings settings)
        {
            if (settings.Mode == WallpaperMode.Single)
            {
                return settings.SingleImage;
            }
            else if (settings.Mode == WallpaperMode.Random)
            {
                var files = settings.RandomSources.SelectMany(
                    fod => !fod.IsDirectory ? new[] { fod.Path } : GetImagesInDirectory(fod.Path, fod.IsRecursive)
                );
                var count = files.Count();
                var index = random.Next(count);
                return files.ElementAt(index);
            }

            return "";
        }

        void RandomWallpaperSwitch()
        {
            DateTime currentTime = DateTime.Now;
            for (int i = 0; i < monitorStates.Length; i++)
            {
                if (monitorSettings[i].Mode == WallpaperMode.Random && MonitorNeedsGeneration(i, currentTime))
                {
                    monitorStates[i].CurrentWallpaper = GetMonitorWallpaper(monitorSettings[i]);
                    monitorStates[i].LastChange = currentTime;

                    WriteToDatabase(i, monitorStates[i]);

                }
                else if (monitorSettings[i].Mode == WallpaperMode.Single)
                {
                    monitorStates[i].CurrentWallpaper = GetMonitorWallpaper(monitorSettings[i]);
                }
            }
        }

        bool IsTablePresent(DataTable dt, string table)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((string)dt.Rows[i].ItemArray[2] == table)
                {
                    return true;
                }
            }

            return false;
        }

        void WriteToDatabase(int index, MonitorState state)
        {
            //using (var conn = new SQLiteConnection("Data Source=history.sqlite"))
            using (var conn = new SQLiteConnection("Data Source='" + GetDatabasePath() + "'"))
            {
                conn.Open();

                DataTable tables = conn.GetSchema("TABLES");
                if (!IsTablePresent(tables, "wallpaper_history"))
                {
                    using (var cmdCreate = new SQLiteCommand(conn))
                    {
                        cmdCreate.CommandText =
@"CREATE TABLE wallpaper_history
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    MonitorIndex INT NOT NULL,
    Timestamp INT NOT NULL,
    Path TEXT NOT NULL
);";
                        cmdCreate.ExecuteNonQuery();
                    }
                }

                using (var cmdInsert = new SQLiteCommand(conn))
                {
                    cmdInsert.CommandText =
@"INSERT INTO wallpaper_history(MonitorIndex, Timestamp, Path) VALUES(@MonitorIndex, @Timestamp, @Path);";

                    cmdInsert.Parameters.Add(new SQLiteParameter("@MonitorIndex", index));
                    cmdInsert.Parameters.Add(new SQLiteParameter("@Timestamp", DateTimeToUnixTimestampMillis(state.LastChange)));
                    cmdInsert.Parameters.Add(new SQLiteParameter("@Path", state.CurrentWallpaper));

                    cmdInsert.ExecuteNonQuery();
                }

                using (var cmdPrune = new SQLiteCommand(conn))
                {
                    cmdPrune.CommandText =
@"DELETE FROM wallpaper_history
WHERE Id IN (SELECT Id FROM wallpaper_history
    LIMIT MAX((SELECT COUNT(*) FROM wallpaper_history) - 1000, 0)
)";
                    cmdPrune.ExecuteNonQuery();
                }
                
            }
        }

        long DateTimeToUnixTimestampMillis(DateTime dateTime)
        {
            return (long)((dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds);
        }

        void GenerateWallpaper()
        {
            var images = new Image[monitorStates.Length];
            for (int i = 0; i < monitorStates.Length; i++)
            {
                var bitmap = new Bitmap(monitorStates[i].CurrentWallpaper);
                bitmap.SetResolution(96, 96);
                images[i] = bitmap;
            }

            var compWallpaper = WallpaperDrawing.GenerateWallpaper(monitors,
                Enumerable.Zip(monitorSettings, images, (set, img) => new { set, img })
                .Select(x => new WallpaperGenSetttings()
                {
                    BackgroundColor = x.set.BackgroundColor,
                    Disposition = x.set.Disposition,
                    Image = x.img
                })
                .ToArray()
            );

            string path = GetGeneratedWallpaperPath();
            compWallpaper.Save(path);
            Utils.EnsureSmoothTransition();
            var iad = IActiveDesktopFactory.CreateInstance();
            if (iad.GetWallpaperStyle() != WallpaperStyle.WPSTYLE_TILE)
            {
                iad.SetWallpaperStyle(WallpaperStyle.WPSTYLE_TILE);
            }
            iad.SetWallpaper(path, 0);
            iad.ApplyChanges(AD_Apply.SAVE);


            compWallpaper.Dispose();
            for (int i = 0; i < images.Length; i++)
            {
                images[i].Dispose();
            }
        }

        Thread changerThread;
        ConcurrentQueue<MessageType> messageQueue;
        AutoResetEvent messageEvent;
        MonitorState[] monitorStates;
        MonitorSettings[] monitorSettings;
        Monitor[] monitors;
        bool running = true;
        bool exited = false;
        Random random = new Random();

        volatile bool cyclePaused = false;
        volatile bool skipCycle = false;
    }

}
