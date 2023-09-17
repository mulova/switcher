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
    public class TMP_TextData : MaskableGraphicData<TMP_Text>
	{
		[Store] public string text;
		[HideInInspector] public bool text_mod;
		[Store] public bool isRightToLeftText;
		[HideInInspector] public bool isRightToLeftText_mod;

        [Store] public TMP_FontAsset font;
        [HideInInspector] public bool font_mod;
        [Store] public bool enableVertexGradient;
        [HideInInspector] public bool enableVertexGradient_mod;
        [Store] public VertexGradient colorGradient;
        [HideInInspector] public bool colorGradient_mod;
        [Store] public TMP_ColorGradient colorGradientPreset;
        [HideInInspector] public bool colorGradientPreset_mod;
        [Store] public TMP_SpriteAsset spriteAsset;
        [HideInInspector] public bool spriteAsset_mod;
        [Store] public bool tintAllSprites;
        [HideInInspector] public bool tintAllSprites_mod;
        [Store] public TMP_StyleSheet styleSheet;
        [HideInInspector] public bool styleSheet_mod;
        [Store] public TMP_Style textStyle;
        [HideInInspector] public bool textStyle_mod;
        [Store] public bool overrideColorTags;
        [HideInInspector] public bool overrideColorTags_mod;
        [Store] public Color32 faceColor;
        [HideInInspector] public bool faceColor_mod;
        [Store] public Color32 outlineColor;
        [HideInInspector] public bool outlineColor_mod;
        [Store] public float outlineWidth;
        [HideInInspector] public bool outlineWidth_mod;
        [Store] public float fontSize;
        [HideInInspector] public bool fontSize_mod;
        [Store] public FontWeight fontWeight;
        [HideInInspector] public bool fontWeight_mod;
        [Store] public bool enableAutoSizing;
        [HideInInspector] public bool enableAutoSizing_mod;
        [Store] public float fontSizeMin;
        [HideInInspector] public bool fontSizeMin_mod;
        [Store] public float fontSizeMax;
        [HideInInspector] public bool fontSizeMax_mod;
        [Store] public FontStyles fontStyle;
        [HideInInspector] public bool fontStyle_mod;
        [Store] public HorizontalAlignmentOptions horizontalAlignment;
        [HideInInspector] public bool horizontalAlignment_mod;
        [Store] public VerticalAlignmentOptions verticalAlignment;
        [HideInInspector] public bool verticalAlignment_mod;
        [Store] public TextAlignmentOptions alignment;
        [HideInInspector] public bool alignment_mod;
        [Store] public float characterSpacing;
        [HideInInspector] public bool characterSpacing_mod;
        [Store] public float wordSpacing;
        [HideInInspector] public bool wordSpacing_mod;
        [Store] public float lineSpacing;
        [HideInInspector] public bool lineSpacing_mod;
        [Store] public float lineSpacingAdjustment;
        [HideInInspector] public bool lineSpacingAdjustment_mod;
        [Store] public float paragraphSpacing;
        [HideInInspector] public bool paragraphSpacing_mod;
        [Store] public float characterWidthAdjustment;
        [HideInInspector] public bool characterWidthAdjustment_mod;
        [Store] public bool enableWordWrapping;
        [HideInInspector] public bool enableWordWrapping_mod;
        [Store] public float wordWrappingRatios;
        [HideInInspector] public bool wordWrappingRatios_mod;
        [Store] public TextOverflowModes overflowMode;
        [HideInInspector] public bool overflowMode_mod;
        [Store] public TMP_Text linkedTextComponent;
        [HideInInspector] public bool linkedTextComponent_mod;
        [Store] public bool enableKerning;
        [HideInInspector] public bool enableKerning_mod;
        [Store] public bool extraPadding;
        [HideInInspector] public bool extraPadding_mod;
        [Store] public bool richText;
        [HideInInspector] public bool richText_mod;
        [Store] public bool parseCtrlCharacters;
        [HideInInspector] public bool parseCtrlCharacters_mod;
        [Store] public bool isOverlay;
        [HideInInspector] public bool isOverlay_mod;
        [Store] public bool isOrthographic;
        [HideInInspector] public bool isOrthographic_mod;
        [Store] public bool enableCulling;
        [HideInInspector] public bool enableCulling_mod;
        [Store] public bool ignoreVisibility;
        [HideInInspector] public bool ignoreVisibility_mod;
        [Store] public TextureMappingOptions horizontalMapping;
        [HideInInspector] public bool horizontalMapping_mod;
        [Store] public TextureMappingOptions verticalMapping;
        [HideInInspector] public bool verticalMapping_mod;
        [Store] public float mappingUvLineOffset;
        [HideInInspector] public bool mappingUvLineOffset_mod;
        [Store] public TextRenderFlags renderMode;
        [HideInInspector] public bool renderMode_mod;
        [Store] public VertexSortingOrder geometrySortingOrder;
        [HideInInspector] public bool geometrySortingOrder_mod;
        [Store] public bool isTextObjectScaleStatic;
        [HideInInspector] public bool isTextObjectScaleStatic_mod;
        [Store] public bool vertexBufferAutoSizeReduction;
        [HideInInspector] public bool vertexBufferAutoSizeReduction_mod;
        [Store] public int firstVisibleCharacter;
        [HideInInspector] public bool firstVisibleCharacter_mod;
        [Store] public int maxVisibleCharacters;
        [HideInInspector] public bool maxVisibleCharacters_mod;
        [Store] public int maxVisibleWords;
        [HideInInspector] public bool maxVisibleWords_mod;
        [Store] public int maxVisibleLines;
        [HideInInspector] public bool maxVisibleLines_mod;
        [Store] public bool useMaxVisibleDescender;
        [HideInInspector] public bool useMaxVisibleDescender_mod;
        [Store] public int pageToDisplay;
        [HideInInspector] public bool pageToDisplay_mod;
        [Store] public Vector4 margin;
        [HideInInspector] public bool margin_mod;
        [Store] public bool isUsingLegacyAnimationComponent;
        [HideInInspector] public bool isUsingLegacyAnimationComponent_mod;
        [Store] public bool autoSizeTextContainer;
        [HideInInspector] public bool autoSizeTextContainer_mod;
        [Store] public bool isVolumetricText;
        [HideInInspector] public bool isVolumetricText_mod;

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
