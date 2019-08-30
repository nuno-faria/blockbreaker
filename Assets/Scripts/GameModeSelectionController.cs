using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeSelectionController : MonoBehaviour {

    public SpriteRenderer preview;
    public Text modeInfo;
    public Sprite classicSprite;
    public string classicInfo;
    public Sprite modernSprite;
    public string modernInfo;
    public Sprite modernExtraSprite;
    public string modernExtraInfo;
    public Sprite modernHardcoreSprite;
    public string modernHardcoreInfo;


    void Start() {
        FocusClassic();     
    }


    public void FocusClassic() {
        preview.sprite = classicSprite;
        modeInfo.text = classicInfo;
        modeInfo.text += "\n\n" + "Highscore: " + PlayerPrefs.GetInt("classic_highscore");
    }


    public void FocusModern() {
        preview.sprite = modernSprite;
        modeInfo.text = modernInfo;
        modeInfo.text += "\n\n" + "Highscore: " + PlayerPrefs.GetInt("modern_highscore");
    }


    public void FocusExtra() {
        preview.sprite = modernExtraSprite;
        modeInfo.text = modernExtraInfo;
        modeInfo.text += "\n\n" + "Highscore: " + PlayerPrefs.GetInt("large_highscore");
    }


    public void FocusHardcore() {
        preview.sprite = modernHardcoreSprite;
        modeInfo.text = modernHardcoreInfo;
        modeInfo.text += "\n\n" + "Highscore: " + PlayerPrefs.GetInt("hardcore_highscore");
    }
}
