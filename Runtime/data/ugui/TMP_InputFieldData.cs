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
        [HideInInspector] public bool readOnly_IsSet;
        [Store] public bool richText;
        [HideInInspector] public bool richText_IsSet;
        [Store] public bool multiLine;
        [HideInInspector] public bool multiLine_IsSet;
        [Store] public char asteriskChar;
        [HideInInspector] public bool asteriskChar_IsSet;
        [Store] public bool shouldHideMobileInput;
        [HideInInspector] public bool shouldHideMobileInput_IsSet;
        [Store] public bool shouldHideSoftKeyboard;
        [HideInInspector] public bool shouldHideSoftKeyboard_IsSet;
        [Store] public string text;
        [HideInInspector] public bool text_IsSet;
        [Store] public float caretBlinkRate;
        [HideInInspector] public bool caretBlinkRate_IsSet;
        [Store] public int caretWidth;
        [HideInInspector] public bool caretWidth_IsSet;
        [Store] public RectTransform textViewport;
        [HideInInspector] public bool textViewport_IsSet;
        [Store] public TMP_Text textComponent;
        [HideInInspector] public bool textComponent_IsSet;
        [Store] public Graphic placeholder;
        [HideInInspector] public bool placeholder_IsSet;
        [Store] public Scrollbar verticalScrollbar;
        [HideInInspector] public bool verticalScrollbar_IsSet;
        [Store] public float scrollSensitivity;
        [HideInInspector] public bool scrollSensitivity_IsSet;
        [Store] public Color caretColor;
        [HideInInspector] public bool caretColor_IsSet;
        [Store] public bool customCaretColor;
        [HideInInspector] public bool customCaretColor_IsSet;
        [Store] public Color selectionColor;
        [HideInInspector] public bool selectionColor_IsSet;
        [Store] public SubmitEvent onEndEdit;
        [HideInInspector] public bool onEndEdit_IsSet;
        [Store] public SubmitEvent onSubmit;
        [HideInInspector] public bool onSubmit_IsSet;
        [Store] public SelectionEvent onSelect;
        [HideInInspector] public bool onSelect_IsSet;
        [Store] public SelectionEvent onDeselect;
        [HideInInspector] public bool onDeselect_IsSet;
        [Store] public TextSelectionEvent onTextSelection;
        [HideInInspector] public bool onTextSelection_IsSet;
        [Store] public TextSelectionEvent onEndTextSelection;
        [HideInInspector] public bool onEndTextSelection_IsSet;
        [Store] public OnChangeEvent onValueChanged;
        [HideInInspector] public bool onValueChanged_IsSet;
        [Store] public TouchScreenKeyboardEvent onTouchScreenKeyboardStatusChanged;
        [HideInInspector] public bool onTouchScreenKeyboardStatusChanged_IsSet;
        [Store] public OnValidateInput onValidateInput;
        [HideInInspector] public bool onValidateInput_IsSet;
        [Store] public int characterLimit;
        [HideInInspector] public bool characterLimit_IsSet;

        [Store] public float pointSize;
        [HideInInspector] public bool pointSize_IsSet;
        [Store] public TMP_FontAsset fontAsset;
        [HideInInspector] public bool fontAsset_IsSet;
        [Store] public bool onFocusSelectAll;
        [HideInInspector] public bool onFocusSelectAll_IsSet;
        [Store] public bool resetOnDeActivation;
        [HideInInspector] public bool resetOnDeActivation_IsSet;

        [Store] public bool restoreOriginalTextOnEscape;
        [HideInInspector] public bool restoreOriginalTextOnEscape_IsSet;
        [Store] public bool isRichTextEditingAllowed;
        [HideInInspector] public bool isRichTextEditingAllowed_IsSet;
        [Store] public ContentType contentType;
        [HideInInspector] public bool contentType_IsSet;

        [Store] public LineType lineType;
        [HideInInspector] public bool lineType_IsSet;
        [Store] public int lineLimit;
        [HideInInspector] public bool lineLimit_IsSet;
        [Store] public InputType inputType;
        [HideInInspector] public bool inputType_IsSet;

        [Store] public TouchScreenKeyboardType keyboardType;
        [HideInInspector] public bool keyboardType_IsSet;
        [Store] public CharacterValidation characterValidation;
        [HideInInspector] public bool characterValidation_IsSet;

        [Store] public TMP_InputValidator inputValidator;
        [HideInInspector] public bool inputValidator_IsSet;

        protected override void CollectMember(MemberControl m, Component c, Transform rc, Transform r0)
        {
            if (textComponent == null && m.name == nameof(caretColor))
            {
                return;
            }
            base.CollectMember(m, c, rc, r0);
        }
    }
}

