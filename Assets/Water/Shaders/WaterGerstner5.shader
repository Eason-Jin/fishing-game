Shader "BeatWeights/RealWater_Gerstner5"
{
    Properties
    {
        _DeepColor     ("Deep Color", Color) = (0.12, 0.28, 0.46, 1)
        _ShallowColor  ("Shallow Color", Color) = (0.40, 0.70, 0.95, 1)
        _Smoothness    ("Smoothness", Range(0,1)) = 0.92
        _FresnelPower  ("Fresnel Power", Range(0.5, 8)) = 4.5
        _Steepness     ("Wave Steepness (0-1)", Range(0,1)) = 0.65

        // Five gerstner waves (amplitude, wavelength, speed, direction degrees)
        _W1Amp("W1 Amp", Range(0,1)) = 0.05
        _W1Len("W1 Len", Range(0.1,20)) = 3.0
        _W1Spd("W1 Speed", Range(-5,5)) = 0.9
        _W1Dir("W1 Dir (deg)", Range(-180,180)) = 10

        _W2Amp("W2 Amp", Range(0,1)) = 0.035
        _W2Len("W2 Len", Range(0.1,20)) = 2.0
        _W2Spd("W2 Speed", Range(-5,5)) = 1.2
        _W2Dir("W2 Dir (deg)", Range(-180,180)) = -25

        _W3Amp("W3 Amp", Range(0,1)) = 0.028
        _W3Len("W3 Len", Range(0.1,20)) = 1.4
        _W3Spd("W3 Speed", Range(-5,5)) = 1.6
        _W3Dir("W3 Dir (deg)", Range(-180,180)) = 55

        _W4Amp("W4 Amp", Range(0,1)) = 0.018
        _W4Len("W4 Len", Range(0.1,20)) = 1.0
        _W4Spd("W4 Speed", Range(-5,5)) = 2.0
        _W4Dir("W4 Dir (deg)", Range(-180,180)) = -60

        _W5Amp("W5 Amp", Range(0,1)) = 0.012
        _W5Len("W5 Len", Range(0.1,20)) = 0.7
        _W5Spd("W5 Speed", Range(-5,5)) = 2.3
        _W5Dir("W5 Dir (deg)", Range(-180,180)) = 90
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
            float3 viewDir;
        };

        fixed4 _DeepColor, _ShallowColor;
        half _Smoothness, _FresnelPower, _Steepness;

        half _W1Amp,_W1Len,_W1Spd,_W1Dir;
        half _W2Amp,_W2Len,_W2Spd,_W2Dir;
        half _W3Amp,_W3Len,_W3Spd,_W3Dir;
        half _W4Amp,_W4Len,_W4Spd,_W4Dir;
        half _W5Amp,_W5Len,_W5Spd,_W5Dir;

        float2 Dir(float deg){ float r = radians(deg); return float2(cos(r), sin(r)); }

        void Gerstner(float3 posOS, half amp, half len, half spd, float2 dir, out float3 disp, out float2 grad)
        {
            disp = 0; grad = 0;
            if (amp <= 0 || len <= 0) return;

            float k = 2.0 * UNITY_PI / len;                  // wave number
            float phase = k * (dir.x * posOS.x + dir.y * posOS.z) + _Time.y * spd;
            float s = sin(phase), c = cos(phase);
            float qa = _Steepness * amp;

            // horizontal displacement (scaled by 1/k for sensible units)
            disp.x = qa * dir.x * c / k;
            disp.z = qa * dir.y * c / k;
            disp.y = amp * s;

            // height derivatives for normal
            grad.x = amp * k * dir.x * c; // dH/dx
            grad.y = amp * k * dir.y * c; // dH/dz
        }

        void vert (inout appdata_full v)
        {
            float3 pos = v.vertex.xyz;

            float3 d; float2 g;
            float3 totalDisp = 0;
            float2 totalGrad = 0;

            Gerstner(pos, _W1Amp,_W1Len,_W1Spd, Dir(_W1Dir), d, g); totalDisp += d; totalGrad += g;
            Gerstner(pos, _W2Amp,_W2Len,_W2Spd, Dir(_W2Dir), d, g); totalDisp += d; totalGrad += g;
            Gerstner(pos, _W3Amp,_W3Len,_W3Spd, Dir(_W3Dir), d, g); totalDisp += d; totalGrad += g;
            Gerstner(pos, _W4Amp,_W4Len,_W4Spd, Dir(_W4Dir), d, g); totalDisp += d; totalGrad += g;
            Gerstner(pos, _W5Amp,_W5Len,_W5Spd, Dir(_W5Dir), d, g); totalDisp += d; totalGrad += g;

            pos += totalDisp;
            v.vertex.xyz = pos;

            // rebuild an object-space normal from height gradient (adds small ripples look)
            float3 nOS = normalize(float3(-totalGrad.x, 1.0, -totalGrad.y));
            v.normal = nOS;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // fresnel tint (brighter glancing angles)
            float3 dpdx = ddx(IN.worldPos);
            float3 dpdy = ddy(IN.worldPos);
            float3 worldN = normalize(cross(dpdy, dpdx));
            float3 V = normalize(IN.viewDir);
            float fres = pow(1.0 - saturate(dot(worldN, V)), _FresnelPower);

            float3 col = lerp(_DeepColor.rgb, _ShallowColor.rgb, fres);
            o.Albedo     = col;
            o.Metallic   = 0.0;
            o.Smoothness = _Smoothness;
            o.Alpha      = 1;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
