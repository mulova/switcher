//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [Serializable]
    public class TextData : MaskableGraphicData<Text>
    {
        [Store] public string text;
        [HideInInspector] public bool text_IsSet;
        [Store] public Font font;
        [HideInInspector] public bool font_IsSet;
        [Store] public int fontSize;
        [HideInInspector] public bool fontSize_IsSet;
        [Store] public FontStyle fontStyle;
        [HideInInspector] public bool fontStyle_IsSet;
        [Store] public float lineSpacing;
        [HideInInspector] public bool lineSpacing_IsSet;
        [Store] public bool supportRichText;
        [HideInInspector] public bool supportRichText_IsSet;
        [Store] public TextAnchor alignment;
        [HideInInspector] public bool alignment_IsSet;
        [Store] public bool alignByGeometry;
        [HideInInspector] public bool alignByGeometry_IsSet;
        [Store] public HorizontalWrapMode horizontalOverflow;
        [HideInInspector] public bool horizontalOverflow_IsSet;
        [Store] public VerticalWrapMode verticalOverflow;
        [HideInInspector] public bool verticalOverflow_IsSet;
        [Store] public bool resizeTextForBestFit;
        [HideInInspector] public bool resizeTextForBestFit_IsSet;
    }
}

