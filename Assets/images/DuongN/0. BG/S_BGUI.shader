Shader "Unlit/S_BGUI"
{
    Properties
    {
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        _TilingX("TilingX", Range(0.3, 20)) = 1
        _TilingY("TilingY", Range(0.3, 20)) = 1
        _OffsetX("OffsetX", Range(-1, 1)) = 0
        _OffsetY("OffsetY", Range(-1, 1)) = 0
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
        float _TilingY;
        float _TilingX;
        float _OffsetY;
        float _OffsetX;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
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
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_4a201ea288464c87a8b6abe7d8bac979_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_ea362a3d294545e6a5f5b17ec528757e_Out_0 = IN.uv0;
            float _Property_0becef68f40745fa960d3ff70b59a7aa_Out_0 = _OffsetX;
            float _Property_c4a92aea791b4839a7e3275846943600_Out_0 = _OffsetY;
            float2 _Vector2_77e9a3b383bf4793bf249b69166df840_Out_0 = float2(_Property_0becef68f40745fa960d3ff70b59a7aa_Out_0, _Property_c4a92aea791b4839a7e3275846943600_Out_0);
            float _Property_73b9f48c3ab6457f98312906ede5f091_Out_0 = _TilingX;
            float _Property_57dc2d0a72c1424880a7c40f06f8d8f7_Out_0 = _TilingY;
            float2 _Vector2_a7add0d65c4b4df4b0e095adb0f2f9a5_Out_0 = float2(_Property_73b9f48c3ab6457f98312906ede5f091_Out_0, _Property_57dc2d0a72c1424880a7c40f06f8d8f7_Out_0);
            Bindings_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float _SSubTransformation_e780fa179b784399bbe82ceed8d69fc4;
            float2 _SSubTransformation_e780fa179b784399bbe82ceed8d69fc4_UVs_1;
            SG_SSubTransformation_f8f43476f34a9a84e9c0f77f0d0af237_float((_UV_ea362a3d294545e6a5f5b17ec528757e_Out_0.xy), _Vector2_77e9a3b383bf4793bf249b69166df840_Out_0, 0, _Vector2_a7add0d65c4b4df4b0e095adb0f2f9a5_Out_0, float2 (0.5, 0.5), _SSubTransformation_e780fa179b784399bbe82ceed8d69fc4, _SSubTransformation_e780fa179b784399bbe82ceed8d69fc4_UVs_1);
            float4 _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0 = SAMPLE_TEXTURE2D(_Property_4a201ea288464c87a8b6abe7d8bac979_Out_0.tex, _Property_4a201ea288464c87a8b6abe7d8bac979_Out_0.samplerstate, _Property_4a201ea288464c87a8b6abe7d8bac979_Out_0.GetTransformedUV(_SSubTransformation_e780fa179b784399bbe82ceed8d69fc4_UVs_1));
            float _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_R_4 = _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0.r;
            float _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_G_5 = _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0.g;
            float _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_B_6 = _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0.b;
            float _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_A_7 = _SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0.a;
            surface.BaseColor = (_SampleTexture2D_1148cf232a7d47ed94264ec5b3790bfc_RGBA_0.xyz);
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