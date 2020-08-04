Shader "Custom/BoidShader" {
		Properties{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
			_Metallic("Metallic", Range(0,1)) = 0.0
		}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model
		#pragma surface surf Standard vertex:vert addshadow fullforwardshadows nolightmap
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		float angle;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			struct Boid
			{
				float3 position;
				float3 direction;
			};
			StructuredBuffer<Boid> boidBuffer;
		#endif

		float3 rotate(float3 vertex, float degrees)
		{
			float deg = degrees + UNITY_PI;
			float4 vert = float4(vertex, 0);
			float4x4 m;

			m[0] = float4(cos(deg), -sin(deg), 0, 0);
			m[1] = float4(sin(deg), cos(deg), 0, 0);
			m[2] = float4(0, 0, 1, 0);
			m[3] = float4(0, 0, 0, 1);
			return mul(m, vert).xyz;
		}

		void setup()
		{
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			float3 data = boidBuffer[unity_InstanceID].position;
			float size = 200;
			angle = atan2(boidBuffer[unity_InstanceID].direction.y, boidBuffer[unity_InstanceID].direction.x);
			unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
			unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
			unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
			unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
			unity_WorldToObject = unity_ObjectToWorld;
			unity_WorldToObject._14_24_34 *= -1;
			unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
		#endif
		}

		void vert(inout appdata_full v)
		{
			v.vertex.xyz = rotate(v.vertex.xyz, angle);
		}
		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
		}
		FallBack "Diffuse"
}