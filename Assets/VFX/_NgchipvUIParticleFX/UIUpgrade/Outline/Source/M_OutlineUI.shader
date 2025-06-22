Shader "OutlineUI"
{
    Properties
    {
        [NoScaleOffset] Texture2D_4f18a69d155a4af681796c90a10d55b7("MainTex", 2D) = "white" {}
        [HDR]Color_30238d21da724e2ead11740464b97fe8("Color", Color) = (0, 0.5768681, 1, 0)
        Vector1_797795000e104da6bc4cd602e1b24985("Speed", Float) = 2
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1("Texture2D", 2D) = "white" {}
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

// Graph Functions

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

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
    float4 _Property_352e1145ed3d42fea6440d06040976e2_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_30238d21da724e2ead11740464b97fe8) : Color_30238d21da724e2ead11740464b97fe8;
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float4 _Multiply_e4bde28168d84280a5f45ac35d913864_Out_2;
    Unity_Multiply_float(_Property_352e1145ed3d42fea6440d06040976e2_Out_0, _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0, _Multiply_e4bde28168d84280a5f45ac35d913864_Out_2);
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.BaseColor = (_Multiply_e4bde28168d84280a5f45ac35d913864_Out_2.xyz);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

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
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

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
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

// Graph Functions

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

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
    float4 _Property_352e1145ed3d42fea6440d06040976e2_Out_0 = IsGammaSpace() ? LinearToSRGB(Color_30238d21da724e2ead11740464b97fe8) : Color_30238d21da724e2ead11740464b97fe8;
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float4 _Multiply_e4bde28168d84280a5f45ac35d913864_Out_2;
    Unity_Multiply_float(_Property_352e1145ed3d42fea6440d06040976e2_Out_0, _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0, _Multiply_e4bde28168d84280a5f45ac35d913864_Out_2);
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.BaseColor = (_Multiply_e4bde28168d84280a5f45ac35d913864_Out_2.xyz);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

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
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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
float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1_TexelSize;
float4 Texture2D_4f18a69d155a4af681796c90a10d55b7_TexelSize;
float4 Color_30238d21da724e2ead11740464b97fe8;
float Vector1_797795000e104da6bc4cd602e1b24985;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
TEXTURE2D(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
SAMPLER(sampler_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1);
TEXTURE2D(Texture2D_4f18a69d155a4af681796c90a10d55b7);
SAMPLER(samplerTexture2D_4f18a69d155a4af681796c90a10d55b7);

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
    UnityTexture2D _Property_091f2bb6edb1402e92f16529834d9da1_Out_0 = UnityBuildTexture2DStructNoScale(Texture2D_4f18a69d155a4af681796c90a10d55b7);
    float4 _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_091f2bb6edb1402e92f16529834d9da1_Out_0.tex, _Property_091f2bb6edb1402e92f16529834d9da1_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_R_4 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.r;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_G_5 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.g;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_B_6 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.b;
    float _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7 = _SampleTexture2D_34e572e507f541ba9315f1114258a5f3_RGBA_0.a;
    float _Property_94fa7af642b1489898213735f8c277df_Out_0 = Vector1_797795000e104da6bc4cd602e1b24985;
    float _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2;
    Unity_Multiply_float(IN.TimeParameters.x, _Property_94fa7af642b1489898213735f8c277df_Out_0, _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2);
    float2 _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3;
    Unity_Rotate_Radians_float(IN.uv0.xy, float2 (0.5, 0.5), _Multiply_1271b49aa5184f5aa56489a82a030b94_Out_2, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float4 _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_Texture_1).samplerstate, _Rotate_0552b655e490488daeea6d8bc8e1bf42_Out_3);
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_R_4 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.r;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_G_5 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.g;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_B_6 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.b;
    float _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7 = _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_RGBA_0.a;
    float _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
    Unity_Multiply_float(_SampleTexture2D_34e572e507f541ba9315f1114258a5f3_A_7, _SampleTexture2D_1245692d6bef4faaa2f13e40c89811eb_A_7, _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2);
    surface.Alpha = _Multiply_6694c71dc32e43458c4aef42e0752e4c_Out_2;
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