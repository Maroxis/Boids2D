Shader "Custom/Unlit/BoidUnlitShader"
{
    Properties
    {
		_TintColor("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_Amplitude("Amplitude", float) = 1
		_Speed("Flap Speed", float) = 1
		_Noise("Noise",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Speed;
			float _Amplitude;
			float _Noise;

			v2f vert(appdata v)
			{
				v2f o;
				if (v.vertex.x > 0.005)
				{
					if (v.vertex.y > 0.005)
						v.vertex.y -= sin((_Time.y + _Noise) * _Speed) * _Amplitude;
					else
						v.vertex.y += sin((_Time.y + _Noise) * _Speed) * _Amplitude;
				}
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
                return col;
            }
            ENDCG
        }
    }
}
