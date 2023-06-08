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
        [HideInInspector] public bool text_mod;
        [Store] public Font font;
        [HideInInspector] public bool font_mod;
        [Store] public int fontSize;
        [HideInInspector] public bool fontSize_mod;
        [Store] public FontStyle fontStyle;
        [HideInInspector] public bool fontStyle_mod;
        [Store] public float lineSpacing;
        [HideInInspector] public bool lineSpacing_mod;
        [Store] public bool supportRichText;
        [HideInInspector] public bool supportRichText_mod;
        [Store] public TextAnchor alignment;
        [HideInInspector] public bool alignment_mod;
        [Store] public bool alignByGeometry;
        [HideInInspector] public bool alignByGeometry_mod;
        [Store] public HorizontalWrapMode horizontalOverflow;
        [HideInInspector] public bool horizontalOverflow_mod;
        [Store] public VerticalWrapMode verticalOverflow;
        [HideInInspector] public bool verticalOverflow_mod;
        [Store] public bool resizeTextForBestFit;
        [HideInInspector] public bool resizeTextForBestFit_mod;
    }
}

