using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleResizeToChild : MonoBehaviour
{
    private RectTransform rtChild;

    private RectTransform rtLastChild;

    private RectTransform rtSelf;

    public int iInactive = 0;

    public int iOffset = 0;

    public int iDivider = 1;

    public int iSpacingPerElement = 0;

    public float fExtraHeight;

    public bool bResizeWidth = false;

    public bool bResizeHeight = true;

    public bool bCountAll = false;

    public bool bManualInactive = false;

    public bool bAlwaysUpdateInactive = false;

    public bool bAsymmetricalAssets = false;

    // Use this for initialization
    void Start()
    {
        if (iInactive == 0)
        {
            init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateSize();
    }

    public void init()
    {
        if (!bManualInactive)
        {
            iInactive = 0;
        }
        if (this.transform.childCount > 0)
        {
            RectTransform _rtChild;
            rtChild = null;
            if (bAsymmetricalAssets)
                fExtraHeight = 0f;

            int i;
            for (i = iOffset; i < this.transform.childCount; i++)
            {
                if (this.transform.GetChild(i).gameObject.activeSelf)
                {
                    _rtChild = this.transform.GetChild(i).GetComponent<RectTransform>();
                    if (bAsymmetricalAssets)
                    {
                        fExtraHeight += _rtChild.rect.height + iSpacingPerElement;
                        rtChild = _rtChild;
                    }
                    else if (rtChild == null)
                    {
                        rtChild = _rtChild;
                    }
                    else if (rtChild.rect.height < _rtChild.rect.height)
                    {
                        rtChild = _rtChild;
                    }
                }
                else
                {
                    iInactive++;
                }
            }
            rtSelf = this.GetComponent<RectTransform>();
        }
    }

    public void updateSize()
    {
        if (this.transform.childCount > 0)
        {
            if (rtSelf == null || rtChild == null || bAsymmetricalAssets)
            {
                init();
            }
            else if (bAlwaysUpdateInactive)
            {
                iInactive = 0;
                if (this.transform.childCount > 0)
                {
                    int _iCpt;
                    for (_iCpt = iOffset; _iCpt < this.transform.childCount; _iCpt++)
                    {
                        if (!this.transform.GetChild(_iCpt).gameObject.activeSelf)
                        {
                            iInactive++;
                        }
                    }
                }
            }

            Vector2 _v2ChatSize = rtSelf.sizeDelta;
            if (bAsymmetricalAssets)
            {
                if (bResizeHeight)
                {
                    if (rtChild != null)
                    {
                        if (bCountAll)
                        {
                            _v2ChatSize.y = fExtraHeight; // Mathf.CeilToInt((float)(this.transform.childCount - iInactive) / (float)iDivider) * (rtChild.rect.height + iSpacingPerElement);
                            if (this.transform.childCount > 1)
                                _v2ChatSize.y -= iSpacingPerElement;
                        }
                        else
                        {
                            _v2ChatSize.y = rtChild.rect.height;
                        }
                    }
                    else
                    {
                        _v2ChatSize.y = 0f;
                    }

                    //_v2ChatSize.y += fExtraHeight;
                }

                if (bResizeWidth)
                {
                    _v2ChatSize.x = 0f;
                }
            }
            else if (rtChild != null)
            {
                if (bResizeHeight)
                {
                    if (bCountAll)
                    {
                        _v2ChatSize.y = Mathf.CeilToInt((float)(this.transform.childCount - iInactive) / (float)iDivider) * (rtChild.rect.height + iSpacingPerElement);
                        if (this.transform.childCount > 1)
                            _v2ChatSize.y -= iSpacingPerElement;
                    }
                    else
                    {
                        _v2ChatSize.y = rtChild.rect.height;
                    }
                    _v2ChatSize.y += fExtraHeight;
                }

                if (bResizeWidth)
                {
                    if (bCountAll)
                    {
                        _v2ChatSize.x = (this.transform.childCount - iInactive) * rtChild.rect.width;
                    }
                    else
                    {
                        _v2ChatSize.x = rtChild.rect.width;
                    }
                }
            }
            else
            {
                if (bResizeHeight)
                {
                    _v2ChatSize.y = fExtraHeight;
                }

                if (bResizeWidth)
                {
                    _v2ChatSize.x = 0f;
                }
            }
            rtSelf.sizeDelta = _v2ChatSize;
        }
    }
}
