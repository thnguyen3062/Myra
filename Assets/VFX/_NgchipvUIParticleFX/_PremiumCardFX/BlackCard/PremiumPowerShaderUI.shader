Shader "PremiumPowerShader"
{
    Properties
    {
        [HDR] Color_3a77e5b24bf345409e3fa5c6da2b2eb7("Color1", Color) = (1, 0.703978, 0, 0)
        [HDR]Color_22cb48c6f6e34f32a235ac8cd7ad501d("Color2", Color) = (1, 0, 0.5928421, 0)
        [NoScaleOffset]Texture2D_0bfc715abbaf486ab9a057885da41035("T_Noise", 2D) = "white" {}
        [NoScaleOffset]Texture2D_6f8406899a754e3b9b4c2dad6adc7b27("T_Noise2", 2D) = "white" {}
        [NoScaleOffset]Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d("T_Mask", 2D) = "white" {}
        Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe("Speed", Float) = 0.54
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
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
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
    float4 _Property_48fc5c25d10b4124ab6633c16f7cfdc1_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_22cb48c6f6e34f32a235ac8cd7ad501d) : Color_22cb48c6f6e34f32a235ac8cd7ad501d;
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Float_8d1d67b726e3443bbac029807ad9f73e_Out_0 = 0.31;
    float4 _Lerp_43d8f45868d24c5b8a3713b783039755_Out_3;
    Unity_Lerp_float4(_SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0, float4(1, 1, 1, 1), (_Float_8d1d67b726e3443bbac029807ad9f73e_Out_0.xxxx), _Lerp_43d8f45868d24c5b8a3713b783039755_Out_3);
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float4 _Multiply_77d2e8666f0046808989998ada16af55_Out_2;
    Unity_Multiply_float(_Lerp_43d8f45868d24c5b8a3713b783039755_Out_3, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0, _Multiply_77d2e8666f0046808989998ada16af55_Out_2);
    float4 _Multiply_64e9163b0f5e400196113c25d150486c_Out_2;
    Unity_Multiply_float(_Property_48fc5c25d10b4124ab6633c16f7cfdc1_Out_0, _Multiply_77d2e8666f0046808989998ada16af55_Out_2, _Multiply_64e9163b0f5e400196113c25d150486c_Out_2);
    float4 _Property_7a49d51b2228453ab4453c95f8b697ae_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_3a77e5b24bf345409e3fa5c6da2b2eb7) : Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    float _Float_3c9ddd3d85ef4220af3e126ed5db3d8c_Out_0 = 0.48;
    float4 _Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3;
    Unity_Lerp_float4(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0, float4(1, 1, 1, 1), (_Float_3c9ddd3d85ef4220af3e126ed5db3d8c_Out_0.xxxx), _Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3);
    float4 _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2;
    Unity_Multiply_float(_Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0, _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2);
    float4 _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2;
    Unity_Multiply_float(_Property_7a49d51b2228453ab4453c95f8b697ae_Out_0, _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2, _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2);
    float4 _Add_9158d274bb394d8eaaf68405d1838517_Out_2;
    Unity_Add_float4(_Multiply_64e9163b0f5e400196113c25d150486c_Out_2, _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2, _Add_9158d274bb394d8eaaf68405d1838517_Out_2);
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.BaseColor = (_Add_9158d274bb394d8eaaf68405d1838517_Out_2.xyz);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
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
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
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
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
{
    Out = lerp(A, B, T);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Add_float4(float4 A, float4 B, out float4 Out)
{
    Out = A + B;
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
    float4 _Property_48fc5c25d10b4124ab6633c16f7cfdc1_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_22cb48c6f6e34f32a235ac8cd7ad501d) : Color_22cb48c6f6e34f32a235ac8cd7ad501d;
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Float_8d1d67b726e3443bbac029807ad9f73e_Out_0 = 0.31;
    float4 _Lerp_43d8f45868d24c5b8a3713b783039755_Out_3;
    Unity_Lerp_float4(_SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0, float4(1, 1, 1, 1), (_Float_8d1d67b726e3443bbac029807ad9f73e_Out_0.xxxx), _Lerp_43d8f45868d24c5b8a3713b783039755_Out_3);
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float4 _Multiply_77d2e8666f0046808989998ada16af55_Out_2;
    Unity_Multiply_float(_Lerp_43d8f45868d24c5b8a3713b783039755_Out_3, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0, _Multiply_77d2e8666f0046808989998ada16af55_Out_2);
    float4 _Multiply_64e9163b0f5e400196113c25d150486c_Out_2;
    Unity_Multiply_float(_Property_48fc5c25d10b4124ab6633c16f7cfdc1_Out_0, _Multiply_77d2e8666f0046808989998ada16af55_Out_2, _Multiply_64e9163b0f5e400196113c25d150486c_Out_2);
    float4 _Property_7a49d51b2228453ab4453c95f8b697ae_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_3a77e5b24bf345409e3fa5c6da2b2eb7) : Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    float _Float_3c9ddd3d85ef4220af3e126ed5db3d8c_Out_0 = 0.48;
    float4 _Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3;
    Unity_Lerp_float4(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0, float4(1, 1, 1, 1), (_Float_3c9ddd3d85ef4220af3e126ed5db3d8c_Out_0.xxxx), _Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3);
    float4 _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2;
    Unity_Multiply_float(_Lerp_5be0f6ebcedc40568bb71e02ba308c3a_Out_3, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0, _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2);
    float4 _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2;
    Unity_Multiply_float(_Property_7a49d51b2228453ab4453c95f8b697ae_Out_0, _Multiply_056acb31a4c14635aa75dbdf711da00c_Out_2, _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2);
    float4 _Add_9158d274bb394d8eaaf68405d1838517_Out_2;
    Unity_Add_float4(_Multiply_64e9163b0f5e400196113c25d150486c_Out_2, _Multiply_db1ec5b4e7424a1582d07a4dd060359c_Out_2, _Add_9158d274bb394d8eaaf68405d1838517_Out_2);
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.BaseColor = (_Add_9158d274bb394d8eaaf68405d1838517_Out_2.xyz);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
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
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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
float4 Color_3a77e5b24bf345409e3fa5c6da2b2eb7;
float4 Color_22cb48c6f6e34f32a235ac8cd7ad501d;
float4 Texture2D_0bfc715abbaf486ab9a057885da41035_TexelSize;
float4 Texture2D_6f8406899a754e3b9b4c2dad6adc7b27_TexelSize;
float4 Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d_TexelSize;
float Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(Texture2D_0bfc715abbaf486ab9a057885da41035);
SAMPLER(samplerTexture2D_0bfc715abbaf486ab9a057885da41035);
TEXTURE2D(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
SAMPLER(samplerTexture2D_6f8406899a754e3b9b4c2dad6adc7b27);
TEXTURE2D(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
SAMPLER(samplerTexture2D_e8cc20a2855f42e1b851a2d926ed5f3d);

// Graph Functions

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
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
    UnityTexture2D _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_0bfc715abbaf486ab9a057885da41035);
    float _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0 = Vector1_1aa1ffffb9c24dcc8a6008537d3f62fe;
    float _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_e315f46b77244c328b2ca4a2a2108dfe_Out_0, _Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2);
    float _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2;
    Unity_Multiply_float(_Multiply_3d4a6f598ee045f3b57acf192623cc88_Out_2, -1, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _Vector2_366239603fbb4a95b04e1389182fe938_Out_0 = float2(0, _Multiply_70d135579be4450dbca9f5365b4bb23e_Out_2);
    float2 _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_366239603fbb4a95b04e1389182fe938_Out_0, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float4 _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0 = SAMPLE_TEXTURE2D(_Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.tex, _Property_aaa4c2e9e4a54f28b0e18348b2fee940_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_R_4 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.r;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_G_5 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.g;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_B_6 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.b;
    float _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7 = _SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_RGBA_0.a;
    UnityTexture2D _Property_88665909c3dc41a6a934c0e07b27e833_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d);
    float4 _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0 = SAMPLE_TEXTURE2D(_Property_88665909c3dc41a6a934c0e07b27e833_Out_0.tex, _Property_88665909c3dc41a6a934c0e07b27e833_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_R_4 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.r;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_G_5 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.g;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_B_6 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.b;
    float _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7 = _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_RGBA_0.a;
    float _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2;
    Unity_Multiply_float(_SampleTexture2D_2a68e99e911b456c981565cfed1a1de5_A_7, _SampleTexture2D_e5cf7df64c3f41d79ea16a601c78ab68_A_7, _Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2);
    UnityTexture2D _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_6f8406899a754e3b9b4c2dad6adc7b27);
    float4 _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0 = SAMPLE_TEXTURE2D(_Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.tex, _Property_ca5527f438e64186a1fa0a454ad32f9b_Out_0.samplerstate, _TilingAndOffset_28df8939003a4ef0954ac0049f9d5680_Out_3);
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_R_4 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.r;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_G_5 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.g;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_B_6 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.b;
    float _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7 = _SampleTexture2D_4d9d528906954b61b027abf01f18c247_RGBA_0.a;
    float _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
    Unity_Multiply_float(_Multiply_7bfc388fbfe840628b47da44ec68e2df_Out_2, _SampleTexture2D_4d9d528906954b61b027abf01f18c247_A_7, _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2);
    surface.Alpha = _Multiply_2f3d4794b48d44a39fe60c46d978bea9_Out_2;
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