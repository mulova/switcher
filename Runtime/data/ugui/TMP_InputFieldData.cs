//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static TMPro.TMP_InputField;
    using CharacterValidation = TMPro.TMP_InputField.CharacterValidation;
    using ContentType = TMPro.TMP_InputField.ContentType;
    using InputType = TMPro.TMP_InputField.InputType;
    using LineType = TMPro.TMP_InputField.LineType;

    public class TMP_InputFieldData : SelectableData<TMP_InputField>
    {
        [Store] public bool readOnly;
        [HideInInspector] public bool readOnly_mod;
        [Store] public bool richText;
        [HideInInspector] public bool richText_mod;
        [Store] public bool multiLine;
        [HideInInspector] public bool multiLine_mod;
        [Store] public char asteriskChar;
        [HideInInspector] public bool asteriskChar_mod;
        [Store] public bool shouldHideMobileInput;
        [HideInInspector] public bool shouldHideMobileInput_mod;
        [Store] public bool shouldHideSoftKeyboard;
        [HideInInspector] public bool shouldHideSoftKeyboard_mod;
        [Store] public string text;
        [HideInInspector] public bool text_mod;
        [Store] public float caretBlinkRate;
        [HideInInspector] public bool caretBlinkRate_mod;
        [Store] public int caretWidth;
        [HideInInspector] public bool caretWidth_mod;
        [Store] public RectTransform textViewport;
        [HideInInspector] public bool textViewport_mod;
        [Store] public TMP_Text textComponent;
        [HideInInspector] public bool textComponent_mod;
        [Store] public Graphic placeholder;
        [HideInInspector] public bool placeholder_mod;
        [Store] public Scrollbar verticalScrollbar;
        [HideInInspector] public bool verticalScrollbar_mod;
        [Store] public float scrollSensitivity;
        [HideInInspector] public bool scrollSensitivity_mod;
        [Store] public Color caretColor;
        [HideInInspector] public bool caretColor_mod;
        [Store] public bool customCaretColor;
        [HideInInspector] public bool customCaretColor_mod;
        [Store] public Color selectionColor;
        [HideInInspector] public bool selectionColor_mod;
        [Store] public SubmitEvent onEndEdit;
        [HideInInspector] public bool onEndEdit_mod;
        [Store] public SubmitEvent onSubmit;
        [HideInInspector] public bool onSubmit_mod;
        [Store] public SelectionEvent onSelect;
        [HideInInspector] public bool onSelect_mod;
        [Store] public SelectionEvent onDeselect;
        [HideInInspector] public bool onDeselect_mod;
        [Store] public TextSelectionEvent onTextSelection;
        [HideInInspector] public bool onTextSelection_mod;
        [Store] public TextSelectionEvent onEndTextSelection;
        [HideInInspector] public bool onEndTextSelection_mod;
        [Store] public OnChangeEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_mod;
        [Store] public TouchScreenKeyboardEvent onTouchScreenKeyboardStatusChanged;
        [HideInInspector] public bool onTouchScreenKeyboardStatusChanged_mod;
        [Store] public OnValidateInput onValidateInput;
        [HideInInspector] public bool onValidateInput_mod;
        [Store] public int characterLimit;
        [HideInInspector] public bool characterLimit_mod;

        [Store] public float pointSize;
        [HideInInspector] public bool pointSize_mod;
        [Store] public TMP_FontAsset fontAsset;
        [HideInInspector] public bool fontAsset_mod;
        [Store] public bool onFocusSelectAll;
        [HideInInspector] public bool onFocusSelectAll_mod;
        [Store] public bool resetOnDeActivation;
        [HideInInspector] public bool resetOnDeActivation_mod;

        [Store] public bool restoreOriginalTextOnEscape;
        [HideInInspector] public bool restoreOriginalTextOnEscape_mod;
        [Store] public bool isRichTextEditingAllowed;
        [HideInInspector] public bool isRichTextEditingAllowed_mod;
        [Store] public ContentType contentType;
        [HideInInspector] public bool contentType_mod;

        [Store] public LineType lineType;
        [HideInInspector] public bool lineType_mod;
        [Store] public int lineLimit;
        [HideInInspector] public bool lineLimit_mod;
        [Store] public InputType inputType;
        [HideInInspector] public bool inputType_mod;

        [Store] public TouchScreenKeyboardType keyboardType;
        [HideInInspector] public bool keyboardType_mod;
        [Store] public CharacterValidation characterValidation;
        [HideInInspector] public bool characterValidation_mod;

        [Store] public TMP_InputValidator inputValidator;
        [HideInInspector] public bool inputValidator_mod;

        protected override bool IsCollectable(MemberControl m, Component c)
        {
            switch (m.name)
            {
                case nameof(caretColor):
                    return textComponent != null;
                default:
                    return true;
            }
        }
    }
}

