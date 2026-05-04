using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject Menu;

    [Header("Панель")]
    [SerializeField] private Image SolidColorPanel;
    [SerializeField] private float PanelSwitchSpeed;

    [Header("Звуки")]
    [SerializeField] private AudioClip UpgradeSound;
    [SerializeField] private AudioSource GameMusic;
    [SerializeField] private float MusicSwitchSpeed;
    [SerializeField] private AudioSource MenuAudioPlayer;

    [Header("Улучшения")]
    [SerializeField] private GameObject UpgradeMenu;
    [SerializeField] private float UpgradeMusicPitch;
    
    [Header("Способности")]
    [SerializeField] private GameObject[] AllSpecials;
    [SerializeField] private GameObject SpecialMenu;

    private Coroutine _currentSwitchSolidPanel;
    private Coroutine _currentSwitchMusicPitch;

    private void Awake()
    {

        Time.timeScale = 1;
        PlayerController.PlayerDead += OnPlayerDead;
        UpgradeBox.UpgradeTaken += OnUpgradeMenu;
        SwitchSolidPanel(true);
        SwitchMusicPitch(1);
    }

    private void SwitchSolidPanel(bool mode)
    {
        if (_currentSwitchSolidPanel != null)
            StopCoroutine(_currentSwitchSolidPanel);
        _currentSwitchSolidPanel = StartCoroutine(SwitchSolidPanel_(mode));
    }

    private IEnumerator SwitchSolidPanel_(bool mode)
    {
        SolidColorPanel.gameObject.SetActive(true);
        bool b = true;
        int modeModifier;
        switch (mode)
        {
            case false:
                SolidColorPanel.color = new Color(0, 0, 0, 0);
                modeModifier = 1;
                break;
            default:
                SolidColorPanel.color = new Color(0, 0, 0, 1);
                modeModifier = -1;
                break;
        }
        while (b)
        {
            switch (mode)
            {
                case false:
                    b = SolidColorPanel.color.a < 1;
                    break;
                default:
                    b = SolidColorPanel.color.a > 0;
                    break;
            }
            SolidColorPanel.color += new Color(0, 0, 0, Time.unscaledDeltaTime * modeModifier * PanelSwitchSpeed);
            yield return null;
        }
        if (!mode) yield break;
        SolidColorPanel.gameObject.SetActive(false);
    }

    private void SwitchMusicPitch(float to)
    {
        if (_currentSwitchMusicPitch != null)
            StopCoroutine(_currentSwitchMusicPitch);
        _currentSwitchMusicPitch = StartCoroutine(SwitchMusicPitch_(to));
    }

    private IEnumerator SwitchMusicPitch_(float to)
    {
        while (!Mathf.Approximately(GameMusic.pitch, to))
        {
            GameMusic.pitch = Mathf.Lerp(GameMusic.pitch, to, Time.unscaledDeltaTime * MusicSwitchSpeed);
            yield return null;
        }
        GameMusic.pitch = to;
    }

    public void OnUpgradeMenu(UpgradeBox.UpgradeType type)
    {
        Time.timeScale = 0;
        SwitchMusicPitch(UpgradeMusicPitch);
        MenuAudioPlayer.PlayOneShot(UpgradeSound);
        if (type == UpgradeBox.UpgradeType.Статы)
            UpgradeMenu.SetActive(true);
        else
        {
            GenerateSpecials();
            SpecialMenu.SetActive(true);
        }
    }

    private void GenerateSpecials()
    {
        List<GameObject> have = new();
        int reqCards;
        if (AllSpecials.Length > 2)
            reqCards = 3;
        else
            reqCards = AllSpecials.Length;
        for (int i = 0; i < reqCards; i++)
        {
            while (true)
            {
                GameObject special = AllSpecials[Random.Range(0, AllSpecials.Length)];
                if (have.Contains(special)) continue;
                special.SetActive(true);
                have.Add(special);
                break;
            }
        }
    }

    public void OffUpgradeMenu()
    {
        SwitchMusicPitch(1);
        Time.timeScale = 1;
        foreach (GameObject special in AllSpecials)
            special.SetActive(false);
    }

    private void OnPlayerDead()
    {

        SwitchMusicPitch(0);
        SwitchSolidPanel(false);
        StartCoroutine(WaitMusic());
    }

    private IEnumerator WaitMusic()
    {

        yield return new WaitWhile(() => GameMusic.pitch > 0.1f);
        SwitchScene.LoadScene("Menu");
    }

    private void OnDestroy()
    {

        UpgradeBox.UpgradeTaken -= OnUpgradeMenu;
        PlayerController.PlayerDead -= OnPlayerDead;
    }
}