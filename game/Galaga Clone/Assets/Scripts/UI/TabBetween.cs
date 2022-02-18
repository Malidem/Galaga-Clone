using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class TabBetween : MonoBehaviour
{
    public InputField nextInputField;
    private InputField currentInputField;

    // Start is called before the first frame update
    void Start()
    {
        if (nextInputField == null)
        {
            Destroy(this);
            return;
        }
        currentInputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInputField.isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            nextInputField.ActivateInputField();
        }
    }
}
