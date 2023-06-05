//----------------------------------------------
// Unity3D UI switch library
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright mulova@gmail.com
//----------------------------------------------

namespace Studio.Common.Ui
{
    using System;
    using mulova.switcher;
    using TMPro;
    using UnityEngine;

    [Serializable]
	public class TextMeshProUGUIData : GraphicData<TextMeshProUGUI>
	{
		[Store] public bool maskable;
		[HideInInspector] public bool maskable_IsSet;
		[Store] public string text;
		[HideInInspector] public bool text_IsSet;
		[Store] public bool isRightToLeftText;
		[HideInInspector] public bool isRightToLeftText_IsSet;

        [Store] public TMP_FontAsset font;
        [HideInInspector] public bool font_IsSet;
        [Store] public bool enableVertexGradient;
        [HideInInspector] public bool enableVertexGradient_IsSet;
        [Store] public VertexGradient colorGradient;
        [HideInInspector] public bool colorGradient_IsSet;
        [Store] public TMP_ColorGradient colorGradientPreset;
        [HideInInspector] public bool colorGradientPreset_IsSet;
        [Store] public TMP_SpriteAsset spriteAsset;
        [HideInInspector] public bool spriteAsset_IsSet;
        [Store] public bool tintAllSprites;
        [HideInInspector] public bool tintAllSprites_IsSet;
        [Store] public TMP_StyleSheet styleSheet;
        [HideInInspector] public bool styleSheet_IsSet;
        [Store] public TMP_Style textStyle;
        [HideInInspector] public bool textStyle_IsSet;
        [Store] public bool overrideColorTags;
        [HideInInspector] public bool overrideColorTags_IsSet;
        [Store] public Color32 faceColor;
        [HideInInspector] public bool faceColor_IsSet;
        [Store] public Color32 outlineColor;
        [HideInInspector] public bool outlineColor_IsSet;
        [Store] public float outlineWidth;
        [HideInInspector] public bool outlineWidth_IsSet;
        [Store] public float fontSize;
        [HideInInspector] public bool fontSize_IsSet;
        [Store] public FontWeight fontWeight;
        [HideInInspector] public bool fontWeight_IsSet;
        [Store] public bool enableAutoSizing;
        [HideInInspector] public bool enableAutoSizing_IsSet;
        [Store] public float fontSizeMin;
        [HideInInspector] public bool fontSizeMin_IsSet;
        [Store] public float fontSizeMax;
        [HideInInspector] public bool fontSizeMax_IsSet;
        [Store] public FontStyles fontStyle;
        [HideInInspector] public bool fontStyle_IsSet;
        [Store] public HorizontalAlignmentOptions horizontalAlignment;
        [HideInInspector] public bool horizontalAlignment_IsSet;
        [Store] public VerticalAlignmentOptions verticalAlignment;
        [HideInInspector] public bool verticalAlignment_IsSet;
        [Store] public TextAlignmentOptions alignment;
        [HideInInspector] public bool alignment_IsSet;
        [Store] public float characterSpacing;
        [HideInInspector] public bool characterSpacing_IsSet;
        [Store] public float wordSpacing;
        [HideInInspector] public bool wordSpacing_IsSet;
        [Store] public float lineSpacing;
        [HideInInspector] public bool lineSpacing_IsSet;
        [Store] public float lineSpacingAdjustment;
        [HideInInspector] public bool lineSpacingAdjustment_IsSet;
        [Store] public float paragraphSpacing;
        [HideInInspector] public bool paragraphSpacing_IsSet;
        [Store] public float characterWidthAdjustment;
        [HideInInspector] public bool characterWidthAdjustment_IsSet;
        [Store] public bool enableWordWrapping;
        [HideInInspector] public bool enableWordWrapping_IsSet;
        [Store] public float wordWrappingRatios;
        [HideInInspector] public bool wordWrappingRatios_IsSet;
        [Store] public TextOverflowModes overflowMode;
        [HideInInspector] public bool overflowMode_IsSet;
        [Store] public TMP_Text linkedTextComponent;
        [HideInInspector] public bool linkedTextComponent_IsSet;
        [Store] public bool enableKerning;
        [HideInInspector] public bool enableKerning_IsSet;
        [Store] public bool extraPadding;
        [HideInInspector] public bool extraPadding_IsSet;
        [Store] public bool richText;
        [HideInInspector] public bool richText_IsSet;
        [Store] public bool parseCtrlCharacters;
        [HideInInspector] public bool parseCtrlCharacters_IsSet;
        [Store] public bool isOverlay;
        [HideInInspector] public bool isOverlay_IsSet;
        [Store] public bool isOrthographic;
        [HideInInspector] public bool isOrthographic_IsSet;
        [Store] public bool enableCulling;
        [HideInInspector] public bool enableCulling_IsSet;
        [Store] public bool ignoreVisibility;
        [HideInInspector] public bool ignoreVisibility_IsSet;
        [Store] public TextureMappingOptions horizontalMapping;
        [HideInInspector] public bool horizontalMapping_IsSet;
        [Store] public TextureMappingOptions verticalMapping;
        [HideInInspector] public bool verticalMapping_IsSet;
        [Store] public float mappingUvLineOffset;
        [HideInInspector] public bool mappingUvLineOffset_IsSet;
        [Store] public TextRenderFlags renderMode;
        [HideInInspector] public bool renderMode_IsSet;
        [Store] public VertexSortingOrder geometrySortingOrder;
        [HideInInspector] public bool geometrySortingOrder_IsSet;
        [Store] public bool isTextObjectScaleStatic;
        [HideInInspector] public bool isTextObjectScaleStatic_IsSet;
        [Store] public bool vertexBufferAutoSizeReduction;
        [HideInInspector] public bool vertexBufferAutoSizeReduction_IsSet;
        [Store] public int firstVisibleCharacter;
        [HideInInspector] public bool firstVisibleCharacter_IsSet;
        [Store] public int maxVisibleCharacters;
        [HideInInspector] public bool maxVisibleCharacters_IsSet;
        [Store] public int maxVisibleWords;
        [HideInInspector] public bool maxVisibleWords_IsSet;
        [Store] public int maxVisibleLines;
        [HideInInspector] public bool maxVisibleLines_IsSet;
        [Store] public bool useMaxVisibleDescender;
        [HideInInspector] public bool useMaxVisibleDescender_IsSet;
        [Store] public int pageToDisplay;
        [HideInInspector] public bool pageToDisplay_IsSet;
        [Store] public Vector4 margin;
        [HideInInspector] public bool margin_IsSet;
        [Store] public bool isUsingLegacyAnimationComponent;
        [HideInInspector] public bool isUsingLegacyAnimationComponent_IsSet;
        [Store] public bool autoSizeTextContainer;
        [HideInInspector] public bool autoSizeTextContainer_IsSet;
        [Store] public bool isVolumetricText;
        [HideInInspector] public bool isVolumetricText_IsSet;

        //public bool isUsingBold => m_isUsingBold;
        //public int firstOverflowCharacterIndex => m_firstOverflowCharacterIndex;
        //public bool isTextTruncated => m_isTextTruncated;
        //public TMP_TextInfo textInfo => m_textInfo;
        //public float flexibleHeight => m_flexibleHeight;
        //public float flexibleWidth => m_flexibleWidth;
        //public float minWidth => m_minWidth;
        //public float minHeight => m_minHeight;
        //public float maxWidth => m_maxWidth;
        //public float maxHeight => m_maxHeight;
        //public virtual float renderedWidth => GetRenderedWidth();
        //public virtual float renderedHeight => GetRenderedHeight();
        //public int layoutPriority => m_layoutPriority;
    }
}
