﻿using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class NeedyQuizComponentSolver : ComponentSolver
{
    public NeedyQuizComponentSolver(BombCommander bombCommander, MonoBehaviour bombComponent, IRCConnection ircConnection, CoroutineCanceller canceller) :
        base(bombCommander, bombComponent, ircConnection, canceller)
    {
        _yesButton = (MonoBehaviour)_yesButtonField.GetValue(bombComponent.GetComponent(_componentSolverType));
        _noButton = (MonoBehaviour)_noButtonField.GetValue(bombComponent.GetComponent(_componentSolverType));
        _display = (TextMesh) _displayField.GetValue(bombComponent.GetComponent(_componentSolverType));

        _service = (KMGameCommands) _serviceField.GetValue(bombComponent.GetComponent(_componentSolverType));
        modInfo = ComponentSolverFactory.GetModuleInfo(GetModuleType());
    }

    protected override IEnumerator RespondToCommandInternal(string inputCommand)
    {
        if (inputCommand.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
            inputCommand.Equals("yes", StringComparison.InvariantCultureIgnoreCase) ||
            inputCommand.Equals("press y", StringComparison.InvariantCultureIgnoreCase) ||
            inputCommand.Equals("press yes", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return "yes";

            if (_display.text.Equals("Abort?"))
            {
                yield return "sendtochat ABORT! ABORT!!! ABOOOOOOORT!!!!!";
                int strikeCount = 0;
                yield return "multiple strikes";
                while (!Detonated)
                {
                    strikeCount++;
                    _service.CauseStrike("ABORT!");
                }
                yield return "award strikes " + strikeCount;
                yield break;
            }
            yield return DoInteractionClick(_yesButton);
        }
        else if (inputCommand.Equals("n", StringComparison.InvariantCultureIgnoreCase) ||
                 inputCommand.Equals("no", StringComparison.InvariantCultureIgnoreCase) ||
                 inputCommand.Equals("press n", StringComparison.InvariantCultureIgnoreCase) ||
                 inputCommand.Equals("press no", StringComparison.InvariantCultureIgnoreCase))
        {
            yield return "no";
            yield return DoInteractionClick(_noButton);
        }
    }

    static NeedyQuizComponentSolver()
    {
        _componentSolverType = ReflectionHelper.FindType("AdvancedVentingGas");
        _yesButtonField = _componentSolverType.GetField("YesButton", BindingFlags.Public | BindingFlags.Instance);
        _noButtonField = _componentSolverType.GetField("NoButton", BindingFlags.Public | BindingFlags.Instance);
        _displayField = _componentSolverType.GetField("Display", BindingFlags.Public | BindingFlags.Instance);
        _serviceField = _componentSolverType.GetField("Service", BindingFlags.Public | BindingFlags.Instance);
    }

    private static Type _componentSolverType = null;
    private static FieldInfo _yesButtonField = null;
    private static FieldInfo _noButtonField = null;
    private static FieldInfo _displayField = null;
    private static FieldInfo _serviceField = null;

    private MonoBehaviour _yesButton = null;
    private MonoBehaviour _noButton = null;
    private TextMesh _display = null;
    private KMGameCommands _service = null;
}
