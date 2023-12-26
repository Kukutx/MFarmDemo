using UnityEngine;
using System;
using MFarm.Save;
using System.Collections.Generic;
/// <summary>
/// 时间管理
/// </summary>
public class TimeManager : Singleton<TimeManager>,ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;     // 秒，分，时，天，月，年
    private Season gameSeason = Season.春天;//默认游戏进入为春天
    private int monthInSeason = 3;//三个月为一个季节，判断每三个月一个季节
    public bool gameClockPause;//定义一个布尔值来控制游戏的暂停   (这个也可以私有化毕竟可能用不上)
    private float tikTime;          //计时器
    private float timeDifference; //灯光时间差
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);     // 当前游戏时间的时间戳

    public string GUID => GetComponent<DataGUID>().guid;     // 生成唯一标识码

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;        // 这边是因为加载场景和卸载场景的时候会有3秒的延迟所以在这段事件内我需要暂停时间直到加载完毕
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;          // 这里也是
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
    }

    private void Update()
    {
        if (!gameClockPause)        // 没有暂停
        {
            tikTime += Time.deltaTime;      //编写一个秒针计时器
            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
        // 作弊命令(用于时间加快，以后可以用于其他)
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)        // 每次按下快一分钟
            {
                UpdateGameTime();
            }
        }
        //作弊命令(天数增加)
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }

    private void OnStartNewGameEvent(int index)
    {
        NewGameTime();
        gameClockPause = false;
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.Pause;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false;

        // 这样方便初始化时间
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);  // CallGameDateSeason
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameSeason, gameDay);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }

    /// <summary>
    /// 新开一局游戏时给游戏初始化赋值
    /// </summary>
    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;//从早上七点开始游戏
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2023;
        gameSeason = Season.春天;
    }

    /// <summary>
    /// 更新游戏时间,秒分年月日依次递进
    /// </summary>
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                            gameMonth = 1;

                        monthInSeason--;

                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;

                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;

                            if (gameYear > 9999)
                            {
                                gameYear = 2023;        // 当年超过一个不太可能的年份我觉得还是返回今年比较好，或者也可以是 0 年，随便都行
                            }
                        }
                        //用来刷新地图和农作物生长
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                //每时间执行到此位置，调用一下委托时间
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason); /*CallGameDateSeason*/ //这里需要调用一下委托事件
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameSeason, gameDay);//这里需要调用一下委托事件
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }

    /// <summary>
    /// 返回LightShift同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            Debug.Log(timeDifference);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }

    /// <summary>
    /// 生成保存数据
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);
        return saveData;
    }

    /// <summary>
    /// 恢复数据
    /// </summary>
    /// <param name="saveDate"></param>
    public void RestoreData(GameSaveData saveDate)
    {
        gameYear = saveDate.timeDict["gameYear"];
        gameSeason = (Season)saveDate.timeDict["gameSeason"];
        gameMonth = saveDate.timeDict["gameMonth"];
        gameDay = saveDate.timeDict["gameDay"];
        gameHour = saveDate.timeDict["gameHour"];
        gameMinute = saveDate.timeDict["gameMinute"];
        gameSecond = saveDate.timeDict["gameSecond"];
    }
}