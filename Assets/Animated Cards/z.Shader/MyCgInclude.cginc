#ifndef MY_CG_INCLUDE
#define MY_CG_INCLUDE

//Distortion function.
    float2 distortionScrollFunc(float x, float y)
    {
        float2 xyDistortion = float2(
            frac(_Time.x * x),
            frac(_Time.x * y)
        );

        return xyDistortion;
    }

// //Smoothstep function for Ripple, with duration, size, center, magnitude, mask.
//     // float2 rippleFunc(float rippleDuration, float rippleSize, float2 len, float2 center, float magnitude, float mask)
//     float2 rippleFunc(float rippleDuration, float rippleSize, float2 len, float2 center, float magnitude)

//     {
//         float lowerEdge = frac(_Time.y / rippleDuration ) - rippleSize;
//         float upperEdge = frac(_Time.y / rippleDuration ) + rippleSize;
//         float2 smoothstepVector = smoothstep(lowerEdge, upperEdge, length(len - center));
//         return (1 - smoothstepVector) * smoothstepVector * (normalize(len - center) * magnitude);
//     }

//Oscillating and rotation matrix calculation from oscillating bool, frequency, magnitude, rotation.
    float2x2 rotationMatrixFunc(bool osBool , float osFrequency , float osMagnitude, float rotation)
    {
        float oscillate = osBool * sin(frac(_Time) * 6.28 * ( 20 / osFrequency)) * osMagnitude * 0.0002 + ( 1 - osBool ) * (_Time.x * 3.0);
        float sinTheta = sin (oscillate * rotation);
        float cosTheta = cos (oscillate * rotation);
        float2x2 rotationMatrixCalc = float2x2(cosTheta, -sinTheta, sinTheta, cosTheta);
        return rotationMatrixCalc;
    }

//Move back and forth when speed is != 0, with frequency and magnitude.
    float backAndForthOrMoveFunc(bool backBool, float freq, float mag)
    {
        float bnfOrM = backBool * (sin(frac(_Time.x)  * 6.28 * round(20 / freq))) *  (mag * 0.01) + (1 - backBool) *  _Time.x;
        return bnfOrM;
    }

//Blink function, with color intensity, blink Bool, speed, blink duration, total appear duration, offset Bool, offset Duration.
    float blinkFunc(float intensity, bool blinkBool, float spd, float blinkDur, float appearDur , bool offsetBool, float offsetDur)
    {
        float blink = intensity * (blinkBool * saturate(sin(frac(_Time.x * spd) * 6.28 * round(20 / blinkDur)) + appearDur) * ( 1 - offsetBool + offsetBool * saturate((fmod(_Time.y , blinkDur) - offsetDur) * 10)) + ( 1 - blinkBool));
        // * 10 for blinks to have enough intensity. Otherwise it's quite faint when the fmod returns low).
        return blink;
    }

// //Breathe function, with horizontal and vertical breathing, frequency, magnitude, motion in U and V direction, and blue zone for masking.
//     float2 breatheFunc(bool brHor, float horFreq, float horMag, float motionU, float zone, bool brVer, float verFreq, float verMag, float motionV)
//     {
//         float2 breath = float2(brHor * sin(frac(_Time.y * horFreq) * 6.283) * horMag * motionU,
//                                brVer * sin(frac(_Time.y * verFreq) * 6.283) * verMag * motionV)  * zone;
//         return breath;
//     }

//Breathe short cut function, with horizontal and vertical breathing, frequency, magnitude.
    float2 breatheFuncShort(bool brHor, float horFreq, float horMag, bool brVer, float verFreq, float verMag)
    {
        float2 breath = float2(brHor * sin(frac(_Time.y * horFreq) * 6.283) * horMag,
                               brVer * sin(frac(_Time.y * verFreq) * 6.283) * verMag);
        return breath;
    }

//Single distortion function, with single distort bool, mask, intensity, distortion direction.
    float2 singleDistortFunc(bool distBool, float mask, float intensity, float2 distortDir)
    {
        float2 singleDist = (distBool * mask * intensity) * distortDir;
        return singleDist;
    }

//Separate effect function for fragment shader, with cutpart bool, current color of fragment, current effect color, current effect alpha, zonemask channel, additive bool.
    float4 separateEffFunc(bool cutBool, float4 currentCol, float4 currentEff, float zoneMaskChannel, bool addBool)
    {
        float alphaXZoneMask = currentEff.a * zoneMaskChannel;
        float4 currentEffWAlpha = currentEff * alphaXZoneMask;
        float degreeOfTransparency = smoothstep(0, 1 , alphaXZoneMask);
        float4 separateEff = cutBool * lerp(currentCol, currentEffWAlpha, degreeOfTransparency) 
                            + ( 1 - cutBool) * ((addBool * (currentCol + currentEffWAlpha) 
                                              + (1 - addBool) * (lerp(currentCol, currentCol - currentEffWAlpha, degreeOfTransparency))));
        return separateEff;
        //CutBool: //Alpha blending. Lerping MainTex color with the color of Effect1, with its transparency, multiply by RED CHANNEL of 
        //zoneMask (Main Distortion Mask).
        //Amount of lerp is between edges 0 and 1, with amount of source (Main Tex) appearing = product of ZoneMask RedChannel and effect1's transparency.
        //AddBool: Additive blending.
        //Last lerp: Multiply blending.
    }

//Grayscale
    float4 toGray(float4 color, bool tired)
    {
        float4 grayFactor = float4(0.299f , 0.587f , 0.114f , 0);
        float4 grayColor = tired * mul(color, grayFactor) + (1 - tired) * color;
        return grayColor;
    }
#endif