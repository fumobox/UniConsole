using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UniConsole
{

    public class ScrollBarPresenter : MonoBehaviour {

        [SerializeField]
        ConsolePresenter _console;

        RectTransform _rt;

        [SerializeField]
        Image _foregroundImage;
        RectTransform _frt;

        #region UnityEngine

        void Start () {
        }
    
        void Update () {
            /*
            if (_rt == null)
                _rt = GetComponent<RectTransform>();

            var bh = _console.TextAreaHeight - 10;
            _rt.sizeDelta = new Vector2(10, bh);

            if (_frt == null)
                _frt = _foregroundImage.GetComponent<RectTransform>();

            if (_console.LineCount <= _console.MaxLine)
            {
                _frt.sizeDelta = _rt.sizeDelta;
                _frt.anchoredPosition = _rt.anchoredPosition;
            }
            else
            {
                float buh = bh / _console.LineCount;

                float fy = _console.LineIndex * buh;
                float fh = buh * _console.MaxLine - fy;

                _frt.sizeDelta = new Vector2(10, fh);
                _frt.anchoredPosition = new Vector2(0, fy);
            }
            */
        }

        #endregion

    }

}
