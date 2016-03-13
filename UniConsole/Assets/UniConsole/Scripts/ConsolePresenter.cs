using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using System.Linq;

namespace UniConsole
{

    public class ConsolePresenter : PresenterBase<ConsoleModel>
    {
        readonly string Prefix = "$ ";

        readonly string BottomSpace = System.Environment.NewLine + System.Environment.NewLine;

        [SerializeField]
        ScrollRect _scrollRect = null;

        [SerializeField]
        Text _text = null;

        [SerializeField]
        InputField _input = null;

        [SerializeField]
        CanvasGroup _canvasGroup = null;

        [SerializeField]
        Text[] _texts = null;

        [SerializeField]
        int _historySize = 100;

        int _historyIndex;

        List<string> _historyList;

        public static ConsolePresenter Create(ConsoleModel model, Transform parent)
        {
            var resource = Resources.Load("ConsolePrefab");
            var gameObject = GameObject.Instantiate(resource) as GameObject;
            var instance = gameObject.GetComponent<ConsolePresenter>();
            instance.transform.SetParent(parent, false);
            instance._canvasGroup.alpha = 0;
            instance.ForceInitialize(model);
            return instance;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_historyIndex >= 0)
                {
                    _input.text = _historyList[_historyIndex];
                    if (_historyIndex != 0)
                        _historyIndex--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_historyIndex < _historyList.Count)
                {
                    _input.text = _historyList[_historyIndex];
                    if (_historyIndex != _historyList.Count - 1)
                        _historyIndex++;
                }
            }
        }

        protected override IPresenter[] Children
        {
            get
            {
                return new IPresenter[] { };
            }
        }

        protected override void BeforeInitialize(ConsoleModel argument)
        {
            argument.Visible.Subscribe(visible =>
            {
                if (visible)
                {
                    _canvasGroup.alpha = 1;
                    _canvasGroup.blocksRaycasts = true;
                    EventSystem.current.SetSelectedGameObject(_input.gameObject);
                }
                else
                {
                    _canvasGroup.alpha = 0;
                    _canvasGroup.blocksRaycasts = false;
                }
            }).AddTo(this);

            argument.Text.Subscribe(text =>
            {
                _text.text = text + BottomSpace;
                _scrollRect.normalizedPosition = new Vector2(0, 0);
                _input.ActivateInputField();
            }
            ).AddTo(this); ;

            _input.OnEndEditAsObservable().Subscribe(line =>
            {
                if (!string.IsNullOrEmpty(line))
                {
                    argument.WriteLine(Prefix + line);
                    argument.ParseCommand(line);

                    // Do not add a duplicated line.
                    if (_historyList.Max() == line)
                        return;

                    _historyList.Add(line);

                    if (_historyList.Count > _historySize)
                        _historyList = _historyList.GetRange(_historyList.Count - _historySize, _historySize);

                    _historyIndex = _historyList.Count - 1;
                }
                _input.text = "";
            }).AddTo(this);

            var inputTransform = ((RectTransform)_input.transform);
            var inputSize = inputTransform.sizeDelta;
            inputSize.y = argument.FontSize + 20;
            inputTransform.sizeDelta = inputSize;

            var scrollViewTransform = ((RectTransform)_scrollRect.transform);
            var scrollViewOffsetMin = inputTransform.offsetMin;
            scrollViewOffsetMin.y = argument.FontSize + 20;
            scrollViewTransform.offsetMin = scrollViewOffsetMin;

            foreach (var text in _texts)
            {
                text.fontSize = argument.FontSize;
            }

            _historyList = new List<string>();
        }

        protected override void Initialize(ConsoleModel argument)
        {
        }

    }

}
