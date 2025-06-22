Shader "Unlit/S_ScrollGlowUI"
{
    Properties
    {
        [NoScaleOffset]_Tex1("Tex1", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
        [NoScaleOffset]_GradientTex("GradientTex", 2D) = "white" {}
        _Intensity("Intensity", Range(0, 3)) = 1
        _TilingX("TilingX", Range(0, 5)) = 1
        _TilingY("TilingY", Range(0, 5)) = 1
        _OffsetX("OffsetX", Range(-3, 3)) = 0
        _OffsetY("OffsetY", Range(-3, 3)) = 0
        _SpeedX("SpeedX", Range(-0.5, 0.5)) = 0.05
        _SpeedY("SpeedY", Range(-0.5, 0.5)) = 0.05
        _TileOff2("TileOff2", Vector) = (1, 1, 0, 0)
        _Speed2("Speed2", Vector) = (-0.05, -0.1, 0, 0)
        _Opacity("Opacity", Range(0, 2)) = 1
        _Test("Test", Range(0, 10)) = 10
        _GammaPower("GammaPower", Float) = 0.4545
        [HDR]_Color("Color", Color) = (8, 8, 8, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
		_StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            //"RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }

        Stencil
        {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
        }
		
        ColorMask[_ColorMask]

        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest[unity_GUIZTestMode]
		ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        //#pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _RECEIVE_SHADOWS_OFF 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
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
             float3 TangentSpaceNormal;
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
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV : INTERP0;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV : INTERP1;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP2;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord : INTERP3;
            #endif
             float4 tangentWS : INTERP4;
             float4 texCoord0 : INTERP5;
             float4 fogFactorAndVertexLight : INTERP6;
             float3 positionWS : INTERP7;
             float3 normalWS : INTERP8;
             float3 viewDirectionWS : INTERP9;
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
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS.xyzw = input.tangentWS;
            output.texCoord0.xyzw = input.texCoord0;
            output.fogFactorAndVertexLight.xyzw = input.fogFactorAndVertexLight;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            output.viewDirectionWS.xyz = input.viewDirectionWS;
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
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.shadowCoord;
            #endif
            output.tangentWS = input.tangentWS.xyzw;
            output.texCoord0 = input.texCoord0.xyzw;
            output.fogFactorAndVertexLight = input.fogFactorAndVertexLight.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            output.viewDirectionWS = input.viewDirectionWS.xyz;
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
        float4 _Tex1_TexelSize;
        float _Intensity;
        float4 _GradientTex_TexelSize;
        float _TilingY;
        float _OffsetX;
        float _OffsetY;
        float _TilingX;
        float _SpeedY;
        float _SpeedX;
        float4 _MaskTex_TexelSize;
        float4 _TileOff2;
        float2 _Speed2;
        float _Opacity;
        float _Test;
        float _GammaPower;
        float4 _Color;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Tex1);
        SAMPLER(sampler_Tex1);
        TEXTURE2D(_GradientTex);
        SAMPLER(sampler_GradientTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        
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
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
        Out = A * B;
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        struct Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float
        {
        };
        
        void SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float(float2 Vector2_89d1d5c774a54fe98db015e8f1e8f537, float2 Vector2_8f3db0cfbc4c43cea2e6fb5fdd737ee1, float Vector1_99cccd65eebb434b84e49797bb813110, float2 Vector2_58c6e2f068a847b78b9658bf4ec55e36, float2 Vector2_41ef96380a404e4698dd7df0929228ea, Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float IN, out float2 UVs_1)
        {
        float _Property_ba2ff800cdc64965ba74636ced70f723_Out_0 = Vector1_99cccd65eebb434b84e49797bb813110;
        float _Cosine_0eb81ab01e3a40c48e3f4a2719d797df_Out_1;
        Unity_Cosine_float(_Property_ba2ff800cdc64965ba74636ced70f723_Out_0, _Cosine_0eb81ab01e3a40c48e3f4a2719d797df_Out_1);
        float2 _Property_9998911e002f4b2cb9d8f9a6f642cbcf_Out_0 = Vector2_89d1d5c774a54fe98db015e8f1e8f537;
        float2 _Property_01b2ebbd8d1a4930bb48555905fb4e04_Out_0 = Vector2_41ef96380a404e4698dd7df0929228ea;
        float2 _Subtract_6b6e53daeb414485beb26ec6d8bea47f_Out_2;
        Unity_Subtract_float2(_Property_9998911e002f4b2cb9d8f9a6f642cbcf_Out_0, _Property_01b2ebbd8d1a4930bb48555905fb4e04_Out_0, _Subtract_6b6e53daeb414485beb26ec6d8bea47f_Out_2);
        float _Split_b50db3b56d5640029e588cae241753c1_R_1 = _Subtract_6b6e53daeb414485beb26ec6d8bea47f_Out_2[0];
        float _Split_b50db3b56d5640029e588cae241753c1_G_2 = _Subtract_6b6e53daeb414485beb26ec6d8bea47f_Out_2[1];
        float _Split_b50db3b56d5640029e588cae241753c1_B_3 = 0;
        float _Split_b50db3b56d5640029e588cae241753c1_A_4 = 0;
        float _Multiply_30e79c8c6ba04b60888af937d6b059d7_Out_2;
        Unity_Multiply_float_float(_Cosine_0eb81ab01e3a40c48e3f4a2719d797df_Out_1, _Split_b50db3b56d5640029e588cae241753c1_R_1, _Multiply_30e79c8c6ba04b60888af937d6b059d7_Out_2);
        float _Sine_2fa3e4b4d7664f83861b98a1420bb82c_Out_1;
        Unity_Sine_float(_Property_ba2ff800cdc64965ba74636ced70f723_Out_0, _Sine_2fa3e4b4d7664f83861b98a1420bb82c_Out_1);
        float _Multiply_d5a5d5c375594695b51d6ad025e44b0d_Out_2;
        Unity_Multiply_float_float(_Sine_2fa3e4b4d7664f83861b98a1420bb82c_Out_1, _Split_b50db3b56d5640029e588cae241753c1_G_2, _Multiply_d5a5d5c375594695b51d6ad025e44b0d_Out_2);
        float _Add_9e0df47361cc4a0aa89757a2e05aaf66_Out_2;
        Unity_Add_float(_Multiply_30e79c8c6ba04b60888af937d6b059d7_Out_2, _Multiply_d5a5d5c375594695b51d6ad025e44b0d_Out_2, _Add_9e0df47361cc4a0aa89757a2e05aaf66_Out_2);
        float _Multiply_89b7fc66fc5f418b90ce777ccea724de_Out_2;
        Unity_Multiply_float_float(_Cosine_0eb81ab01e3a40c48e3f4a2719d797df_Out_1, _Split_b50db3b56d5640029e588cae241753c1_G_2, _Multiply_89b7fc66fc5f418b90ce777ccea724de_Out_2);
        float _Multiply_5ef24ab0e44b4db69e6257ea97fee72d_Out_2;
        Unity_Multiply_float_float(_Sine_2fa3e4b4d7664f83861b98a1420bb82c_Out_1, _Split_b50db3b56d5640029e588cae241753c1_R_1, _Multiply_5ef24ab0e44b4db69e6257ea97fee72d_Out_2);
        float _Subtract_70d7d50bf4094cf783daf26fad2c19e7_Out_2;
        Unity_Subtract_float(_Multiply_89b7fc66fc5f418b90ce777ccea724de_Out_2, _Multiply_5ef24ab0e44b4db69e6257ea97fee72d_Out_2, _Subtract_70d7d50bf4094cf783daf26fad2c19e7_Out_2);
        float4 _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RGBA_4;
        float3 _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RGB_5;
        float2 _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RG_6;
        Unity_Combine_float(_Add_9e0df47361cc4a0aa89757a2e05aaf66_Out_2, _Subtract_70d7d50bf4094cf783daf26fad2c19e7_Out_2, 0, 0, _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RGBA_4, _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RGB_5, _Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RG_6);
        float2 _Property_c1984ab36c6b4c2abf263c65acb47b94_Out_0 = Vector2_58c6e2f068a847b78b9658bf4ec55e36;
        float2 _Multiply_c3a701aa50884356ae3a5e95915d4013_Out_2;
        Unity_Multiply_float2_float2(_Combine_9446efa9c7ce4a9e9c1cd69017ab4334_RG_6, _Property_c1984ab36c6b4c2abf263c65acb47b94_Out_0, _Multiply_c3a701aa50884356ae3a5e95915d4013_Out_2);
        float2 _Add_a9e49e67e73a4a0bb2cda36996bb0fa0_Out_2;
        Unity_Add_float2(_Multiply_c3a701aa50884356ae3a5e95915d4013_Out_2, _Property_01b2ebbd8d1a4930bb48555905fb4e04_Out_0, _Add_a9e49e67e73a4a0bb2cda36996bb0fa0_Out_2);
        float2 _Property_7c1b871e1ce54b42aaa8a1149605bfd4_Out_0 = Vector2_8f3db0cfbc4c43cea2e6fb5fdd737ee1;
        float2 _Add_83cdfd47ba07431bac2f68b4a79e4698_Out_2;
        Unity_Add_float2(_Add_a9e49e67e73a4a0bb2cda36996bb0fa0_Out_2, _Property_7c1b871e1ce54b42aaa8a1149605bfd4_Out_0, _Add_83cdfd47ba07431bac2f68b4a79e4698_Out_2);
        UVs_1 = _Add_83cdfd47ba07431bac2f68b4a79e4698_Out_2;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Power_float4(float4 A, float4 B, out float4 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        // Custom interpolators pre vertex
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
        
        // Custom interpolators, pre surface
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
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0 = UnityBuildTexture2DStructNoScale(_Tex1);
            float4 _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0 = IN.uv0;
            float _Split_06860e9345884070bf53b999d104c292_R_1 = _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0[0];
            float _Split_06860e9345884070bf53b999d104c292_G_2 = _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0[1];
            float _Split_06860e9345884070bf53b999d104c292_B_3 = _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0[2];
            float _Split_06860e9345884070bf53b999d104c292_A_4 = _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0[3];
            float _Subtract_bef3acdd98764dbe85b0eccd42ed586c_Out_2;
            Unity_Subtract_float(_Split_06860e9345884070bf53b999d104c292_R_1, 0.005, _Subtract_bef3acdd98764dbe85b0eccd42ed586c_Out_2);
            float _Add_89641e7c1c7d45f2a0d120dbdd9eb981_Out_2;
            Unity_Add_float(_Split_06860e9345884070bf53b999d104c292_G_2, 0.005, _Add_89641e7c1c7d45f2a0d120dbdd9eb981_Out_2);
            float2 _Vector2_0a4e811462b44edfbc273b9beb259966_Out_0 = float2(_Subtract_bef3acdd98764dbe85b0eccd42ed586c_Out_2, _Add_89641e7c1c7d45f2a0d120dbdd9eb981_Out_2);
            float _Property_cbfbdc2276564f23b221e37e29c2b920_Out_0 = _OffsetY;
            float _Property_1ea18927728e487fbda6e9fda1bb1557_Out_0 = _OffsetX;
            float2 _Vector2_f7c300ab8eb3499c966b1877bda20569_Out_0 = float2(_Property_cbfbdc2276564f23b221e37e29c2b920_Out_0, _Property_1ea18927728e487fbda6e9fda1bb1557_Out_0);
            float _Property_a3fa92d14ddc40e2993fa5fb176150d6_Out_0 = _SpeedX;
            float _Property_2bd28354f77d4eeda8cb20aa959be3ef_Out_0 = _SpeedY;
            float2 _Vector2_884c061f0a464dbbbf53825aa47f7b98_Out_0 = float2(_Property_a3fa92d14ddc40e2993fa5fb176150d6_Out_0, _Property_2bd28354f77d4eeda8cb20aa959be3ef_Out_0);
            float2 _Multiply_585773567f6540ab870f2cf0f8eb6b3b_Out_2;
            Unity_Multiply_float2_float2(_Vector2_884c061f0a464dbbbf53825aa47f7b98_Out_0, (IN.TimeParameters.x.xx), _Multiply_585773567f6540ab870f2cf0f8eb6b3b_Out_2);
            float2 _Add_ccb18cef2fae4ca4be6809ecfc13e3f8_Out_2;
            Unity_Add_float2(_Vector2_f7c300ab8eb3499c966b1877bda20569_Out_0, _Multiply_585773567f6540ab870f2cf0f8eb6b3b_Out_2, _Add_ccb18cef2fae4ca4be6809ecfc13e3f8_Out_2);
            float _Property_83b3f60440b0487e94f8d35c4f96d2d4_Out_0 = _TilingY;
            float _Property_7bf779138bcf4473a1bd741b694b828c_Out_0 = _TilingX;
            float2 _Vector2_85e61bf4012f4f4cafc3acb6b5aafa07_Out_0 = float2(_Property_83b3f60440b0487e94f8d35c4f96d2d4_Out_0, _Property_7bf779138bcf4473a1bd741b694b828c_Out_0);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b;
            float2 _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float(_Vector2_0a4e811462b44edfbc273b9beb259966_Out_0, _Add_ccb18cef2fae4ca4be6809ecfc13e3f8_Out_2, 0, _Vector2_85e61bf4012f4f4cafc3acb6b5aafa07_Out_0, float2 (0.5, 0.5), _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b, _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1);
            float4 _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.tex, _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.samplerstate, _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.GetTransformedUV(_SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1));
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_R_4 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.r;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_G_5 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.g;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_B_6 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.b;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_A_7 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.a;
            float _Property_60627e08b3234cfdb40f517ccd7b28c9_Out_0 = _GammaPower;
            float _Power_cbc088ef699b472b8ac3f03618244297_Out_2;
            Unity_Power_float(_SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_R_4, _Property_60627e08b3234cfdb40f517ccd7b28c9_Out_0, _Power_cbc088ef699b472b8ac3f03618244297_Out_2);
            UnityTexture2D _Property_4bc3fdf9e0814b4f8dedd1e7d57593e3_Out_0 = UnityBuildTexture2DStructNoScale(_Tex1);
            float4 _UV_005fc2c53cba400da9cb1bcbb2b48eb6_Out_0 = IN.uv0;
            float4 _Property_3217fb39f823483cac1010e0db83f998_Out_0 = _TileOff2;
            float _Split_092eb41f347c4cf3a28c00d4afe4e2ac_R_1 = _Property_3217fb39f823483cac1010e0db83f998_Out_0[0];
            float _Split_092eb41f347c4cf3a28c00d4afe4e2ac_G_2 = _Property_3217fb39f823483cac1010e0db83f998_Out_0[1];
            float _Split_092eb41f347c4cf3a28c00d4afe4e2ac_B_3 = _Property_3217fb39f823483cac1010e0db83f998_Out_0[2];
            float _Split_092eb41f347c4cf3a28c00d4afe4e2ac_A_4 = _Property_3217fb39f823483cac1010e0db83f998_Out_0[3];
            float2 _Vector2_d8b14abcb04e48cdb4a457b7e8555ace_Out_0 = float2(_Split_092eb41f347c4cf3a28c00d4afe4e2ac_B_3, _Split_092eb41f347c4cf3a28c00d4afe4e2ac_A_4);
            float2 _Property_e2bdf3e36264468aa97c7d7f70c1b7a8_Out_0 = _Speed2;
            float _Split_f5849b807b9d4a488c0698b5a36e3c37_R_1 = _Property_e2bdf3e36264468aa97c7d7f70c1b7a8_Out_0[0];
            float _Split_f5849b807b9d4a488c0698b5a36e3c37_G_2 = _Property_e2bdf3e36264468aa97c7d7f70c1b7a8_Out_0[1];
            float _Split_f5849b807b9d4a488c0698b5a36e3c37_B_3 = 0;
            float _Split_f5849b807b9d4a488c0698b5a36e3c37_A_4 = 0;
            float2 _Vector2_199e8830ac574c2c9f08c208601ab1fe_Out_0 = float2(_Split_f5849b807b9d4a488c0698b5a36e3c37_R_1, _Split_f5849b807b9d4a488c0698b5a36e3c37_G_2);
            float2 _Multiply_e5b695c426be4dae859e9d42a3b76b89_Out_2;
            Unity_Multiply_float2_float2(_Vector2_199e8830ac574c2c9f08c208601ab1fe_Out_0, (IN.TimeParameters.x.xx), _Multiply_e5b695c426be4dae859e9d42a3b76b89_Out_2);
            float2 _Add_1aefa74c13f74b3fb164881113a1d7b7_Out_2;
            Unity_Add_float2(_Vector2_d8b14abcb04e48cdb4a457b7e8555ace_Out_0, _Multiply_e5b695c426be4dae859e9d42a3b76b89_Out_2, _Add_1aefa74c13f74b3fb164881113a1d7b7_Out_2);
            float2 _Vector2_f779a937888b4f37a749da01cec39eb1_Out_0 = float2(_Split_092eb41f347c4cf3a28c00d4afe4e2ac_R_1, _Split_092eb41f347c4cf3a28c00d4afe4e2ac_G_2);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_3a4be1e7e417402aaaf3d61eb128abae;
            float2 _SSubTransformation_3a4be1e7e417402aaaf3d61eb128abae_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_005fc2c53cba400da9cb1bcbb2b48eb6_Out_0.xy), _Add_1aefa74c13f74b3fb164881113a1d7b7_Out_2, 0, _Vector2_f779a937888b4f37a749da01cec39eb1_Out_0, float2 (0.5, 0.5), _SSubTransformation_3a4be1e7e417402aaaf3d61eb128abae, _SSubTransformation_3a4be1e7e417402aaaf3d61eb128abae_UVs_1);
            float4 _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4bc3fdf9e0814b4f8dedd1e7d57593e3_Out_0.tex, _Property_4bc3fdf9e0814b4f8dedd1e7d57593e3_Out_0.samplerstate, _Property_4bc3fdf9e0814b4f8dedd1e7d57593e3_Out_0.GetTransformedUV(_SSubTransformation_3a4be1e7e417402aaaf3d61eb128abae_UVs_1));
            float _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_R_4 = _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_RGBA_0.r;
            float _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_G_5 = _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_RGBA_0.g;
            float _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_B_6 = _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_RGBA_0.b;
            float _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_A_7 = _SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_RGBA_0.a;
            float _Power_e737bc7fbb034ebeb270d1b91fc36304_Out_2;
            Unity_Power_float(_SampleTexture2D_1a2f0e6ad45b4b1f83e57cbe3163da26_R_4, _Property_60627e08b3234cfdb40f517ccd7b28c9_Out_0, _Power_e737bc7fbb034ebeb270d1b91fc36304_Out_2);
            float _Multiply_3470dfe59c7644c5ac99503901d81746_Out_2;
            Unity_Multiply_float_float(_Power_cbc088ef699b472b8ac3f03618244297_Out_2, _Power_e737bc7fbb034ebeb270d1b91fc36304_Out_2, _Multiply_3470dfe59c7644c5ac99503901d81746_Out_2);
            float _Multiply_3605c41fcd5c4f33b06d0517e94838c7_Out_2;
            Unity_Multiply_float_float(_Multiply_3470dfe59c7644c5ac99503901d81746_Out_2, 2, _Multiply_3605c41fcd5c4f33b06d0517e94838c7_Out_2);
            UnityTexture2D _Property_f9cf039414294048addd27e7a0a51619_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0 = SAMPLE_TEXTURE2D(_Property_f9cf039414294048addd27e7a0a51619_Out_0.tex, _Property_f9cf039414294048addd27e7a0a51619_Out_0.samplerstate, _Property_f9cf039414294048addd27e7a0a51619_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_R_4 = _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0.r;
            float _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_G_5 = _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0.g;
            float _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_B_6 = _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0.b;
            float _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_A_7 = _SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0.a;
            float _Property_486a84e6dc834b518b18079a5207abbf_Out_0 = _GammaPower;
            float4 _Power_be95ee0aad0549978ee48dc6ca3a2b60_Out_2;
            Unity_Power_float4(_SampleTexture2D_1a8a3fe4444d43a793fa808ca0eb2298_RGBA_0, (_Property_486a84e6dc834b518b18079a5207abbf_Out_0.xxxx), _Power_be95ee0aad0549978ee48dc6ca3a2b60_Out_2);
            float4 _Multiply_c4b8e67f373f4c618f7aa868a0ac8486_Out_2;
            Unity_Multiply_float4_float4((_Multiply_3605c41fcd5c4f33b06d0517e94838c7_Out_2.xxxx), _Power_be95ee0aad0549978ee48dc6ca3a2b60_Out_2, _Multiply_c4b8e67f373f4c618f7aa868a0ac8486_Out_2);
            float4 _Add_aaf2d56b9e9c4fa6a72d67937734feeb_Out_2;
            Unity_Add_float4(_Multiply_c4b8e67f373f4c618f7aa868a0ac8486_Out_2, _Power_be95ee0aad0549978ee48dc6ca3a2b60_Out_2, _Add_aaf2d56b9e9c4fa6a72d67937734feeb_Out_2);
            float _Property_7d81c7c052e04980a0ad8092f58358b0_Out_0 = _Intensity;
            UnityTexture2D _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0 = UnityBuildTexture2DStructNoScale(_GradientTex);
            float2 _Vector2_e454b95381a1425787ecfe3110020e80_Out_0 = float2((_Add_aaf2d56b9e9c4fa6a72d67937734feeb_Out_2).x, 0);
            float4 _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.tex, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.samplerstate, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.GetTransformedUV(_Vector2_e454b95381a1425787ecfe3110020e80_Out_0));
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_R_4 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.r;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_G_5 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.g;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_B_6 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.b;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_A_7 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.a;
            float _Property_cf5f1ae0ea0b422ab67cd47bb19f8f28_Out_0 = _GammaPower;
            float4 _Power_8321ff1cc12c4d7d832b6b4a4f5f6e03_Out_2;
            Unity_Power_float4(_SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0, (_Property_cf5f1ae0ea0b422ab67cd47bb19f8f28_Out_0.xxxx), _Power_8321ff1cc12c4d7d832b6b4a4f5f6e03_Out_2);
            float4 _Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2;
            Unity_Multiply_float4_float4((_Property_7d81c7c052e04980a0ad8092f58358b0_Out_0.xxxx), _Power_8321ff1cc12c4d7d832b6b4a4f5f6e03_Out_2, _Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2);
            float4 _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1;
            Unity_Saturate_float4(_Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2, _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1);
            float4 _Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2;
            Unity_Multiply_float4_float4(_Add_aaf2d56b9e9c4fa6a72d67937734feeb_Out_2, _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1, _Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2);
            float4 _Property_bdea7c383d7f47f1ac3d67ba6d817a11_Out_0 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            float4 _Multiply_8c4d08bc0705412f937037b58d256724_Out_2;
            Unity_Multiply_float4_float4(_Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2, _Property_bdea7c383d7f47f1ac3d67ba6d817a11_Out_0, _Multiply_8c4d08bc0705412f937037b58d256724_Out_2);
            float _Property_27b3485d153b4ebfb9a4354477a74337_Out_0 = _Opacity;
            float4 _Multiply_bfb2752aa16b45e8a5f669c9feb1a4c6_Out_2;
            Unity_Multiply_float4_float4(_Add_aaf2d56b9e9c4fa6a72d67937734feeb_Out_2, (_Property_27b3485d153b4ebfb9a4354477a74337_Out_0.xxxx), _Multiply_bfb2752aa16b45e8a5f669c9feb1a4c6_Out_2);
            float4 _Saturate_35acbade06f3488da77d201068f6f553_Out_1;
            Unity_Saturate_float4(_Multiply_bfb2752aa16b45e8a5f669c9feb1a4c6_Out_2, _Saturate_35acbade06f3488da77d201068f6f553_Out_1);
            surface.BaseColor = (_Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2.xyz);
            surface.NormalTS = IN.TangentSpaceNormal;
            surface.Emission = (_Multiply_8c4d08bc0705412f937037b58d256724_Out_2.xyz);
            surface.Metallic = 0;
            surface.Smoothness = 0.5;
            surface.Occlusion = 1;
            surface.Alpha = (_Saturate_35acbade06f3488da77d201068f6f553_Out_1).x;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
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
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.uv0 = input.texCoord0;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}