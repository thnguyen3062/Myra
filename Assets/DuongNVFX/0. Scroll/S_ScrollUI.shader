Shader "Unlit/S_ScrollUI"
{
    Properties
    {
        [NoScaleOffset]_Tex1("Tex1", 2D) = "white" {}
        [NoScaleOffset]_PatternTex("PatternTex", 2D) = "white" {}
        [NoScaleOffset]_GradientTex("GradientTex", 2D) = "white" {}
        _Intensity("Intensity", Range(0, 3)) = 1
        _Dissolve("Dissolve", Range(0.01, 1)) = 0.5
        _TileOff1("TileOff1", Vector) = (1, 1, 0, 0)
        _TileOff2("TileOff2", Vector) = (1, 1, 0, 0)
        _Speeds("Speeds", Vector) = (0.05, 0.05, 0.07, 0.08)
        _TileOffMain("TileOffMain", Vector) = (1, 1, 0, 0)
        [HDR]_EdgeColor("EdgeColor", Color) = (16, 1.759162, 0, 1)
        _EdgeWidth("EdgeWidth", Range(0, 0.5)) = 0.1
        _DissolveAmount("DissolveAmount", Range(-1, 1)) = 0.2
        _Fade("Fade", Range(0.1, 0.5)) = 0.25
        _Direction("Direction", Range(0, 1)) = 0
        _Grayscale("Grayscale", Range(0, 1)) = 0
        _GammaPower("GammaPower", Range(0.4545, 1)) = 0.4545
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
            //"UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
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
                // LightMode: <None>
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
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
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
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float3 viewDirectionWS;
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
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
             float3 viewDirectionWS : INTERP3;
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
            output.texCoord0.xyzw = input.texCoord0;
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
            output.texCoord0 = input.texCoord0.xyzw;
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
        float _Dissolve;
        float4 _TileOff2;
        float4 _TileOff1;
        float4 _Speeds;
        float4 _PatternTex_TexelSize;
        float4 _TileOffMain;
        float4 _EdgeColor;
        float _EdgeWidth;
        float _DissolveAmount;
        float _Fade;
        float _Direction;
        float _Grayscale;
        float _GammaPower;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_Tex1);
        SAMPLER(sampler_Tex1);
        TEXTURE2D(_GradientTex);
        SAMPLER(sampler_GradientTex);
        TEXTURE2D(_PatternTex);
        SAMPLER(sampler_PatternTex);
        float3 _GrayscaleParams;
        
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
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
        Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
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
        
        void Unity_Power_float4(float4 A, float4 B, out float4 Out)
        {
            Out = pow(abs(A), B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(abs(A), B);
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        
        inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
        {
            float angle = dot(uv, float2(12.9898, 78.233));
            #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
            #endif
            return frac(sin(angle)*43758.5453);
        }
        
        inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }
        
        
        inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
        
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);
        
            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;
        
            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
        
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
        
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;
        
            Out = t;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Preview_float(float In, out float Out)
        {
            Out = In;
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
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0 = UnityBuildTexture2DStructNoScale(_Tex1);
            float4 _UV_658273acb8454dcabb92b3cb0d9037b7_Out_0 = IN.uv0;
            float4 _Property_485ba34db97544a483c729de69bf57b4_Out_0 = _TileOffMain;
            float _Split_a36669475eef4f3d8f702e9e84067a96_R_1 = _Property_485ba34db97544a483c729de69bf57b4_Out_0[0];
            float _Split_a36669475eef4f3d8f702e9e84067a96_G_2 = _Property_485ba34db97544a483c729de69bf57b4_Out_0[1];
            float _Split_a36669475eef4f3d8f702e9e84067a96_B_3 = _Property_485ba34db97544a483c729de69bf57b4_Out_0[2];
            float _Split_a36669475eef4f3d8f702e9e84067a96_A_4 = _Property_485ba34db97544a483c729de69bf57b4_Out_0[3];
            float2 _Vector2_85e61bf4012f4f4cafc3acb6b5aafa07_Out_0 = float2(_Split_a36669475eef4f3d8f702e9e84067a96_B_3, _Split_a36669475eef4f3d8f702e9e84067a96_A_4);
            float2 _Vector2_f7c300ab8eb3499c966b1877bda20569_Out_0 = float2(_Split_a36669475eef4f3d8f702e9e84067a96_R_1, _Split_a36669475eef4f3d8f702e9e84067a96_G_2);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b;
            float2 _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_658273acb8454dcabb92b3cb0d9037b7_Out_0.xy), _Vector2_85e61bf4012f4f4cafc3acb6b5aafa07_Out_0, 0, _Vector2_f7c300ab8eb3499c966b1877bda20569_Out_0, float2 (0.5, 0.5), _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b, _SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1);
            float4 _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.tex, _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.samplerstate, _Property_1be56dc75dbd4565a8c02a949bfd853f_Out_0.GetTransformedUV(_SSubTransformation_e4e4b0f7f10a4b04a5b56f87a840129b_UVs_1));
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_R_4 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.r;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_G_5 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.g;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_B_6 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.b;
            float _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_A_7 = _SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0.a;
            float _Property_7fc68541cfa146938beed824467054e4_Out_0 = _GammaPower;
            float4 _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2;
            Unity_Power_float4(_SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_RGBA_0, (_Property_7fc68541cfa146938beed824467054e4_Out_0.xxxx), _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2);
            float _Property_7d81c7c052e04980a0ad8092f58358b0_Out_0 = _Intensity;
            UnityTexture2D _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0 = UnityBuildTexture2DStructNoScale(_GradientTex);
            float2 _Vector2_e454b95381a1425787ecfe3110020e80_Out_0 = float2((_Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2).x, 0);
            float4 _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.tex, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.samplerstate, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.GetTransformedUV(_Vector2_e454b95381a1425787ecfe3110020e80_Out_0));
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_R_4 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.r;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_G_5 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.g;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_B_6 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.b;
            float _SampleTexture2D_30411b2df4f245d4a957740156afc616_A_7 = _SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0.a;
            float _Property_4cb633f49b0f4e97b9e99532eb125e85_Out_0 = _GammaPower;
            float4 _Power_2c4f642da8fa411595fd971ab557fc8c_Out_2;
            Unity_Power_float4(_SampleTexture2D_30411b2df4f245d4a957740156afc616_RGBA_0, (_Property_4cb633f49b0f4e97b9e99532eb125e85_Out_0.xxxx), _Power_2c4f642da8fa411595fd971ab557fc8c_Out_2);
            float4 _Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2;
            Unity_Multiply_float4_float4((_Property_7d81c7c052e04980a0ad8092f58358b0_Out_0.xxxx), _Power_2c4f642da8fa411595fd971ab557fc8c_Out_2, _Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2);
            float4 _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1;
            Unity_Saturate_float4(_Multiply_7b61816e08d9458eb1693f2bf5b493fc_Out_2, _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1);
            float4 _Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2;
            Unity_Multiply_float4_float4(_Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2, _Saturate_328e23ab9096422cb1c8272f7bfcd4b6_Out_1, _Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2);
            float _Property_7512a97129e245ba888fa35804c1d815_Out_0 = _Intensity;
            float _Split_a5d72ed0587143aa8dc393d5d107314d_R_1 = _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2[0];
            float _Split_a5d72ed0587143aa8dc393d5d107314d_G_2 = _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2[1];
            float _Split_a5d72ed0587143aa8dc393d5d107314d_B_3 = _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2[2];
            float _Split_a5d72ed0587143aa8dc393d5d107314d_A_4 = _Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2[3];
            float _Multiply_fea7b8fbc5334f3eb767c345e27ccdb2_Out_2;
            Unity_Multiply_float_float(_Split_a5d72ed0587143aa8dc393d5d107314d_G_2, _Split_a5d72ed0587143aa8dc393d5d107314d_A_4, _Multiply_fea7b8fbc5334f3eb767c345e27ccdb2_Out_2);
            UnityTexture2D _Property_a593a90031854918a6778f26b6b1dbad_Out_0 = UnityBuildTexture2DStructNoScale(_PatternTex);
            float4 _UV_ac2812a9682c4c95a28a0163e374b9c3_Out_0 = IN.uv0;
            float4 _Property_0cdde3e59cc244c79851793962e5f194_Out_0 = _TileOff1;
            float _Split_99870dbe8ba647888705b08ccbafaaf4_R_1 = _Property_0cdde3e59cc244c79851793962e5f194_Out_0[0];
            float _Split_99870dbe8ba647888705b08ccbafaaf4_G_2 = _Property_0cdde3e59cc244c79851793962e5f194_Out_0[1];
            float _Split_99870dbe8ba647888705b08ccbafaaf4_B_3 = _Property_0cdde3e59cc244c79851793962e5f194_Out_0[2];
            float _Split_99870dbe8ba647888705b08ccbafaaf4_A_4 = _Property_0cdde3e59cc244c79851793962e5f194_Out_0[3];
            float2 _Vector2_4b20ae7f67934a7fa81c77d3c86adc62_Out_0 = float2(_Split_99870dbe8ba647888705b08ccbafaaf4_B_3, _Split_99870dbe8ba647888705b08ccbafaaf4_A_4);
            float4 _Property_03bfa31b4d8c46cfb57e7e92df06a577_Out_0 = _Speeds;
            float _Split_d2fb2c56215b4c188fc20c5b34917d99_R_1 = _Property_03bfa31b4d8c46cfb57e7e92df06a577_Out_0[0];
            float _Split_d2fb2c56215b4c188fc20c5b34917d99_G_2 = _Property_03bfa31b4d8c46cfb57e7e92df06a577_Out_0[1];
            float _Split_d2fb2c56215b4c188fc20c5b34917d99_B_3 = _Property_03bfa31b4d8c46cfb57e7e92df06a577_Out_0[2];
            float _Split_d2fb2c56215b4c188fc20c5b34917d99_A_4 = _Property_03bfa31b4d8c46cfb57e7e92df06a577_Out_0[3];
            float2 _Vector2_90710a9903874981b622272d68ba3b91_Out_0 = float2(_Split_d2fb2c56215b4c188fc20c5b34917d99_R_1, _Split_d2fb2c56215b4c188fc20c5b34917d99_G_2);
            float2 _Multiply_a1f742c53c084383b13652cfd6e42248_Out_2;
            Unity_Multiply_float2_float2(_Vector2_90710a9903874981b622272d68ba3b91_Out_0, (IN.TimeParameters.x.xx), _Multiply_a1f742c53c084383b13652cfd6e42248_Out_2);
            float2 _Add_4a1e1f7431974824a8b564883621b6b4_Out_2;
            Unity_Add_float2(_Vector2_4b20ae7f67934a7fa81c77d3c86adc62_Out_0, _Multiply_a1f742c53c084383b13652cfd6e42248_Out_2, _Add_4a1e1f7431974824a8b564883621b6b4_Out_2);
            float2 _Vector2_c016e5243b1a4c02a5e1e2ab534599c6_Out_0 = float2(_Split_99870dbe8ba647888705b08ccbafaaf4_R_1, _Split_99870dbe8ba647888705b08ccbafaaf4_G_2);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_61879f6d8bc4424290e57c47942d5d54;
            float2 _SSubTransformation_61879f6d8bc4424290e57c47942d5d54_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_ac2812a9682c4c95a28a0163e374b9c3_Out_0.xy), _Add_4a1e1f7431974824a8b564883621b6b4_Out_2, 0, _Vector2_c016e5243b1a4c02a5e1e2ab534599c6_Out_0, float2 (0.5, 0.5), _SSubTransformation_61879f6d8bc4424290e57c47942d5d54, _SSubTransformation_61879f6d8bc4424290e57c47942d5d54_UVs_1);
            float4 _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_RGBA_0 = SAMPLE_TEXTURE2D(_Property_a593a90031854918a6778f26b6b1dbad_Out_0.tex, _Property_a593a90031854918a6778f26b6b1dbad_Out_0.samplerstate, _Property_a593a90031854918a6778f26b6b1dbad_Out_0.GetTransformedUV(_SSubTransformation_61879f6d8bc4424290e57c47942d5d54_UVs_1));
            float _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_R_4 = _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_RGBA_0.r;
            float _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_G_5 = _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_RGBA_0.g;
            float _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_B_6 = _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_RGBA_0.b;
            float _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_A_7 = _SampleTexture2D_fc61280913ab43f790e43b029c0296b1_RGBA_0.a;
            float _Property_6be7036579f94cb89a0fce1422689dda_Out_0 = _GammaPower;
            float _Power_53aa183a204f46e688bef6595341af37_Out_2;
            Unity_Power_float(_SampleTexture2D_fc61280913ab43f790e43b029c0296b1_R_4, _Property_6be7036579f94cb89a0fce1422689dda_Out_0, _Power_53aa183a204f46e688bef6595341af37_Out_2);
            float _Property_e7b4b74727ab46868f30dcf888934021_Out_0 = _GammaPower;
            float _Power_29a9ba10ce4f497aaa1898132aa9da1a_Out_2;
            Unity_Power_float(_SampleTexture2D_fc61280913ab43f790e43b029c0296b1_R_4, _Property_e7b4b74727ab46868f30dcf888934021_Out_0, _Power_29a9ba10ce4f497aaa1898132aa9da1a_Out_2);
            float _Multiply_04f921bc14a94e0182853c64906a4904_Out_2;
            Unity_Multiply_float_float(_Power_53aa183a204f46e688bef6595341af37_Out_2, _Power_29a9ba10ce4f497aaa1898132aa9da1a_Out_2, _Multiply_04f921bc14a94e0182853c64906a4904_Out_2);
            float _Multiply_1577adf5dc414f3b82615b926288e990_Out_2;
            Unity_Multiply_float_float(_Multiply_04f921bc14a94e0182853c64906a4904_Out_2, 2, _Multiply_1577adf5dc414f3b82615b926288e990_Out_2);
            float _Multiply_5c305f6f5fb7461bbe9bd6ed256068b8_Out_2;
            Unity_Multiply_float_float(_Multiply_fea7b8fbc5334f3eb767c345e27ccdb2_Out_2, _Multiply_1577adf5dc414f3b82615b926288e990_Out_2, _Multiply_5c305f6f5fb7461bbe9bd6ed256068b8_Out_2);
            float _Add_e915026ad36a4cc59a02a667e2643ce2_Out_2;
            Unity_Add_float(_Multiply_fea7b8fbc5334f3eb767c345e27ccdb2_Out_2, _Multiply_5c305f6f5fb7461bbe9bd6ed256068b8_Out_2, _Add_e915026ad36a4cc59a02a667e2643ce2_Out_2);
            float2 _Vector2_34f3fd73fe154655809ea0fcfecb611f_Out_0 = float2(_Add_e915026ad36a4cc59a02a667e2643ce2_Out_2, 0);
            float4 _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.tex, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.samplerstate, _Property_7ae7370e6ed44aa9af134e04d5cefe35_Out_0.GetTransformedUV(_Vector2_34f3fd73fe154655809ea0fcfecb611f_Out_0));
            float _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_R_4 = _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0.r;
            float _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_G_5 = _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0.g;
            float _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_B_6 = _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0.b;
            float _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_A_7 = _SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0.a;
            float _Property_3cc82f6df9a1412f98b6049314276cdd_Out_0 = _GammaPower;
            float4 _Power_7f9f2b03887f4891af2ebd22c19792d5_Out_2;
            Unity_Power_float4(_SampleTexture2D_ccadaf9c67914416a017ce27e00406c8_RGBA_0, (_Property_3cc82f6df9a1412f98b6049314276cdd_Out_0.xxxx), _Power_7f9f2b03887f4891af2ebd22c19792d5_Out_2);
            float4 _Multiply_5639af438e914e169f9928e45eba9dea_Out_2;
            Unity_Multiply_float4_float4((_Property_7512a97129e245ba888fa35804c1d815_Out_0.xxxx), _Power_7f9f2b03887f4891af2ebd22c19792d5_Out_2, _Multiply_5639af438e914e169f9928e45eba9dea_Out_2);
            float4 _Saturate_5d3302a6ba7341148dcd3d88988e94f2_Out_1;
            Unity_Saturate_float4(_Multiply_5639af438e914e169f9928e45eba9dea_Out_2, _Saturate_5d3302a6ba7341148dcd3d88988e94f2_Out_1);
            float4 _Multiply_1d4403b0654e461f806859019da91ec6_Out_2;
            Unity_Multiply_float4_float4(_Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2, _Saturate_5d3302a6ba7341148dcd3d88988e94f2_Out_1, _Multiply_1d4403b0654e461f806859019da91ec6_Out_2);
            float4 _Add_071ce6e538624f968222f1a9ef481551_Out_2;
            Unity_Add_float4(_Multiply_5d2296bc4c6749749a95fc0d0e9ba3e3_Out_2, _Multiply_1d4403b0654e461f806859019da91ec6_Out_2, _Add_071ce6e538624f968222f1a9ef481551_Out_2);
            float4 _UV_470ad0e393594c199950d6fe9355f87a_Out_0 = IN.uv0;
            float _Split_a6b2ddd906bc463ca0db3e62be7fa525_R_1 = _UV_470ad0e393594c199950d6fe9355f87a_Out_0[0];
            float _Split_a6b2ddd906bc463ca0db3e62be7fa525_G_2 = _UV_470ad0e393594c199950d6fe9355f87a_Out_0[1];
            float _Split_a6b2ddd906bc463ca0db3e62be7fa525_B_3 = _UV_470ad0e393594c199950d6fe9355f87a_Out_0[2];
            float _Split_a6b2ddd906bc463ca0db3e62be7fa525_A_4 = _UV_470ad0e393594c199950d6fe9355f87a_Out_0[3];
            float _Property_14abe57ad8194690bdf24aa8a4c71990_Out_0 = _DissolveAmount;
            float _Subtract_5f5b8a745574490d93416df70580d9e6_Out_2;
            Unity_Subtract_float(_Split_a6b2ddd906bc463ca0db3e62be7fa525_G_2, _Property_14abe57ad8194690bdf24aa8a4c71990_Out_0, _Subtract_5f5b8a745574490d93416df70580d9e6_Out_2);
            float _SimpleNoise_89bb170de98a4dab8e169452810ed6a6_Out_2;
            Unity_SimpleNoise_float(IN.uv0.xy, 80, _SimpleNoise_89bb170de98a4dab8e169452810ed6a6_Out_2);
            float _Property_45a12e789527410491b063c1b7652989_Out_0 = _Fade;
            float2 _Vector2_683dda6f109845ecbb84062137c98084_Out_0 = float2(0.01, _Property_45a12e789527410491b063c1b7652989_Out_0);
            float _Remap_8a76756ffa434771a711134e4e849703_Out_3;
            Unity_Remap_float(_SimpleNoise_89bb170de98a4dab8e169452810ed6a6_Out_2, float2 (0, 1), _Vector2_683dda6f109845ecbb84062137c98084_Out_0, _Remap_8a76756ffa434771a711134e4e849703_Out_3);
            float _Divide_7c15f4fa11f342978b6fb4a3e60b168c_Out_2;
            Unity_Divide_float(_Subtract_5f5b8a745574490d93416df70580d9e6_Out_2, _Remap_8a76756ffa434771a711134e4e849703_Out_3, _Divide_7c15f4fa11f342978b6fb4a3e60b168c_Out_2);
            float _Saturate_63a013a323804c9a8995252834ce1db3_Out_1;
            Unity_Saturate_float(_Divide_7c15f4fa11f342978b6fb4a3e60b168c_Out_2, _Saturate_63a013a323804c9a8995252834ce1db3_Out_1);
            float _Remap_5953af6e3d394692b65ef2365a1aefaf_Out_3;
            Unity_Remap_float(_Saturate_63a013a323804c9a8995252834ce1db3_Out_1, float2 (0, 1), float2 (-5, 1), _Remap_5953af6e3d394692b65ef2365a1aefaf_Out_3);
            float _Saturate_8659c57d76444970adf6ae3e729f07f4_Out_1;
            Unity_Saturate_float(_Remap_5953af6e3d394692b65ef2365a1aefaf_Out_3, _Saturate_8659c57d76444970adf6ae3e729f07f4_Out_1);
            float _Property_08fceb1d821a478a8acaf34659525fc4_Out_0 = _EdgeWidth;
            float _Subtract_216fa765937341389afb9222ef74763d_Out_2;
            Unity_Subtract_float(_Divide_7c15f4fa11f342978b6fb4a3e60b168c_Out_2, _Property_08fceb1d821a478a8acaf34659525fc4_Out_0, _Subtract_216fa765937341389afb9222ef74763d_Out_2);
            float _Saturate_211651a5854946048a482159df440860_Out_1;
            Unity_Saturate_float(_Subtract_216fa765937341389afb9222ef74763d_Out_2, _Saturate_211651a5854946048a482159df440860_Out_1);
            float _Remap_1912ef41966f45dab0b4300c93f06084_Out_3;
            Unity_Remap_float(_Saturate_211651a5854946048a482159df440860_Out_1, float2 (0, 1), float2 (-5, 1), _Remap_1912ef41966f45dab0b4300c93f06084_Out_3);
            float _Saturate_916ebe57fff1492cb8639336020acf16_Out_1;
            Unity_Saturate_float(_Remap_1912ef41966f45dab0b4300c93f06084_Out_3, _Saturate_916ebe57fff1492cb8639336020acf16_Out_1);
            float _Subtract_95a1ee8dec0f48c9bbb5e6c5c93346ab_Out_2;
            Unity_Subtract_float(_Saturate_8659c57d76444970adf6ae3e729f07f4_Out_1, _Saturate_916ebe57fff1492cb8639336020acf16_Out_1, _Subtract_95a1ee8dec0f48c9bbb5e6c5c93346ab_Out_2);
            float _Saturate_e961eb05fc9245aba50d12c06f851ffb_Out_1;
            Unity_Saturate_float(_Subtract_95a1ee8dec0f48c9bbb5e6c5c93346ab_Out_2, _Saturate_e961eb05fc9245aba50d12c06f851ffb_Out_1);
            float4 _Property_2bcb1c43210f49fba1b0265b1dbcddfe_Out_0 = IsGammaSpace() ? LinearToSRGB(_EdgeColor) : _EdgeColor;
            float4 _Multiply_5176ebe640264637bc48053e21b3325b_Out_2;
            Unity_Multiply_float4_float4((_Saturate_e961eb05fc9245aba50d12c06f851ffb_Out_1.xxxx), _Property_2bcb1c43210f49fba1b0265b1dbcddfe_Out_0, _Multiply_5176ebe640264637bc48053e21b3325b_Out_2);
            float4 _Add_4d234da9c4dc473e8701d8b7094031f0_Out_2;
            Unity_Add_float4(_Add_071ce6e538624f968222f1a9ef481551_Out_2, _Multiply_5176ebe640264637bc48053e21b3325b_Out_2, _Add_4d234da9c4dc473e8701d8b7094031f0_Out_2);
            float _Property_47a050b4601e4c3aa7e5ea93da2a8bd4_Out_0 = _Grayscale;
            float _OneMinus_82ee1ac21866420ebb1bb4eb3bcd81ed_Out_1;
            Unity_OneMinus_float(_Property_47a050b4601e4c3aa7e5ea93da2a8bd4_Out_0, _OneMinus_82ee1ac21866420ebb1bb4eb3bcd81ed_Out_1);
            float4 _Multiply_28c52266a2724fd39efe54814cb825b7_Out_2;
            Unity_Multiply_float4_float4(_Add_4d234da9c4dc473e8701d8b7094031f0_Out_2, (_OneMinus_82ee1ac21866420ebb1bb4eb3bcd81ed_Out_1.xxxx), _Multiply_28c52266a2724fd39efe54814cb825b7_Out_2);
            float4 _Multiply_2ea9aeb55eb546c9889425cc986dd0b4_Out_2;
            Unity_Multiply_float4_float4(_Power_4443f1b724e6463fa0e8c3a6194b570a_Out_2, (_Property_47a050b4601e4c3aa7e5ea93da2a8bd4_Out_0.xxxx), _Multiply_2ea9aeb55eb546c9889425cc986dd0b4_Out_2);
            float4 _Add_a1da4d6803a642c2a3c1356cf32ba8b7_Out_2;
            Unity_Add_float4(_Multiply_28c52266a2724fd39efe54814cb825b7_Out_2, _Multiply_2ea9aeb55eb546c9889425cc986dd0b4_Out_2, _Add_a1da4d6803a642c2a3c1356cf32ba8b7_Out_2);
            float _Preview_b6c95696660e486fbbef99b8ff2d5c7b_Out_1;
            Unity_Preview_float(_SampleTexture2D_2fc6b6a95a5a439a873608b18e91158a_A_7, _Preview_b6c95696660e486fbbef99b8ff2d5c7b_Out_1);
            float _Property_6c75aa1e0ba5457a9b51ef71335c8350_Out_0 = _Direction;
            float _Add_e133a4c3868941d7bc0d147320f6edf3_Out_2;
            Unity_Add_float(_Saturate_8659c57d76444970adf6ae3e729f07f4_Out_1, _Saturate_e961eb05fc9245aba50d12c06f851ffb_Out_1, _Add_e133a4c3868941d7bc0d147320f6edf3_Out_2);
            float _Multiply_d73ea7e58924491aa2d97387b63bc903_Out_2;
            Unity_Multiply_float_float(_Property_6c75aa1e0ba5457a9b51ef71335c8350_Out_0, _Add_e133a4c3868941d7bc0d147320f6edf3_Out_2, _Multiply_d73ea7e58924491aa2d97387b63bc903_Out_2);
            float _Add_4925bc7a1178494e90ac94448d9cf1ea_Out_2;
            Unity_Add_float(_Saturate_e961eb05fc9245aba50d12c06f851ffb_Out_1, _Saturate_916ebe57fff1492cb8639336020acf16_Out_1, _Add_4925bc7a1178494e90ac94448d9cf1ea_Out_2);
            float _OneMinus_3150948fd79c48178aea3cae49d123fc_Out_1;
            Unity_OneMinus_float(_Property_6c75aa1e0ba5457a9b51ef71335c8350_Out_0, _OneMinus_3150948fd79c48178aea3cae49d123fc_Out_1);
            float _Multiply_cb46acfe29924178abd4b918d0b29098_Out_2;
            Unity_Multiply_float_float(_Add_4925bc7a1178494e90ac94448d9cf1ea_Out_2, _OneMinus_3150948fd79c48178aea3cae49d123fc_Out_1, _Multiply_cb46acfe29924178abd4b918d0b29098_Out_2);
            float _Add_92d2df02668f442bb472188f1bea3d5e_Out_2;
            Unity_Add_float(_Multiply_d73ea7e58924491aa2d97387b63bc903_Out_2, _Multiply_cb46acfe29924178abd4b918d0b29098_Out_2, _Add_92d2df02668f442bb472188f1bea3d5e_Out_2);
            float _Saturate_fe800e2ac534411c8f5510b2112bebf9_Out_1;
            Unity_Saturate_float(_Add_92d2df02668f442bb472188f1bea3d5e_Out_2, _Saturate_fe800e2ac534411c8f5510b2112bebf9_Out_1);
            float _Saturate_cf70d7db6e8b44e4bb63d6610907e7ab_Out_1;
            Unity_Saturate_float(_Saturate_fe800e2ac534411c8f5510b2112bebf9_Out_1, _Saturate_cf70d7db6e8b44e4bb63d6610907e7ab_Out_1);
            float _Property_fb1e965e1ea94ddcaac7fe5145bef245_Out_0 = _Direction;
            float _Multiply_6f497a0d5c994dbbab9529f0187a34bd_Out_2;
            Unity_Multiply_float_float(_Saturate_cf70d7db6e8b44e4bb63d6610907e7ab_Out_1, _Property_fb1e965e1ea94ddcaac7fe5145bef245_Out_0, _Multiply_6f497a0d5c994dbbab9529f0187a34bd_Out_2);
            float _OneMinus_11d67fc61a44438899b1327898258dcc_Out_1;
            Unity_OneMinus_float(_Saturate_cf70d7db6e8b44e4bb63d6610907e7ab_Out_1, _OneMinus_11d67fc61a44438899b1327898258dcc_Out_1);
            float _OneMinus_9d1011b170cd4786a61461cc083259c0_Out_1;
            Unity_OneMinus_float(_Property_fb1e965e1ea94ddcaac7fe5145bef245_Out_0, _OneMinus_9d1011b170cd4786a61461cc083259c0_Out_1);
            float _Multiply_26b4de71548a48ff919a70073df0f22f_Out_2;
            Unity_Multiply_float_float(_OneMinus_11d67fc61a44438899b1327898258dcc_Out_1, _OneMinus_9d1011b170cd4786a61461cc083259c0_Out_1, _Multiply_26b4de71548a48ff919a70073df0f22f_Out_2);
            float _Add_9971805558c6447dac788698f01a13c0_Out_2;
            Unity_Add_float(_Multiply_6f497a0d5c994dbbab9529f0187a34bd_Out_2, _Multiply_26b4de71548a48ff919a70073df0f22f_Out_2, _Add_9971805558c6447dac788698f01a13c0_Out_2);
            float _Multiply_7864df4aa8904e338ebe9d2b89ef2f63_Out_2;
            Unity_Multiply_float_float(_Preview_b6c95696660e486fbbef99b8ff2d5c7b_Out_1, _Add_9971805558c6447dac788698f01a13c0_Out_2, _Multiply_7864df4aa8904e338ebe9d2b89ef2f63_Out_2);
            surface.BaseColor = (_Add_a1da4d6803a642c2a3c1356cf32ba8b7_Out_2.xyz);
            surface.Alpha = _Multiply_7864df4aa8904e338ebe9d2b89ef2f63_Out_2;
            surface.AlphaClipThreshold = 0.5;
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
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}