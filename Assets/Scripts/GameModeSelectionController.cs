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
    }


    public void FocusModern() {
        preview.sprite = modernSprite;
        modeInfo.text = modernInfo;
    }


    public void FocusExtra() {
        preview.sprite = modernExtraSprite;
        modeInfo.text = modernExtraInfo;
    }


    public void FocusHardcore() {
        preview.sprite = modernHardcoreSprite;
        modeInfo.text = modernHardcoreInfo;
    }
}
