using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;

public class NameTagPage: MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI timeField;

    public void InitiateKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
    public string GetInputFieldName()
    {
        return inputField.text;
    }

    public void SetEndTime(string time)
    {
        timeField.text = time;
    }
}
