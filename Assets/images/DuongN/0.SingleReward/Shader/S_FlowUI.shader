Shader "Unlit/S_FlowUI"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        [NoScaleOffset]_GradientTex("GradientTex", 2D) = "white" {}
        [NoScaleOffset]_MaskTex("MaskTex", 2D) = "white" {}
        _Intensity("Intensity", Range(-1, 10)) = 1
        _SpeedX("SpeedX", Range(-0.5, 0.5)) = 0.1
        _SpeedY("SpeedY", Range(-0.5, 0.5)) = 0.1
        _GammaPower("GammaPower", Range(0.01, 0.4545)) = 0.4545
        [HideInInspector]_CastShadows("_CastShadows", Float) = 0
        [HideInInspector]_Surface("_Surface", Float) = 1
        [HideInInspector]_Blend("_Blend", Float) = 2
        [HideInInspector]_AlphaClip("_AlphaClip", Float) = 0
        [HideInInspector]_SrcBlend("_SrcBlend", Float) = 1
        [HideInInspector]_DstBlend("_DstBlend", Float) = 0
        [HideInInspector][ToggleUI]_ZWrite("_ZWrite", Float) = 0
        [HideInInspector]_ZWriteControl("_ZWriteControl", Float) = 0
        [HideInInspector]_ZTest("_ZTest", Float) = 4
        [HideInInspector]_Cull("_Cull", Float) = 2
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
        #pragma shader_feature_fragment _ _SURFACE_TYPE_TRANSPARENT
        #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _ALPHATEST_ON
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
        float _SpeedX;
        float _SpeedY;
        float4 _GradientTex_TexelSize;
        float _GammaPower;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);
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
        
        void Unity_Power_float4(float4 A, float4 B, out float4 Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
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
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_968efc4a55cd4f6fb50f094e4dfee42b_Out_0 = UnityBuildTexture2DStructNoScale(_GradientTex);
            UnityTexture2D _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float _Property_1d8c13618232490dba095df5eafe9a16_Out_0 = _SpeedX;
            float _Property_f7ebac9bb12b4325bce37b5448504adb_Out_0 = _SpeedY;
            float2 _Vector2_f8d927de11874bfdbca9f82c0933da14_Out_0 = float2(_Property_1d8c13618232490dba095df5eafe9a16_Out_0, _Property_f7ebac9bb12b4325bce37b5448504adb_Out_0);
            float2 _Multiply_fc91d687c3624b20b96b6bbf900d792d_Out_2;
            Unity_Multiply_float2_float2(_Vector2_f8d927de11874bfdbca9f82c0933da14_Out_0, (IN.TimeParameters.x.xx), _Multiply_fc91d687c3624b20b96b6bbf900d792d_Out_2);
            float2 _TilingAndOffset_04d21947b55b4f198fbb8c2c8aa32174_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_fc91d687c3624b20b96b6bbf900d792d_Out_2, _TilingAndOffset_04d21947b55b4f198fbb8c2c8aa32174_Out_3);
            float4 _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0 = SAMPLE_TEXTURE2D(_Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.tex, _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.samplerstate, _Property_6a1fbdc3c8a447b0aabce833cc251479_Out_0.GetTransformedUV(_TilingAndOffset_04d21947b55b4f198fbb8c2c8aa32174_Out_3));
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_R_4 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.r;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_G_5 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.g;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_B_6 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.b;
            float _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_A_7 = _SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_RGBA_0.a;
            float _Property_64de4a196f94456daa233d5b8cf467a7_Out_0 = _GammaPower;
            float _Power_5900fdf6395943b3ba84d2b3e517cbf3_Out_2;
            Unity_Power_float(_SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_R_4, _Property_64de4a196f94456daa233d5b8cf467a7_Out_0, _Power_5900fdf6395943b3ba84d2b3e517cbf3_Out_2);
            float2 _Vector2_fbd59257bde84b639eebfadd9f22d588_Out_0 = float2(_Power_5900fdf6395943b3ba84d2b3e517cbf3_Out_2, 0);
            float4 _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0 = SAMPLE_TEXTURE2D(_Property_968efc4a55cd4f6fb50f094e4dfee42b_Out_0.tex, _Property_968efc4a55cd4f6fb50f094e4dfee42b_Out_0.samplerstate, _Property_968efc4a55cd4f6fb50f094e4dfee42b_Out_0.GetTransformedUV(_Vector2_fbd59257bde84b639eebfadd9f22d588_Out_0));
            float _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_R_4 = _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0.r;
            float _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_G_5 = _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0.g;
            float _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_B_6 = _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0.b;
            float _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_A_7 = _SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0.a;
            float _Property_84a019ad53fb4f90be39024059fdaf38_Out_0 = _GammaPower;
            float4 _Power_19715fdb2e8d4787bcc21fb96be93f6e_Out_2;
            Unity_Power_float4(_SampleTexture2D_08a4ebb935734dc889d0306b17ae9d0e_RGBA_0, (_Property_84a019ad53fb4f90be39024059fdaf38_Out_0.xxxx), _Power_19715fdb2e8d4787bcc21fb96be93f6e_Out_2);
            float _Property_ca69eb8a161449849c71c23bb5630a90_Out_0 = _Intensity;
            float4 _Multiply_76056eb129114308b45de5a08d83704c_Out_2;
            Unity_Multiply_float4_float4(_Power_19715fdb2e8d4787bcc21fb96be93f6e_Out_2, (_Property_ca69eb8a161449849c71c23bb5630a90_Out_0.xxxx), _Multiply_76056eb129114308b45de5a08d83704c_Out_2);
            float _Property_549bb04ef0b6484f946870e56d5438d8_Out_0 = _GammaPower;
            float _Power_4f2b78fc341c4d9dac86497b411e4be2_Out_2;
            Unity_Power_float(_SampleTexture2D_1b0d314f4f8f477cbbb9967af68133af_A_7, _Property_549bb04ef0b6484f946870e56d5438d8_Out_0, _Power_4f2b78fc341c4d9dac86497b411e4be2_Out_2);
            UnityTexture2D _Property_74736db2890a4853a5fc463c414d9ee3_Out_0 = UnityBuildTexture2DStructNoScale(_MaskTex);
            float4 _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0 = SAMPLE_TEXTURE2D(_Property_74736db2890a4853a5fc463c414d9ee3_Out_0.tex, _Property_74736db2890a4853a5fc463c414d9ee3_Out_0.samplerstate, _Property_74736db2890a4853a5fc463c414d9ee3_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_R_4 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.r;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_G_5 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.g;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_B_6 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.b;
            float _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_A_7 = _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_RGBA_0.a;
            float _Multiply_3b48bc642bf04c47bb1edd64cb18f6ea_Out_2;
            Unity_Multiply_float_float(_Power_4f2b78fc341c4d9dac86497b411e4be2_Out_2, _SampleTexture2D_a05eb3e51636424a832d917a57a5f9c7_R_4, _Multiply_3b48bc642bf04c47bb1edd64cb18f6ea_Out_2);
            float _Property_e2f35728124941e4a1d5f2df8264332e_Out_0 = _GammaPower;
            float _Power_5cc3be45be3f45708cef2e3ee2ab2058_Out_2;
            Unity_Power_float(_Multiply_3b48bc642bf04c47bb1edd64cb18f6ea_Out_2, _Property_e2f35728124941e4a1d5f2df8264332e_Out_0, _Power_5cc3be45be3f45708cef2e3ee2ab2058_Out_2);
            float _Multiply_dac4870739284e4ca42893f2daaeb4ad_Out_2;
            Unity_Multiply_float_float(_Power_5900fdf6395943b3ba84d2b3e517cbf3_Out_2, _Power_5cc3be45be3f45708cef2e3ee2ab2058_Out_2, _Multiply_dac4870739284e4ca42893f2daaeb4ad_Out_2);
            surface.BaseColor = (_Multiply_76056eb129114308b45de5a08d83704c_Out_2.xyz);
            surface.Alpha = _Multiply_dac4870739284e4ca42893f2daaeb4ad_Out_2;
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