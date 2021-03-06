﻿using UnityEngine;
using UniConsole;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class ConsoleDemoScript : MonoBehaviour
{

    [SerializeField]
    Canvas _canvas = null;

    [SerializeField]
    Button _button = null;

    void Start()
    {
        var model = new ConsoleModel(50, 128);
        var console = ConsolePresenter.Create(model, _canvas.transform);

        // Show console
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            model.Visible.Value = true;
        }).AddTo(this);

        // Subscribe commands
        model.CommandStream.Subscribe(command =>
        {
            Debug.Log(command.ToString());

            if (command.Equals("hello"))
            {
                model.WriteLine("hi!");
                return;
            }

            if (command.Equals("text"))
            {
                model.WriteLine(new List<string> { "aaaaaaaaaa", "bbbbbbbbbb", "cccccccccc"});
                return;
            }

            if (command.Equals("close"))
            {
                model.Visible.Value = false;
                return;
            }

            if (command.Equals("clear"))
            {
                model.Clear();
                return;
            }

        }).AddTo(this);
    }
}
