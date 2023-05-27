//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace mulova.switcher
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngine.UI;

    [Serializable, Preserve]
    public class TextData : GraphicData<Text>
    {
        [Store, Preserve] public bool maskable;
        [HideInInspector, Preserve] public bool maskable_IsSet;
        [Store, Preserve] public string text;
        [HideInInspector, Preserve] public bool text_IsSet;
        [Store, Preserve] public Font font;
        [HideInInspector, Preserve] public bool font_IsSet;
        [Store, Preserve] public int fontSize;
        [HideInInspector, Preserve] public bool fontSize_IsSet;
        [Store, Preserve] public FontStyle fontStyle;
        [HideInInspector, Preserve] public bool fontStyle_IsSet;
        [Store, Preserve] public float lineSpacing;
        [HideInInspector, Preserve] public bool lineSpacing_IsSet;
        [Store, Preserve] public bool supportRichText;
        [HideInInspector, Preserve] public bool supportRichText_IsSet;
        [Store, Preserve] public TextAnchor alignment;
        [HideInInspector, Preserve] public bool alignment_IsSet;
        [Store, Preserve] public bool alignByGeometry;
        [HideInInspector, Preserve] public bool alignByGeometry_IsSet;
        [Store, Preserve] public HorizontalWrapMode horizontalOverflow;
        [HideInInspector, Preserve] public bool horizontalOverflow_IsSet;
        [Store, Preserve] public VerticalWrapMode verticalOverflow;
        [HideInInspector, Preserve] public bool verticalOverflow_IsSet;
        [Store, Preserve] public bool resizeTextForBestFit;
        [HideInInspector, Preserve] public bool resizeTextForBestFit_IsSet;
    }
}

