Shader "OpenPackFlare_Shader"
{
    Properties
    {
        [HDR] Color_34ade15b74f24a389cd07e25cf345d19("Color", Color) = (0.1745283, 0.6628754, 1, 0)
        [NoScaleOffset]Texture2D_d08433b203604dff92d784361959ced9("T_Flare1", 2D) = "white" {}
        Vector1_d7286bd9b1174e7092ec9e73acf0fc0c("RSpeed1", Float) = 0.5
        [NoScaleOffset]Texture2D_477ae252c04143eca161e1bf8c024e9f("T_Flare2", 2D) = "white" {}
        Vector1_1abd14dcb843415fa16310530e42a3ab("RSpeed2", Float) = 0.5
        [NoScaleOffset]Texture2D_7b85c02fe0224ad9a6c438e4c9a295db("Mask", 2D) = "white" {}
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
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma shader_feature _ _SAMPLE_GI
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Add_float(float A, float B, out float Out)
{
    Out = A + B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float4 _Property_23f2645ac04e4bbfa3b03425f11d7405_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_34ade15b74f24a389cd07e25cf345d19) : Color_34ade15b74f24a389cd07e25cf345d19;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    UnityTexture2D _Property_893a3c2596db4124ae010e92895827f4_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_477ae252c04143eca161e1bf8c024e9f);
    float _Property_3831b4c58e624fc2a42f04b95acd4f66_Out_0 = Vector1_1abd14dcb843415fa16310530e42a3ab;
    float _Multiply_857468dc039f4906af21187f16eede77_Out_2;
    Unity_Multiply_float(-1, _Property_3831b4c58e624fc2a42f04b95acd4f66_Out_0, _Multiply_857468dc039f4906af21187f16eede77_Out_2);
    float _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Multiply_857468dc039f4906af21187f16eede77_Out_2, _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2);
    float2 _Rotate_cfc2160438b842eebaed24fb494adc08_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2, _Rotate_cfc2160438b842eebaed24fb494adc08_Out_3);
    float2 _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3;
    Unity_TilingAndOffset_float(_Rotate_cfc2160438b842eebaed24fb494adc08_Out_3, float2 (1, 1), float2 (0, 0), _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3);
    float4 _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0 = SAMPLE_TEXTURE2D(_Property_893a3c2596db4124ae010e92895827f4_Out_0.tex, _Property_893a3c2596db4124ae010e92895827f4_Out_0.samplerstate, _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3);
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_R_4 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.r;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_G_5 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.g;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_B_6 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.b;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_A_7 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.a;
    float _Add_e284973ed40640fe8043dbc5c9fd818c_Out_2;
    Unity_Add_float(_SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7, _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_A_7, _Add_e284973ed40640fe8043dbc5c9fd818c_Out_2);
    float4 _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2;
    Unity_Add_float4(_SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0, (_Add_e284973ed40640fe8043dbc5c9fd818c_Out_2.xxxx), _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2);
    float4 _Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2;
    Unity_Multiply_float(_Property_23f2645ac04e4bbfa3b03425f11d7405_Out_0, _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2, _Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2);
    surface.BaseColor = (_Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2.xyz);
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
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
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
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
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        }
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
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma multi_compile_fog
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma shader_feature _ _SAMPLE_GI
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Add_float(float A, float B, out float Out)
{
    Out = A + B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float4 _Property_23f2645ac04e4bbfa3b03425f11d7405_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_34ade15b74f24a389cd07e25cf345d19) : Color_34ade15b74f24a389cd07e25cf345d19;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    UnityTexture2D _Property_893a3c2596db4124ae010e92895827f4_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_477ae252c04143eca161e1bf8c024e9f);
    float _Property_3831b4c58e624fc2a42f04b95acd4f66_Out_0 = Vector1_1abd14dcb843415fa16310530e42a3ab;
    float _Multiply_857468dc039f4906af21187f16eede77_Out_2;
    Unity_Multiply_float(-1, _Property_3831b4c58e624fc2a42f04b95acd4f66_Out_0, _Multiply_857468dc039f4906af21187f16eede77_Out_2);
    float _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Multiply_857468dc039f4906af21187f16eede77_Out_2, _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2);
    float2 _Rotate_cfc2160438b842eebaed24fb494adc08_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_3f2cacbab4c140e58f7046040c948975_Out_2, _Rotate_cfc2160438b842eebaed24fb494adc08_Out_3);
    float2 _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3;
    Unity_TilingAndOffset_float(_Rotate_cfc2160438b842eebaed24fb494adc08_Out_3, float2 (1, 1), float2 (0, 0), _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3);
    float4 _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0 = SAMPLE_TEXTURE2D(_Property_893a3c2596db4124ae010e92895827f4_Out_0.tex, _Property_893a3c2596db4124ae010e92895827f4_Out_0.samplerstate, _TilingAndOffset_214dedff04c647289db073cb4690fa6b_Out_3);
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_R_4 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.r;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_G_5 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.g;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_B_6 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.b;
    float _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_A_7 = _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_RGBA_0.a;
    float _Add_e284973ed40640fe8043dbc5c9fd818c_Out_2;
    Unity_Add_float(_SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7, _SampleTexture2D_e899e0bf7e0a4d239b02d202536a32ff_A_7, _Add_e284973ed40640fe8043dbc5c9fd818c_Out_2);
    float4 _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2;
    Unity_Add_float4(_SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0, (_Add_e284973ed40640fe8043dbc5c9fd818c_Out_2.xxxx), _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2);
    float4 _Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2;
    Unity_Multiply_float(_Property_23f2645ac04e4bbfa3b03425f11d7405_Out_0, _Add_cad697baf5d54bb4b86dc2e5b882a87b_Out_2, _Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2);
    surface.BaseColor = (_Multiply_d6c4a26dddc044c4b97c9785b72264cc_Out_2.xyz);
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
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
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
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
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

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
        float4 interp0 : TEXCOORD0;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyzw = input.texCoord0;
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
    Varyings UnpackVaryings(PackedVaryings input)
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
float4 Color_34ade15b74f24a389cd07e25cf345d19;
float4 Texture2D_d08433b203604dff92d784361959ced9_TexelSize;
float Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
float4 Texture2D_477ae252c04143eca161e1bf8c024e9f_TexelSize;
float Vector1_1abd14dcb843415fa16310530e42a3ab;
float4 Texture2D_7b85c02fe0224ad9a6c438e4c9a295db_TexelSize;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_d08433b203604dff92d784361959ced9);
SAMPLER(samplerTexture2D_d08433b203604dff92d784361959ced9);
TEXTURE2D(Texture2D_477ae252c04143eca161e1bf8c024e9f);
SAMPLER(samplerTexture2D_477ae252c04143eca161e1bf8c024e9f);
TEXTURE2D(Texture2D_7b85c02fe0224ad9a6c438e4c9a295db);
SAMPLER(samplerTexture2D_7b85c02fe0224ad9a6c438e4c9a295db);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
{
    //rotation matrix
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Out = UV;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    UnityTexture2D _Property_55193027fedb44d3a3354ea4f032a698_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_d08433b203604dff92d784361959ced9);
    float _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0 = Vector1_d7286bd9b1174e7092ec9e73acf0fc0c;
    float _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_7d334e9855b04717b8d5a57dc9b0f026_Out_0, _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2);
    float2 _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_bd65056c858848fa91c8ba474f4dc257_Out_2, _Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3);
    float2 _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3;
    Unity_TilingAndOffset_float(_Rotate_bbae0b0e34ed4c579d4cf7982c8837ef_Out_3, float2 (1.64, 1.64), float2 (-0.33, -0.33), _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float4 _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0 = SAMPLE_TEXTURE2D(_Property_55193027fedb44d3a3354ea4f032a698_Out_0.tex, _Property_55193027fedb44d3a3354ea4f032a698_Out_0.samplerstate, _TilingAndOffset_accd1bbff28d491885e7ae796d97eefa_Out_3);
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_R_4 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.r;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_G_5 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.g;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_B_6 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.b;
    float _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7 = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_RGBA_0.a;
    surface.Alpha = _SampleTexture2D_2f3a7c8f1d2941d6b16038175ef8c1d0_A_7;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
    }
        FallBack "Hidden/Shader Graph/FallbackError"
}