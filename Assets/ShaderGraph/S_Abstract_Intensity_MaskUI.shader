Shader "Unlit/S_Abstract_Intensity_MaskUI"
{
    Properties
    {
        [NoScaleOffset]Texture2D_00b6fd5f99c646f5b2d9af89ed5c3cd3("MainTex", 2D) = "white" {}
        [NoScaleOffset]_GradientTex("GradientTex", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
        Vector2_36ba532bb9a4439aaa2eeb176be24622("Tiling_1", Vector) = (1, 1, 0, 0)
        Vector2_4b5fbef70e9845b29b9e2c7d3e1a630c("Tiling_2", Vector) = (0.5, 0.5, 0, 0)
        _Speed_1("Speed_1", Vector) = (0, -0.1, 0, 0)
        Vector1_8c4e5f592c464be18c40dec560ca9bfb("Mask_Strength", Range(-1, 1)) = 0
        Vector1_9354f6a4a90e4f5db66a13c176e018c9("MaskDirection_1", Float) = 0
        Vector1_1("MaskDirection_2", Float) = 0
        _Intensity("Intensity", Range(0, 10)) = 1
        _GammaPower("GammaPower", Range(0.01, 0.4545)) = 0.4545
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
            //"RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            //"UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
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
            Name "Pass"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Cull Back
		Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha //Blend alpha
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
        float4 Texture2D_00b6fd5f99c646f5b2d9af89ed5c3cd3_TexelSize;
        float2 Vector2_36ba532bb9a4439aaa2eeb176be24622;
        float2 Vector2_4b5fbef70e9845b29b9e2c7d3e1a630c;
        float2 _Speed_1;
        float Vector1_8c4e5f592c464be18c40dec560ca9bfb;
        float Vector1_9354f6a4a90e4f5db66a13c176e018c9;
        float Vector1_1;
        float _Intensity;
        float4 _GradientTex_TexelSize;
        float4 _MaskTex_TexelSize;
        float _GammaPower;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(Texture2D_00b6fd5f99c646f5b2d9af89ed5c3cd3);
        SAMPLER(samplerTexture2D_00b6fd5f99c646f5b2d9af89ed5c3cd3);
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
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        void Unity_Power_float4(float4 A, float4 B, out float4 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_8903f18cdc3744828747b60000edffb4_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_00b6fd5f99c646f5b2d9af89ed5c3cd3);
            float4 _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0 = IN.uv0;
            float4 _Add_633565bf494e4ed88aba7d3f86f7bd25_Out_2;
            Unity_Add_float4(_UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0, _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0, _Add_633565bf494e4ed88aba7d3f86f7bd25_Out_2);
            float2 _Property_f8b4263fca244f37a5e3227e221c4aae_Out_0 = Vector2_36ba532bb9a4439aaa2eeb176be24622;
            float2 _Property_4e9b55f8a883443db42a0bfdc8c36e64_Out_0 = _Speed_1;
            float2 _Multiply_ad608a54705741178c96a7bf6ca9b4a4_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_4e9b55f8a883443db42a0bfdc8c36e64_Out_0, _Multiply_ad608a54705741178c96a7bf6ca9b4a4_Out_2);
            float2 _TilingAndOffset_45b59439fe624075ae755fca24b8e3bb_Out_3;
            Unity_TilingAndOffset_float((_Add_633565bf494e4ed88aba7d3f86f7bd25_Out_2.xy), _Property_f8b4263fca244f37a5e3227e221c4aae_Out_0, _Multiply_ad608a54705741178c96a7bf6ca9b4a4_Out_2, _TilingAndOffset_45b59439fe624075ae755fca24b8e3bb_Out_3);
            float4 _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8903f18cdc3744828747b60000edffb4_Out_0.tex, _Property_8903f18cdc3744828747b60000edffb4_Out_0.samplerstate, _Property_8903f18cdc3744828747b60000edffb4_Out_0.GetTransformedUV(_TilingAndOffset_45b59439fe624075ae755fca24b8e3bb_Out_3));
            float _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_R_4 = _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_RGBA_0.r;
            float _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_G_5 = _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_RGBA_0.g;
            float _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_B_6 = _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_RGBA_0.b;
            float _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_A_7 = _SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_RGBA_0.a;
            float _Property_9783d39122dd45e5ba1c9a8ebb75035b_Out_0 = _GammaPower;
            float _Power_ee33555c85494fc7adf466ec7b64e3f5_Out_2;
            Unity_Power_float(_SampleTexture2D_509cc477ced14e739e0ef850f0b1e860_R_4, _Property_9783d39122dd45e5ba1c9a8ebb75035b_Out_0, _Power_ee33555c85494fc7adf466ec7b64e3f5_Out_2);
            float2 _Property_2578851d105f48adb8889038b495d88d_Out_0 = Vector2_4b5fbef70e9845b29b9e2c7d3e1a630c;
            float2 _Multiply_44a93a2f34ca4377afa65b5e04cac25b_Out_2;
            Unity_Multiply_float2_float2(_Property_4e9b55f8a883443db42a0bfdc8c36e64_Out_0, float2(0.5, 0.8), _Multiply_44a93a2f34ca4377afa65b5e04cac25b_Out_2);
            float2 _Multiply_6dc248215ee041d49b02c1d1ba4f5cfd_Out_2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Multiply_44a93a2f34ca4377afa65b5e04cac25b_Out_2, _Multiply_6dc248215ee041d49b02c1d1ba4f5cfd_Out_2);
            float2 _TilingAndOffset_64dd52593b9e48a0aac89b23f53e3c8f_Out_3;
            Unity_TilingAndOffset_float((_Add_633565bf494e4ed88aba7d3f86f7bd25_Out_2.xy), _Property_2578851d105f48adb8889038b495d88d_Out_0, _Multiply_6dc248215ee041d49b02c1d1ba4f5cfd_Out_2, _TilingAndOffset_64dd52593b9e48a0aac89b23f53e3c8f_Out_3);
            float4 _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_RGBA_0 = SAMPLE_TEXTURE2D(_Property_8903f18cdc3744828747b60000edffb4_Out_0.tex, _Property_8903f18cdc3744828747b60000edffb4_Out_0.samplerstate, _Property_8903f18cdc3744828747b60000edffb4_Out_0.GetTransformedUV(_TilingAndOffset_64dd52593b9e48a0aac89b23f53e3c8f_Out_3));
            float _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_R_4 = _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_RGBA_0.r;
            float _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_G_5 = _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_RGBA_0.g;
            float _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_B_6 = _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_RGBA_0.b;
            float _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_A_7 = _SampleTexture2D_258622f4a9b345e394ee4193e7b88225_RGBA_0.a;
            float _Property_a3fe13c9637c412f80f7cb6fb4a98b1c_Out_0 = _GammaPower;
            float _Power_b5bf0810323e412d8569a4f363e0548a_Out_2;
            Unity_Power_float(_SampleTexture2D_258622f4a9b345e394ee4193e7b88225_R_4, _Property_a3fe13c9637c412f80f7cb6fb4a98b1c_Out_0, _Power_b5bf0810323e412d8569a4f363e0548a_Out_2);
            float _Multiply_49d578004ede4904b10cf52037219517_Out_2;
            Unity_Multiply_float_float(_Power_ee33555c85494fc7adf466ec7b64e3f5_Out_2, _Power_b5bf0810323e412d8569a4f363e0548a_Out_2, _Multiply_49d578004ede4904b10cf52037219517_Out_2);
            float _Property_8de7c306919844bd9d0eb01a671f5ea3_Out_0 = Vector1_1;
            float _OneMinus_99e1fd860391470cb4b87bae2fcfffbd_Out_1;
            Unity_OneMinus_float(_Property_8de7c306919844bd9d0eb01a671f5ea3_Out_0, _OneMinus_99e1fd860391470cb4b87bae2fcfffbd_Out_1);
            float _Split_ff81224cdcdc49949696229fbf9fd599_R_1 = _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0[0];
            float _Split_ff81224cdcdc49949696229fbf9fd599_G_2 = _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0[1];
            float _Split_ff81224cdcdc49949696229fbf9fd599_B_3 = _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0[2];
            float _Split_ff81224cdcdc49949696229fbf9fd599_A_4 = _UV_e5e08be2f1174c3cb736ba3318ba4187_Out_0[3];
            float _Property_dd870df591974f918e41f491cc87cb17_Out_0 = Vector1_9354f6a4a90e4f5db66a13c176e018c9;
            float _Multiply_5b52203288394d049190a3e48e0fc574_Out_2;
            Unity_Multiply_float_float(_Split_ff81224cdcdc49949696229fbf9fd599_R_1, _Property_dd870df591974f918e41f491cc87cb17_Out_0, _Multiply_5b52203288394d049190a3e48e0fc574_Out_2);
            float _OneMinus_3866895a5dc947e08a846d381ce63273_Out_1;
            Unity_OneMinus_float(_Property_dd870df591974f918e41f491cc87cb17_Out_0, _OneMinus_3866895a5dc947e08a846d381ce63273_Out_1);
            float _Multiply_e40b552901914022a41c67a0a097c2b7_Out_2;
            Unity_Multiply_float_float(_OneMinus_3866895a5dc947e08a846d381ce63273_Out_1, _Split_ff81224cdcdc49949696229fbf9fd599_G_2, _Multiply_e40b552901914022a41c67a0a097c2b7_Out_2);
            float _Add_2bc75289de0f4b4a84bb36ebfa448eaa_Out_2;
            Unity_Add_float(_Multiply_5b52203288394d049190a3e48e0fc574_Out_2, _Multiply_e40b552901914022a41c67a0a097c2b7_Out_2, _Add_2bc75289de0f4b4a84bb36ebfa448eaa_Out_2);
            float _Property_09b53690ad5c4e24a2e3d4ae83b197c5_Out_0 = Vector1_8c4e5f592c464be18c40dec560ca9bfb;
            float _Add_5dd063bad8b84a6f8769955f3a039063_Out_2;
            Unity_Add_float(_Add_2bc75289de0f4b4a84bb36ebfa448eaa_Out_2, _Property_09b53690ad5c4e24a2e3d4ae83b197c5_Out_0, _Add_5dd063bad8b84a6f8769955f3a039063_Out_2);
            float _OneMinus_ebf280747d2d4d09a55a75f16fedf551_Out_1;
            Unity_OneMinus_float(_Add_5dd063bad8b84a6f8769955f3a039063_Out_2, _OneMinus_ebf280747d2d4d09a55a75f16fedf551_Out_1);
            float _Multiply_ac1c66684ee94659b2bcaa1c0ef5c1a7_Out_2;
            Unity_Multiply_float_float(_OneMinus_99e1fd860391470cb4b87bae2fcfffbd_Out_1, _OneMinus_ebf280747d2d4d09a55a75f16fedf551_Out_1, _Multiply_ac1c66684ee94659b2bcaa1c0ef5c1a7_Out_2);
            float _Multiply_9ea63bcdce724dd6a3e0440d1c23c6fd_Out_2;
            Unity_Multiply_float_float(_Property_8de7c306919844bd9d0eb01a671f5ea3_Out_0, _Add_5dd063bad8b84a6f8769955f3a039063_Out_2, _Multiply_9ea63bcdce724dd6a3e0440d1c23c6fd_Out_2);
            float _Add_c2c6ca029ca145089fa69fb3382a95e9_Out_2;
            Unity_Add_float(_Multiply_ac1c66684ee94659b2bcaa1c0ef5c1a7_Out_2, _Multiply_9ea63bcdce724dd6a3e0440d1c23c6fd_Out_2, _Add_c2c6ca029ca145089fa69fb3382a95e9_Out_2);
            float _Multiply_b571369076d84042917fca9f9a1cb587_Out_2;
            Unity_Multiply_float_float(_Multiply_49d578004ede4904b10cf52037219517_Out_2, _Add_c2c6ca029ca145089fa69fb3382a95e9_Out_2, _Multiply_b571369076d84042917fca9f9a1cb587_Out_2);
            float _Smoothstep_2f6102853df14a4a9327fa24fbdcafd9_Out_3;
            Unity_Smoothstep_float(0, 1, _Multiply_b571369076d84042917fca9f9a1cb587_Out_2, _Smoothstep_2f6102853df14a4a9327fa24fbdcafd9_Out_3);
            float _OneMinus_29803246a77a4fbbb61f9fa518353522_Out_1;
            Unity_OneMinus_float(_Multiply_b571369076d84042917fca9f9a1cb587_Out_2, _OneMinus_29803246a77a4fbbb61f9fa518353522_Out_1);
            float _Smoothstep_031cc7c78e394ddb98fe7e976e58d44c_Out_3;
            Unity_Smoothstep_float(0, 1, _OneMinus_29803246a77a4fbbb61f9fa518353522_Out_1, _Smoothstep_031cc7c78e394ddb98fe7e976e58d44c_Out_3);
            float _Multiply_98dc3fcfad694617bc377fba438c1bd3_Out_2;
            Unity_Multiply_float_float(_Smoothstep_2f6102853df14a4a9327fa24fbdcafd9_Out_3, _Smoothstep_031cc7c78e394ddb98fe7e976e58d44c_Out_3, _Multiply_98dc3fcfad694617bc377fba438c1bd3_Out_2);
            UnityTexture2D _Property_1eada8c1119d4d00b07330a2cfa06ad0_Out_0 = UnityBuildTexture2DStructNoScale(_GradientTex);
            float4 _SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0 = SAMPLE_TEXTURE2D(_Property_1eada8c1119d4d00b07330a2cfa06ad0_Out_0.tex, _Property_1eada8c1119d4d00b07330a2cfa06ad0_Out_0.samplerstate, _Property_1eada8c1119d4d00b07330a2cfa06ad0_Out_0.GetTransformedUV((_Add_5dd063bad8b84a6f8769955f3a039063_Out_2.xx)));
            float _SampleTexture2D_64687303078643f58877e19b670fee17_R_4 = _SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0.r;
            float _SampleTexture2D_64687303078643f58877e19b670fee17_G_5 = _SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0.g;
            float _SampleTexture2D_64687303078643f58877e19b670fee17_B_6 = _SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0.b;
            float _SampleTexture2D_64687303078643f58877e19b670fee17_A_7 = _SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0.a;
            float _Property_cfb26249217f4bf0b295bdbad58c25fe_Out_0 = _GammaPower;
            float4 _Power_795fec35e1274a6db99a1ea4233d1b29_Out_2;
            Unity_Power_float4(_SampleTexture2D_64687303078643f58877e19b670fee17_RGBA_0, (_Property_cfb26249217f4bf0b295bdbad58c25fe_Out_0.xxxx), _Power_795fec35e1274a6db99a1ea4233d1b29_Out_2);
            float4 _Multiply_98220aa786a642e9a0e65efa6f2b49d5_Out_2;
            Unity_Multiply_float4_float4((_Multiply_98dc3fcfad694617bc377fba438c1bd3_Out_2.xxxx), _Power_795fec35e1274a6db99a1ea4233d1b29_Out_2, _Multiply_98220aa786a642e9a0e65efa6f2b49d5_Out_2);
            float _Property_73f6a5111ca14f17a70eea053fb32e28_Out_0 = _Intensity;
            float4 _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2;
            Unity_Multiply_float4_float4(_Multiply_98220aa786a642e9a0e65efa6f2b49d5_Out_2, (_Property_73f6a5111ca14f17a70eea053fb32e28_Out_0.xxxx), _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2);
            float _Split_f1244d9fd8aa42848f637c4dce3eb460_R_1 = _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2[0];
            float _Split_f1244d9fd8aa42848f637c4dce3eb460_G_2 = _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2[1];
            float _Split_f1244d9fd8aa42848f637c4dce3eb460_B_3 = _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2[2];
            float _Split_f1244d9fd8aa42848f637c4dce3eb460_A_4 = _Multiply_94bbad2bff8849e0b24c436110d03106_Out_2[3];
            UnityTexture2D _Property_2813815b1ae84549b93cd3d9bc8e8eda_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_49371d678b9c449496232e1dcd78add6_RGBA_0 = SAMPLE_TEXTURE2D(_Property_2813815b1ae84549b93cd3d9bc8e8eda_Out_0.tex, _Property_2813815b1ae84549b93cd3d9bc8e8eda_Out_0.samplerstate, _Property_2813815b1ae84549b93cd3d9bc8e8eda_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_49371d678b9c449496232e1dcd78add6_R_4 = _SampleTexture2D_49371d678b9c449496232e1dcd78add6_RGBA_0.r;
            float _SampleTexture2D_49371d678b9c449496232e1dcd78add6_G_5 = _SampleTexture2D_49371d678b9c449496232e1dcd78add6_RGBA_0.g;
            float _SampleTexture2D_49371d678b9c449496232e1dcd78add6_B_6 = _SampleTexture2D_49371d678b9c449496232e1dcd78add6_RGBA_0.b;
            float _SampleTexture2D_49371d678b9c449496232e1dcd78add6_A_7 = _SampleTexture2D_49371d678b9c449496232e1dcd78add6_RGBA_0.a;
            float _Property_74e5d32d38bb425f8d80e732a1905cf2_Out_0 = _GammaPower;
            float _Power_1eb7ac0525594a21ac78f0772d8dbe54_Out_2;
            Unity_Power_float(_SampleTexture2D_49371d678b9c449496232e1dcd78add6_R_4, _Property_74e5d32d38bb425f8d80e732a1905cf2_Out_0, _Power_1eb7ac0525594a21ac78f0772d8dbe54_Out_2);
            float _Multiply_dfdedf6bae3a40d1ac7bf7d4e163059b_Out_2;
            Unity_Multiply_float_float(_Split_f1244d9fd8aa42848f637c4dce3eb460_A_4, _Power_1eb7ac0525594a21ac78f0772d8dbe54_Out_2, _Multiply_dfdedf6bae3a40d1ac7bf7d4e163059b_Out_2);
            surface.BaseColor = (_Multiply_94bbad2bff8849e0b24c436110d03106_Out_2.xyz);
            surface.Alpha = _Multiply_dfdedf6bae3a40d1ac7bf7d4e163059b_Out_2;
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