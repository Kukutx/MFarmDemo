using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 界面管理,这个是目前唯一一个不在Manager面板下的Manager,因为需要获取其他UI的参数所以被挂在在Main Canvas上
/// </summary>
public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;  // 主菜单的Canvas
    public GameObject menuPrefab;   // 主菜单界面的预制体，这个是用来在menuCanvas里方便加载和卸载的
    public Button settingsBtn;      // 游戏设置按钮，这个在Game Time这个UI面板下可以看到
    public GameObject pausePanel;   // 暂停菜单
    public Slider volumeSlider;     // 音量滑条

    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);          // 启动暂停菜单
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume); // 监听音量滑条的变化
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (menuCanvas.transform.childCount > 0)
        {
            Destroy(menuCanvas.transform.GetChild(0).gameObject);   // 销毁canvas下的第一个物体，某个暂时不需要的ui界面
        }
    }

    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    /// <summary>
    /// 启动暂停菜单
    /// </summary>
    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;//判断当前GameObject是否在Hierarchy面板中是激活状态
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            System.GC.Collect();//在游戏暂停时强制进行垃圾回收,提高游戏性能
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 返回游戏主菜单，即标题
    /// </summary>
    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }

    /// <summary>
    /// 回到主菜单的携程
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();            // 添加这个在回到主菜单会保存玩家的进度和有过场动画(这是TransitionManager上的CallEndGameEvent带的可以将UnloadScene修改如果不喜欢)
        yield return new WaitForSeconds(1.0f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void GameEndEvent()
    {
        Application.Quit();
    }
}
