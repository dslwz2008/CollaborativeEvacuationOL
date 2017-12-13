using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PasswordInputFieldState : MonoBehaviour
{
    private Pvr_UnitySDKSightInputModule sim;
    private InputField inputField;

    void Start()
    {
        sim = FindObjectOfType<Pvr_UnitySDKSightInputModule>();
        inputField = GetComponent<InputField>();
    }

    void Update()
    {
        sim.passwordInputFieldFocused = inputField.isFocused;
    }
}
