Shader "UILevelFX"
{
    Properties
    {
        [HDR]Color_1a4631009d74432abfb1b9df4a7c0d5f("Color", Color) = (0.4301988, 0.1193745, 0, 0)
        [NoScaleOffset]_MainTex("MainTextures", 2D) = "white" {}
        [NoScaleOffset]Texture2D_4b628934ab38483b86b546fa2cc99b31("MaskTextures", 2D) = "white" {}
        [NoScaleOffset]Texture2D_a7a259bb6aaa44cb95502c8db7460b29("NoiseTextures", 2D) = "white" {}
        Vector1_0256107dba4c44cca54513566d3f09db("Speed", Float) = 0.5
        [HDR]Color_031eafd771f94940bbe876602ac41ee6("ColorLevel", Color) = (0.3773585, 0.3773585, 0.3773585, 0)
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

         _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest [unity_GUIZTestMode]
        ZWrite Off

        ColorMask [_ColorMask]
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 Color_1a4631009d74432abfb1b9df4a7c0d5f;
        float4 _MainTex_TexelSize;
        float4 Texture2D_4b628934ab38483b86b546fa2cc99b31_TexelSize;
        float4 Texture2D_a7a259bb6aaa44cb95502c8db7460b29_TexelSize;
        float Vector1_0256107dba4c44cca54513566d3f09db;
        float4 Color_031eafd771f94940bbe876602ac41ee6;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(Texture2D_4b628934ab38483b86b546fa2cc99b31);
        SAMPLER(samplerTexture2D_4b628934ab38483b86b546fa2cc99b31);
        TEXTURE2D(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
        SAMPLER(samplerTexture2D_a7a259bb6aaa44cb95502c8db7460b29);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1f9e10b4673a4d33be2c3f6085b55e16_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_1a4631009d74432abfb1b9df4a7c0d5f) : Color_1a4631009d74432abfb1b9df4a7c0d5f;
            float4 _Property_cdb2dd509e2547a3a8094470ddf8e243_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_031eafd771f94940bbe876602ac41ee6) : Color_031eafd771f94940bbe876602ac41ee6;
            UnityTexture2D _Property_83befa59a6c94882af9f5d247120a790_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
            float _Property_cee161fddf9a48b99778ae90b9eff6f6_Out_0 = Vector1_0256107dba4c44cca54513566d3f09db;
            float _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_cee161fddf9a48b99778ae90b9eff6f6_Out_0, _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2);
            float2 _Vector2_4d9d36a97c974c85979b03c87f8794e4_Out_0 = float2(_Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2, _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2);
            float2 _TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_4d9d36a97c974c85979b03c87f8794e4_Out_0, _TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3);
            float4 _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0 = SAMPLE_TEXTURE2D(_Property_83befa59a6c94882af9f5d247120a790_Out_0.tex, _Property_83befa59a6c94882af9f5d247120a790_Out_0.samplerstate, _Property_83befa59a6c94882af9f5d247120a790_Out_0.GetTransformedUV(_TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3));
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_R_4 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.r;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_G_5 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.g;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_B_6 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.b;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_A_7 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.a;
            float4 _Add_ddc3bb278502435d871c2c3588f6eec9_Out_2;
            Unity_Add_float4(_Property_cdb2dd509e2547a3a8094470ddf8e243_Out_0, _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0, _Add_ddc3bb278502435d871c2c3588f6eec9_Out_2);
            UnityTexture2D _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.tex, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.samplerstate, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_R_4 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.r;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_G_5 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.g;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_B_6 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.b;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.a;
            UnityTexture2D _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4b628934ab38483b86b546fa2cc99b31);
            float4 _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.tex, _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.samplerstate, _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_R_4 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.r;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_G_5 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.g;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_B_6 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.b;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_A_7 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.a;
            float4 _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2;
            Unity_Multiply_float4_float4((_SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7.xxxx), _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0, _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2);
            float4 _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2;
            Unity_Multiply_float4_float4(_Add_ddc3bb278502435d871c2c3588f6eec9_Out_2, _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2, _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2);
            float4 _Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2;
            Unity_Multiply_float4_float4(_Property_1f9e10b4673a4d33be2c3f6085b55e16_Out_0, _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2, _Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2);
            float4 _Add_82579f877edb44da977e704e2fa1a150_Out_2;
            Unity_Add_float4(_Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2, _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0, _Add_82579f877edb44da977e704e2fa1a150_Out_2);
            surface.BaseColor = (_Add_82579f877edb44da977e704e2fa1a150_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
            // Render State
            Cull Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 Color_1a4631009d74432abfb1b9df4a7c0d5f;
        float4 _MainTex_TexelSize;
        float4 Texture2D_4b628934ab38483b86b546fa2cc99b31_TexelSize;
        float4 Texture2D_a7a259bb6aaa44cb95502c8db7460b29_TexelSize;
        float Vector1_0256107dba4c44cca54513566d3f09db;
        float4 Color_031eafd771f94940bbe876602ac41ee6;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(Texture2D_4b628934ab38483b86b546fa2cc99b31);
        SAMPLER(samplerTexture2D_4b628934ab38483b86b546fa2cc99b31);
        TEXTURE2D(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
        SAMPLER(samplerTexture2D_a7a259bb6aaa44cb95502c8db7460b29);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            // GraphFunctions: <None>
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.tex, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.samplerstate, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_R_4 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.r;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_G_5 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.g;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_B_6 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.b;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.a;
            surface.Alpha = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
            // Render State
            Cull Back
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 Color_1a4631009d74432abfb1b9df4a7c0d5f;
        float4 _MainTex_TexelSize;
        float4 Texture2D_4b628934ab38483b86b546fa2cc99b31_TexelSize;
        float4 Texture2D_a7a259bb6aaa44cb95502c8db7460b29_TexelSize;
        float Vector1_0256107dba4c44cca54513566d3f09db;
        float4 Color_031eafd771f94940bbe876602ac41ee6;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(Texture2D_4b628934ab38483b86b546fa2cc99b31);
        SAMPLER(samplerTexture2D_4b628934ab38483b86b546fa2cc99b31);
        TEXTURE2D(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
        SAMPLER(samplerTexture2D_a7a259bb6aaa44cb95502c8db7460b29);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            // GraphFunctions: <None>
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.tex, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.samplerstate, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_R_4 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.r;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_G_5 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.g;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_B_6 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.b;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.a;
            surface.Alpha = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 Color_1a4631009d74432abfb1b9df4a7c0d5f;
        float4 _MainTex_TexelSize;
        float4 Texture2D_4b628934ab38483b86b546fa2cc99b31_TexelSize;
        float4 Texture2D_a7a259bb6aaa44cb95502c8db7460b29_TexelSize;
        float Vector1_0256107dba4c44cca54513566d3f09db;
        float4 Color_031eafd771f94940bbe876602ac41ee6;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(Texture2D_4b628934ab38483b86b546fa2cc99b31);
        SAMPLER(samplerTexture2D_4b628934ab38483b86b546fa2cc99b31);
        TEXTURE2D(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
        SAMPLER(samplerTexture2D_a7a259bb6aaa44cb95502c8db7460b29);
        
            // Graph Includes
            // GraphIncludes: <None>
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_1f9e10b4673a4d33be2c3f6085b55e16_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_1a4631009d74432abfb1b9df4a7c0d5f) : Color_1a4631009d74432abfb1b9df4a7c0d5f;
            float4 _Property_cdb2dd509e2547a3a8094470ddf8e243_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_031eafd771f94940bbe876602ac41ee6) : Color_031eafd771f94940bbe876602ac41ee6;
            UnityTexture2D _Property_83befa59a6c94882af9f5d247120a790_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_a7a259bb6aaa44cb95502c8db7460b29);
            float _Property_cee161fddf9a48b99778ae90b9eff6f6_Out_0 = Vector1_0256107dba4c44cca54513566d3f09db;
            float _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_cee161fddf9a48b99778ae90b9eff6f6_Out_0, _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2);
            float2 _Vector2_4d9d36a97c974c85979b03c87f8794e4_Out_0 = float2(_Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2, _Multiply_ba42ee82531a43729f628c2acfaab72a_Out_2);
            float2 _TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_4d9d36a97c974c85979b03c87f8794e4_Out_0, _TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3);
            float4 _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0 = SAMPLE_TEXTURE2D(_Property_83befa59a6c94882af9f5d247120a790_Out_0.tex, _Property_83befa59a6c94882af9f5d247120a790_Out_0.samplerstate, _Property_83befa59a6c94882af9f5d247120a790_Out_0.GetTransformedUV(_TilingAndOffset_082afaacf5654ba29590841a291268e1_Out_3));
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_R_4 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.r;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_G_5 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.g;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_B_6 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.b;
            float _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_A_7 = _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0.a;
            float4 _Add_ddc3bb278502435d871c2c3588f6eec9_Out_2;
            Unity_Add_float4(_Property_cdb2dd509e2547a3a8094470ddf8e243_Out_0, _SampleTexture2D_963acb2f735c4091b56caaf8fc0a2e36_RGBA_0, _Add_ddc3bb278502435d871c2c3588f6eec9_Out_2);
            UnityTexture2D _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.tex, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.samplerstate, _Property_9c1d7c45e8f243c2808fd6dd66734aee_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_R_4 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.r;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_G_5 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.g;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_B_6 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.b;
            float _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7 = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0.a;
            UnityTexture2D _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4b628934ab38483b86b546fa2cc99b31);
            float4 _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.tex, _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.samplerstate, _Property_983476e7ed2b4720b03ac770cdf5a6d9_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_R_4 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.r;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_G_5 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.g;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_B_6 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.b;
            float _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_A_7 = _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0.a;
            float4 _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2;
            Unity_Multiply_float4_float4((_SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7.xxxx), _SampleTexture2D_7b4be775c5b64c80bc12c498265a54b3_RGBA_0, _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2);
            float4 _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2;
            Unity_Multiply_float4_float4(_Add_ddc3bb278502435d871c2c3588f6eec9_Out_2, _Multiply_3c24578cb4c1459f9beacede7fcdc480_Out_2, _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2);
            float4 _Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2;
            Unity_Multiply_float4_float4(_Property_1f9e10b4673a4d33be2c3f6085b55e16_Out_0, _Multiply_575f46bc9d99484284dc81f20e2900d7_Out_2, _Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2);
            float4 _Add_82579f877edb44da977e704e2fa1a150_Out_2;
            Unity_Add_float4(_Multiply_2fcf4530d65a4d3baa3c35dc2778badd_Out_2, _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_RGBA_0, _Add_82579f877edb44da977e704e2fa1a150_Out_2);
            surface.BaseColor = (_Add_82579f877edb44da977e704e2fa1a150_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_a7cf2ee9417b4a6ca173aefa00f445f6_A_7;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}