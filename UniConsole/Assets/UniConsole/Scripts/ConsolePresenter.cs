using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

namespace UniConsole
{

    public class ConsolePresenter : PresenterBase<ConsoleModel>
    {
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

        readonly string Prefix = "$ ";

        readonly string BottomSpace = System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;

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
                    argument.WriteLine(line, Prefix);
                _input.text = "";
            }).AddTo(this);

            var inputTransform = ((RectTransform)_input.transform);
            var inputSize = inputTransform.sizeDelta;
            inputSize.y = argument.FontSize + 20;
            inputTransform.sizeDelta = inputSize;

            var textTransform = ((RectTransform)_text.transform);
            var textSize = inputTransform.offsetMin;
            textSize.y = argument.FontSize + 20;
            textTransform.offsetMin = textSize;

            foreach (var text in _texts)
            {
                text.fontSize = argument.FontSize;
            }
        }

        protected override void Initialize(ConsoleModel argument)
        {
        }

    }

}
