Shader "Unlit/S_SpinNotBlur_UI"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [NoScaleOffset]_SecondTex("SecondTex", 2D) = "white" {}
        [NoScaleOffset]_GradientTex("GradientTex", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
        _Intensity("Intensity", Range(-1, 10)) = 2
        _Rotation("Rotation", Range(-0.5, 0.5)) = 0.1
        _SecondRotation("SecondRotation", Range(-0.5, 0.5)) = -0.1
        _Scale_1_2("Scale_1_2", Vector) = (1, 1, 1, 1)
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
        float4 _MainTex_TexelSize;
        float4 _MaskTex_TexelSize;
        float _Intensity;
        float _Rotation;
        float _SecondRotation;
        float4 _SecondTex_TexelSize;
        float4 _Scale_1_2;
        float4 _GradientTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
        TEXTURE2D(_SecondTex);
        SAMPLER(sampler_SecondTex);
        TEXTURE2D(_GradientTex);
        SAMPLER(sampler_GradientTex);
        
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
        
        void Unity_Cosine_float(float In, out float Out)
        {
            Out = cos(In);
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
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
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Saturate_float4(float4 In, out float4 Out)
        {
            Out = saturate(In);
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
            float _Property_35b85191b8df4fa39c6b3e5c7a3b703a_Out_0 = _Intensity;
            UnityTexture2D _Property_e0da15f5ff82449eb764055230e5e25a_Out_0 = UnityBuildTexture2DStructNoScale(_GradientTex);
            UnityTexture2D _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_179f0480dd5144c1a5d6bb214eb4bb6a_Out_0 = IN.uv0;
            float _Property_f66c1fd5dd8d44e7a71751fc7b82e867_Out_0 = _Rotation;
            float _Multiply_7794f3aaccf241eea58e70093dd34d67_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_f66c1fd5dd8d44e7a71751fc7b82e867_Out_0, _Multiply_7794f3aaccf241eea58e70093dd34d67_Out_2);
            float4 _Property_bc77edf465b740f6bbb6e0db0d30a6a3_Out_0 = _Scale_1_2;
            float _Split_69367000cc294ad4b5e5d022809891a3_R_1 = _Property_bc77edf465b740f6bbb6e0db0d30a6a3_Out_0[0];
            float _Split_69367000cc294ad4b5e5d022809891a3_G_2 = _Property_bc77edf465b740f6bbb6e0db0d30a6a3_Out_0[1];
            float _Split_69367000cc294ad4b5e5d022809891a3_B_3 = _Property_bc77edf465b740f6bbb6e0db0d30a6a3_Out_0[2];
            float _Split_69367000cc294ad4b5e5d022809891a3_A_4 = _Property_bc77edf465b740f6bbb6e0db0d30a6a3_Out_0[3];
            float2 _Vector2_5d1dfa2d31784d5dab8ba80a4f8ccdb1_Out_0 = float2(_Split_69367000cc294ad4b5e5d022809891a3_R_1, _Split_69367000cc294ad4b5e5d022809891a3_G_2);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_58d04ed5cfea46bfa450e5d8b7025662;
            float2 _SSubTransformation_58d04ed5cfea46bfa450e5d8b7025662_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_179f0480dd5144c1a5d6bb214eb4bb6a_Out_0.xy), float2 (0, 0), _Multiply_7794f3aaccf241eea58e70093dd34d67_Out_2, _Vector2_5d1dfa2d31784d5dab8ba80a4f8ccdb1_Out_0, float2 (0.5, 0.5), _SSubTransformation_58d04ed5cfea46bfa450e5d8b7025662, _SSubTransformation_58d04ed5cfea46bfa450e5d8b7025662_UVs_1);
            float4 _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.tex, _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.samplerstate, _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.GetTransformedUV(_SSubTransformation_58d04ed5cfea46bfa450e5d8b7025662_UVs_1));
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_R_4 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.r;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_G_5 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.g;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_B_6 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.b;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_A_7 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.a;
            UnityTexture2D _Property_f0b6c8ba1dec42ab82a73066b808a101_Out_0 = UnityBuildTexture2DStructNoScale(_SecondTex);
            float _Property_8838eaa6b4094bfaa467e760b48c6537_Out_0 = _SecondRotation;
            float _Multiply_5fde6615d8a64e0d8c59ac65fbc51a1f_Out_2;
            Unity_Multiply_float_float(IN.TimeParameters.x, _Property_8838eaa6b4094bfaa467e760b48c6537_Out_0, _Multiply_5fde6615d8a64e0d8c59ac65fbc51a1f_Out_2);
            float2 _Vector2_53239e038054414c9b91dcad1f05bc71_Out_0 = float2(_Split_69367000cc294ad4b5e5d022809891a3_B_3, _Split_69367000cc294ad4b5e5d022809891a3_A_4);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_18fd2163f82c457e910ef086937feef0;
            float2 _SSubTransformation_18fd2163f82c457e910ef086937feef0_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_179f0480dd5144c1a5d6bb214eb4bb6a_Out_0.xy), float2 (0, 0), _Multiply_5fde6615d8a64e0d8c59ac65fbc51a1f_Out_2, _Vector2_53239e038054414c9b91dcad1f05bc71_Out_0, float2 (0.5, 0.5), _SSubTransformation_18fd2163f82c457e910ef086937feef0, _SSubTransformation_18fd2163f82c457e910ef086937feef0_UVs_1);
            float4 _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0 = SAMPLE_TEXTURE2D(_Property_f0b6c8ba1dec42ab82a73066b808a101_Out_0.tex, _Property_f0b6c8ba1dec42ab82a73066b808a101_Out_0.samplerstate, _Property_f0b6c8ba1dec42ab82a73066b808a101_Out_0.GetTransformedUV(_SSubTransformation_18fd2163f82c457e910ef086937feef0_UVs_1));
            float _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_R_4 = _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0.r;
            float _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_G_5 = _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0.g;
            float _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_B_6 = _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0.b;
            float _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_A_7 = _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0.a;
            float4 _Add_bea2440586e04e69a619f8ec033958a1_Out_2;
            Unity_Add_float4(_SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0, _SampleTexture2D_f5a1b9a96cca48a8938480d5b33ed67b_RGBA_0, _Add_bea2440586e04e69a619f8ec033958a1_Out_2);
            float4 _Saturate_66f3c6045b434017be2a4e084a1b4e43_Out_1;
            Unity_Saturate_float4(_Add_bea2440586e04e69a619f8ec033958a1_Out_2, _Saturate_66f3c6045b434017be2a4e084a1b4e43_Out_1);
            UnityTexture2D _Property_74736db2890a4853a5fc463c414d9ee3_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0 = SAMPLE_TEXTURE2D(_Property_74736db2890a4853a5fc463c414d9ee3_Out_0.tex, _Property_74736db2890a4853a5fc463c414d9ee3_Out_0.samplerstate, _Property_74736db2890a4853a5fc463c414d9ee3_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_R_4 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.r;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_G_5 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.g;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_B_6 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.b;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_A_7 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.a;
            float4 _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2;
            Unity_Multiply_float4_float4(_Saturate_66f3c6045b434017be2a4e084a1b4e43_Out_1, (_SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_R_4.xxxx), _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2);
            float2 _Vector2_4e8304f04c5c475ab47e8babb0b06355_Out_0 = float2((_Multiply_b993caffb2ed4da7832e930a7518912d_Out_2).x, 1);
            float4 _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e0da15f5ff82449eb764055230e5e25a_Out_0.tex, _Property_e0da15f5ff82449eb764055230e5e25a_Out_0.samplerstate, _Property_e0da15f5ff82449eb764055230e5e25a_Out_0.GetTransformedUV(_Vector2_4e8304f04c5c475ab47e8babb0b06355_Out_0));
            float _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_R_4 = _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0.r;
            float _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_G_5 = _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0.g;
            float _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_B_6 = _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0.b;
            float _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_A_7 = _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0.a;
            float4 _Multiply_8b0d5da18e644c9fa63a19c4993f140f_Out_2;
            Unity_Multiply_float4_float4((_Property_35b85191b8df4fa39c6b3e5c7a3b703a_Out_0.xxxx), _SampleTexture2D_16fc41517e4d495da226e155c3c482e2_RGBA_0, _Multiply_8b0d5da18e644c9fa63a19c4993f140f_Out_2);
            float4 _Saturate_0bc3071447f440efb03738fb80a34ea5_Out_1;
            Unity_Saturate_float4(_Multiply_8b0d5da18e644c9fa63a19c4993f140f_Out_2, _Saturate_0bc3071447f440efb03738fb80a34ea5_Out_1);
            float _Split_51ca7993ea444f7c9e042ca570d2e88e_R_1 = _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2[0];
            float _Split_51ca7993ea444f7c9e042ca570d2e88e_G_2 = _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2[1];
            float _Split_51ca7993ea444f7c9e042ca570d2e88e_B_3 = _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2[2];
            float _Split_51ca7993ea444f7c9e042ca570d2e88e_A_4 = _Multiply_b993caffb2ed4da7832e930a7518912d_Out_2[3];
            surface.BaseColor = (_Saturate_0bc3071447f440efb03738fb80a34ea5_Out_1.xyz);
            surface.Alpha = _Split_51ca7993ea444f7c9e042ca570d2e88e_R_1;
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