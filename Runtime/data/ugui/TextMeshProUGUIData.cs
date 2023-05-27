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
    using UnityEngine.Scripting;
    [Serializable, Preserve]
	public class TextMeshProUGUIData : GraphicData<TextMeshProUGUI>
	{
		[Store, Preserve] public bool maskable;
		[HideInInspector, Preserve] public bool maskable_IsSet;
		[Store, Preserve] public string text;
		[HideInInspector, Preserve] public bool text_IsSet;
		[Store, Preserve] public bool isRightToLeftText;
		[HideInInspector, Preserve] public bool isRightToLeftText_IsSet;

        [Store, Preserve] public TMP_FontAsset font;
        [HideInInspector, Preserve] public bool font_IsSet;
        [Store, Preserve] public bool enableVertexGradient;
        [HideInInspector, Preserve] public bool enableVertexGradient_IsSet;
        [Store, Preserve] public VertexGradient colorGradient;
        [HideInInspector, Preserve] public bool colorGradient_IsSet;
        [Store, Preserve] public TMP_ColorGradient colorGradientPreset;
        [HideInInspector, Preserve] public bool colorGradientPreset_IsSet;
        [Store, Preserve] public TMP_SpriteAsset spriteAsset;
        [HideInInspector, Preserve] public bool spriteAsset_IsSet;
        [Store, Preserve] public bool tintAllSprites;
        [HideInInspector, Preserve] public bool tintAllSprites_IsSet;
        [Store, Preserve] public TMP_StyleSheet styleSheet;
        [HideInInspector, Preserve] public bool styleSheet_IsSet;
        [Store, Preserve] public TMP_Style textStyle;
        [HideInInspector, Preserve] public bool textStyle_IsSet;
        [Store, Preserve] public bool overrideColorTags;
        [HideInInspector, Preserve] public bool overrideColorTags_IsSet;
        [Store, Preserve] public Color32 faceColor;
        [HideInInspector, Preserve] public bool faceColor_IsSet;
        [Store, Preserve] public Color32 outlineColor;
        [HideInInspector, Preserve] public bool outlineColor_IsSet;
        [Store, Preserve] public float outlineWidth;
        [HideInInspector, Preserve] public bool outlineWidth_IsSet;
        [Store, Preserve] public float fontSize;
        [HideInInspector, Preserve] public bool fontSize_IsSet;
        [Store, Preserve] public FontWeight fontWeight;
        [HideInInspector, Preserve] public bool fontWeight_IsSet;
        [Store, Preserve] public bool enableAutoSizing;
        [HideInInspector, Preserve] public bool enableAutoSizing_IsSet;
        [Store, Preserve] public float fontSizeMin;
        [HideInInspector, Preserve] public bool fontSizeMin_IsSet;
        [Store, Preserve] public float fontSizeMax;
        [HideInInspector, Preserve] public bool fontSizeMax_IsSet;
        [Store, Preserve] public FontStyles fontStyle;
        [HideInInspector, Preserve] public bool fontStyle_IsSet;
        [Store, Preserve] public HorizontalAlignmentOptions horizontalAlignment;
        [HideInInspector, Preserve] public bool horizontalAlignment_IsSet;
        [Store, Preserve] public VerticalAlignmentOptions verticalAlignment;
        [HideInInspector, Preserve] public bool verticalAlignment_IsSet;
        [Store, Preserve] public TextAlignmentOptions alignment;
        [HideInInspector, Preserve] public bool alignment_IsSet;
        [Store, Preserve] public float characterSpacing;
        [HideInInspector, Preserve] public bool characterSpacing_IsSet;
        [Store, Preserve] public float wordSpacing;
        [HideInInspector, Preserve] public bool wordSpacing_IsSet;
        [Store, Preserve] public float lineSpacing;
        [HideInInspector, Preserve] public bool lineSpacing_IsSet;
        [Store, Preserve] public float lineSpacingAdjustment;
        [HideInInspector, Preserve] public bool lineSpacingAdjustment_IsSet;
        [Store, Preserve] public float paragraphSpacing;
        [HideInInspector, Preserve] public bool paragraphSpacing_IsSet;
        [Store, Preserve] public float characterWidthAdjustment;
        [HideInInspector, Preserve] public bool characterWidthAdjustment_IsSet;
        [Store, Preserve] public bool enableWordWrapping;
        [HideInInspector, Preserve] public bool enableWordWrapping_IsSet;
        [Store, Preserve] public float wordWrappingRatios;
        [HideInInspector, Preserve] public bool wordWrappingRatios_IsSet;
        [Store, Preserve] public TextOverflowModes overflowMode;
        [HideInInspector, Preserve] public bool overflowMode_IsSet;
        [Store, Preserve] public TMP_Text linkedTextComponent;
        [HideInInspector, Preserve] public bool linkedTextComponent_IsSet;
        [Store, Preserve] public bool enableKerning;
        [HideInInspector, Preserve] public bool enableKerning_IsSet;
        [Store, Preserve] public bool extraPadding;
        [HideInInspector, Preserve] public bool extraPadding_IsSet;
        [Store, Preserve] public bool richText;
        [HideInInspector, Preserve] public bool richText_IsSet;
        [Store, Preserve] public bool parseCtrlCharacters;
        [HideInInspector, Preserve] public bool parseCtrlCharacters_IsSet;
        [Store, Preserve] public bool isOverlay;
        [HideInInspector, Preserve] public bool isOverlay_IsSet;
        [Store, Preserve] public bool isOrthographic;
        [HideInInspector, Preserve] public bool isOrthographic_IsSet;
        [Store, Preserve] public bool enableCulling;
        [HideInInspector, Preserve] public bool enableCulling_IsSet;
        [Store, Preserve] public bool ignoreVisibility;
        [HideInInspector, Preserve] public bool ignoreVisibility_IsSet;
        [Store, Preserve] public TextureMappingOptions horizontalMapping;
        [HideInInspector, Preserve] public bool horizontalMapping_IsSet;
        [Store, Preserve] public TextureMappingOptions verticalMapping;
        [HideInInspector, Preserve] public bool verticalMapping_IsSet;
        [Store, Preserve] public float mappingUvLineOffset;
        [HideInInspector, Preserve] public bool mappingUvLineOffset_IsSet;
        [Store, Preserve] public TextRenderFlags renderMode;
        [HideInInspector, Preserve] public bool renderMode_IsSet;
        [Store, Preserve] public VertexSortingOrder geometrySortingOrder;
        [HideInInspector, Preserve] public bool geometrySortingOrder_IsSet;
        [Store, Preserve] public bool isTextObjectScaleStatic;
        [HideInInspector, Preserve] public bool isTextObjectScaleStatic_IsSet;
        [Store, Preserve] public bool vertexBufferAutoSizeReduction;
        [HideInInspector, Preserve] public bool vertexBufferAutoSizeReduction_IsSet;
        [Store, Preserve] public int firstVisibleCharacter;
        [HideInInspector, Preserve] public bool firstVisibleCharacter_IsSet;
        [Store, Preserve] public int maxVisibleCharacters;
        [HideInInspector, Preserve] public bool maxVisibleCharacters_IsSet;
        [Store, Preserve] public int maxVisibleWords;
        [HideInInspector, Preserve] public bool maxVisibleWords_IsSet;
        [Store, Preserve] public int maxVisibleLines;
        [HideInInspector, Preserve] public bool maxVisibleLines_IsSet;
        [Store, Preserve] public bool useMaxVisibleDescender;
        [HideInInspector, Preserve] public bool useMaxVisibleDescender_IsSet;
        [Store, Preserve] public int pageToDisplay;
        [HideInInspector, Preserve] public bool pageToDisplay_IsSet;
        [Store, Preserve] public Vector4 margin;
        [HideInInspector, Preserve] public bool margin_IsSet;
        [Store, Preserve] public bool isUsingLegacyAnimationComponent;
        [HideInInspector, Preserve] public bool isUsingLegacyAnimationComponent_IsSet;
        [Store, Preserve] public bool autoSizeTextContainer;
        [HideInInspector, Preserve] public bool autoSizeTextContainer_IsSet;
        [Store, Preserve] public bool isVolumetricText;
        [HideInInspector, Preserve] public bool isVolumetricText_IsSet;

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
