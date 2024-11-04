﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nickeltin.SDF.Runtime
{
    public partial class SDFImage
    {
        #region Base Image re-creation
        
        /// <summary>
        /// Whether or not to render the center of a Tiled or Sliced image.
        /// </summary>
        /// <remarks>
        /// This will only have any effect if the <see cref="ActiveSprite"/> has borders.
        /// </remarks>
        public bool FillCenter
        {
            get => _fillCenter;
            set => SDFRendererUtil.SetProperty(this, ref _fillCenter, value);
        }

        /// <summary>
        /// Filling method for filled sprites.
        /// </summary>
        public Image.FillMethod FillMethod
        {
            get => _fillMethod;
            set
            {
                if (SDFRendererUtil.SetProperty(this, ref _fillMethod, value))
                {
                    _fillOrigin = 0;
                }
            }
        }
        
        /// <summary>
        /// Amount of the Image shown when the <see cref="ImageType"/> is set to <see cref="Image.Type.Filled"/>
        /// </summary>
        /// <remarks>
        /// 0-1 range with 0 being nothing shown, and 1 being the full Image.
        /// </remarks>
        public float FillAmount
        {
            get => _fillAmount;
            set => SDFRendererUtil.SetProperty(this, ref _fillAmount, Mathf.Clamp01(value));
        }

        /// <summary>
        /// How to display the image.
        /// </summary>
        /// <remarks>
        /// Unity can interpret an Image in various different ways depending on the intended purpose. This can be used to display:
        /// - Whole images stretched to fit the RectTransform of the Image.
        /// - A 9-sliced image useful for various decorated UI boxes and other rectangular elements.
        /// - A tiled image with sections of the sprite repeated.
        /// - As a partial image, useful for wipes, fades, timers, status bars etc.
        /// </remarks>
        public Image.Type ImageType
        {
            get => _imageType;
            set => SDFRendererUtil.SetProperty(this, ref _imageType, value);
        }
        
        /// <summary>
        /// Whether this image should preserve its Sprite aspect ratio.
        /// </summary>
        public bool PreserveAspect
        {
            get => _preserveAspect;
            set => SDFRendererUtil.SetProperty(this, ref _preserveAspect, value);
        }
        
        /// <summary>
        /// Whether the Image should be filled clockwise (true) or counter-clockwise (false).
        /// </summary>
        /// <remarks>
        /// This will only have any effect if the Image.type is set to Image.Type.Filled and Image.fillMethod is set to any of the Radial methods.
        /// </remarks>
        public bool FillClockwise
        {
            get => _fillClockwise;
            set => SDFRendererUtil.SetProperty(this, ref _fillClockwise, value);
        }
        
        /// <summary>
        /// Controls the origin point of the Fill process. Value means different things with each fill method.
        /// </summary>
        /// <remarks>
        /// You should cast to the appropriate origin type: Image.OriginHorizontal, Image.OriginVertical, Image.Origin90, Image.Origin180 or Image.Origin360 depending on the Image.Fillmethod.
        /// Note: This will only have any effect if the Image.type is set to Image.Type.Filled.
        /// </remarks>
        public int FillOrigin
        {
            get => _fillOrigin;
            set => SDFRendererUtil.SetProperty(this, ref _fillOrigin, value);
        }
        
        /// <summary>
        /// Allows you to specify whether the UI Image should be displayed using the mesh generated by the TextureImporter, or by a simple quad mesh.
        /// </summary>
        /// <remarks>
        /// When this property is set to false, the UI Image uses a simple quad. When set to true, the UI Image uses the sprite mesh generated by the [[TextureImporter]]. You should set this to true if you want to use a tightly fitted sprite mesh based on the alpha values in your image.
        /// Note: If the texture importer's SpriteMeshType property is set to SpriteMeshType.FullRect, it will only generate a quad, and not a tightly fitted sprite mesh, which means this UI image will be drawn using a quad regardless of the value of this property. Therefore, when enabling this property to use a tightly fitted sprite mesh, you must also ensure the texture importer's SpriteMeshType property is set to Tight.
        /// </remarks>
        public bool UseSpriteMesh
        {
            get => _useSpriteMesh;
            set => SDFRendererUtil.SetProperty(this, ref _useSpriteMesh, value);
        }
        
        /// <summary>
        /// Pixel per unit modifier to change how sliced sprites are generated.
        /// </summary>
        public float PixelsPerUnitMultiplier
        {
            get => _pixelsPerUnitMultiplier;
            set => SDFRendererUtil.SetProperty(this, ref _pixelsPerUnitMultiplier, Mathf.Max(0.01f, value));
        }
        
        /// <summary>
        /// Whether the Sprite of the image has a border to work with.
        /// </summary>
        public bool HasBorder
        {
            get
            {
                if (ActiveSprite == null) return false;
                var v = ActiveSprite.border;
                return v.sqrMagnitude > 0f;
            }
        }
        
        /// <summary>
        /// Pixels per unit of sprite divided by pixels per unity of canvas.
        /// </summary>
        public float PixelsPerUnit
        {
            get
            {
                float spritePixelsPerUnit = 100;
                if (ActiveSprite != null)
                    spritePixelsPerUnit = ActiveSprite.pixelsPerUnit;

                if (canvas != null)
                    _cachedReferencePixelsPerUnit = canvas.referencePixelsPerUnit;

                return spritePixelsPerUnit / _cachedReferencePixelsPerUnit;
            }
        }
        
        
        /// <summary>
        /// The alpha threshold specifies the minimum alpha a pixel must have for the event to considered a "hit" on the Image.
        /// </summary>
        /// <remarks>
        /// Alpha values less than the threshold will cause raycast events to pass through the Image. An value of 1 would cause only fully opaque pixels to register raycast events on the Image. The alpha tested is retrieved from the image sprite only, while the alpha of the Image [[UI.Graphic.color]] is disregarded.
        ///
        /// alphaHitTestMinimumThreshold defaults to 0; all raycast events inside the Image rectangle are considered a hit. In order for greater than 0 to values to work, the sprite used by the Image must have readable pixels. This can be achieved by enabling Read/Write enabled in the advanced Texture Import Settings for the sprite and disabling atlassing for the sprite.
        /// </remarks>
        // ReSharper disable once ConvertToAutoProperty
        public float AlphaHitTestMinimumThreshold
        {
            get => _alphaHitTestMinimumThreshold;
            set => _alphaHitTestMinimumThreshold = value;
        }
        
        #endregion

        #region Base properties override

        public override Texture mainTexture
        {
            get
            {
                if (ActiveSprite != null) return ActiveSprite.texture;
                return s_WhiteTexture;

            }
        }
        
        public override Material defaultMaterial => DefaultMaterial;

        /// <summary>
        /// SDFImage uses color as tint for all layers, it used as vertex color multiplied with each layer color.
        /// </summary>
        public override Color color
        {
            get => _sdfRendererSettings.MainColor;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.MainColor, value);
        }

        #endregion

        #region Unique to SDFImage

        /// <summary>
        /// All SDF related settings can be setted at once
        /// </summary>
        public SDFRendererSettings SDFRendererSettings
        {
            get => _sdfRendererSettings;
            set
            {
                var v = value;
                v.OutlineWidth = Mathf.Clamp(v.OutlineWidth, SDFRendererUtil.MIN_OUTLINE, SDFRendererUtil.MAX_OUTLINE);
                v.ShadowWidth = Mathf.Clamp(v.ShadowWidth, SDFRendererUtil.MIN_SHADOW, SDFRendererUtil.MAX_SHADOW);
                SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings, v);
            }
        }

        /// <summary>
        /// Wrapper around sprite and its metadata to keep compatability between versions
        /// </summary>
        public SDFSpriteReference SDFSpriteReference
        {
            get => _sdfSpriteReference;
            set
            {
                if (SDFRendererUtil.SetProperty(ref _sdfSpriteReference, value))
                {
                    ResetAlphaHitThresholdIfNeeded();
                    SetAllDirty();
                    TrackSprite();
                }
            }
        }
        

        /// <summary>
        /// Sprite used in inspector.
        /// </summary>
        public Sprite Sprite => SDFSpriteReference.SourceSprite;

        /// <summary>
        /// Currently used sprite.
        /// In later versions might point to override sprite if its support were added.
        /// </summary>
        public Sprite ActiveSprite => Sprite;

        /// <summary>
        /// Returns generated sdf texture if <see cref="ActiveSDFSprite"/> not null.
        /// This might be either original texture, or packed atlas texture.
        /// </summary>
        public Texture2D SDFTexture => ActiveSDFSprite != null ? ActiveSDFSprite.texture : null;

        /// <summary>
        /// Active sprite generated by SDF Importer.
        /// SDF Image require both, original and sdf sprites to function. 
        /// </summary>
        public Sprite ActiveSDFSprite => SDFSpriteReference.SDFSprite;

        /// <summary>
        /// Metadata from sprite SDF sprite generation.
        /// This is adjusted border offset, might differ from one use in import settings.
        /// Adjusted accordingly to texture size, texture might get clamped by max size.
        /// Border offset added to sdf texture along all sprite edges to make enough space for sdf pixels.
        /// </summary>
        public Vector4 BorderOffset => SDFSpriteReference.Metadata.BorderOffset;
        

        /// <inheritdoc cref="color"/>
        /// <remarks>
        /// Wrapper for unification.
        /// </remarks>>
        public Color MainColor
        {
            get => this.color;
            set => this.color = value;
        }
        
        
        /// <summary>
        /// Should mesh for regular image be generated?
        /// </summary>
        public bool RenderRegular
        {
            get => _sdfRendererSettings.RenderRegular;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.RenderRegular, value);
        }
        
        /// <summary>
        /// Color of regular mesh part, where source sprite is rendered.
        /// </summary>
        public Color RegularColor
        {
            get => _sdfRendererSettings.RegularColor;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.RegularColor, value);
        }
        
        

        /// <summary>
        /// Should mesh for sdf sprite be generated?
        /// </summary>
        public bool RenderOutline
        {
            get => _sdfRendererSettings.RenderOutline;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.RenderOutline, value);
        }
        
        /// <summary>
        /// Value passed to shader as vertex data, sdf color will be multiplied with it.
        /// Usually useful for simple materials, with one color or grayscale, otherwise color multiplication will yield unpleasant results. 
        /// </summary>
        public Color OutlineColor
        {
            get => _sdfRendererSettings.OutlineColor;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.OutlineColor, value);
        }
        
        /// <summary>
        /// Value passed to shader as vertex data, outline layer width will be multiplied with it.
        /// </summary>
        public float OutlineWidth
        {
            get => _sdfRendererSettings.OutlineWidth;
            set => SDFRendererUtil.SetFloat(this, ref _sdfRendererSettings.OutlineWidth, value, 
                SDFRendererUtil.MIN_OUTLINE, SDFRendererUtil.MAX_OUTLINE);
        }
        
        
        
        /// <summary>
        /// Should mesh for sdf sprite shadow part be generated?
        /// </summary>
        public bool RenderShadow
        {
            get => _sdfRendererSettings.RenderShadow;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.RenderShadow, value);
        }
        
        
        /// <summary>
        /// Value passed to shader as vertex data, sdf shadow color will be multiplied with it.
        /// </summary>
        public Color ShadowColor
        {
            get => _sdfRendererSettings.ShadowColor;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.ShadowColor, value);
        }
        
        /// <summary>
        /// Value passed to shader as vertex data, shadow layer width will be multiplied with it.
        /// </summary>
        public float ShadowWidth
        {
            get => _sdfRendererSettings.ShadowWidth;
            set => SDFRendererUtil.SetFloat(this, ref _sdfRendererSettings.ShadowWidth, value, 
                SDFRendererUtil.MIN_SHADOW, SDFRendererUtil.MAX_SHADOW);
        }
        
        /// <summary>
        /// How much shadow mesh is offseted.
        /// </summary>
        public Vector2 ShadowOffset
        {
            get => _sdfRendererSettings.ShadowOffset;
            set => SDFRendererUtil.SetProperty(this, ref _sdfRendererSettings.ShadowOffset, value);
        }
        
        public static Material DefaultMaterial
        {
            get
            {
                if (_defaultMaterial == null)
                {
                    _defaultMaterial = new Material(Shader.Find(SDFUtil.SDFDisplayUIShaderName))
                    {
                        name = "Default UI SDF Material",
                        hideFlags = HideFlags.NotEditable
                    };
                }

                return _defaultMaterial;
            }
        }

        #endregion
    }
}