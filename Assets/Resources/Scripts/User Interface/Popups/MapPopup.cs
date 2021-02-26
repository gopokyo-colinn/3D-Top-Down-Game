using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPopup : Popup
{
    public Image playerIcon;
    public Terrain terrain;
    PlayerController player;
    public float fOffsetX;
    public float fOffsetY;
    public Image map;
    public Slider zoomSlider;
    Vector3 defaultSizeDelta;
    private void Start()
    {
        defaultSizeDelta = map.rectTransform.sizeDelta;
    }
    public override void open()
    {
        base.open();
        player = PlayerController.Instance;
        SetPlayerLocation();

        zoomSlider.onValueChanged.AddListener(delegate { ChangeMapScale(); });
       
    }
    void ChangeMapScale()
    {
        map.rectTransform.sizeDelta = defaultSizeDelta * new Vector2(zoomSlider.value, zoomSlider.value);
        playerIcon.rectTransform.localScale = new Vector2(zoomSlider.value, zoomSlider.value);
        SetPlayerLocation();
    }
    void SetPlayerLocation()
    {
        float _fPercentX = (player.transform.position.x / terrain.terrainData.size.x) * 100f;// same as above, just calculations made easy for computers
        float _fPercentY = (player.transform.position.z / terrain.terrainData.size.z) * 100f;// 

        Vector3 _pos = new Vector3((_fPercentX * map.rectTransform.rect.size.x) / 100f, (_fPercentY * map.rectTransform.rect.size.y) / 100f, 0);
        playerIcon.rectTransform.anchoredPosition = _pos;
    }
}
