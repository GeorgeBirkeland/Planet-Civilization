Shader "Triplanar"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MainTex1("Base (RGB)", 2D) = "white" {}
		_MainTex2("Base (RGB)", 2D) = "white" {}
		_SizeX("SizeX", Float) = 1
		_SizeY("SizeY", Float) = 1
		_NX("NX", Range(0,1)) = 1
		_NY("NY", Range(0,1)) = 1
		_NZ("NZ", Range(0,1)) = 1
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		sampler2D _MainTex1;
		sampler2D _MainTex2;
		fixed4 _Color;
		fixed _NX;
		fixed _NY;
		fixed _NZ;
		half _SizeX;
		half _SizeY;

		struct Input
		{
			float3 localPos;
			fixed3 worldNormal;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			half2 scale = half2(_SizeX, _SizeY);

			fixed4 c = tex2D(_MainTex, IN.localPos.xz / scale);
			fixed4 c1 = tex2D(_MainTex1, IN.localPos.xy / scale);
			fixed4 c2 = tex2D(_MainTex2, IN.localPos.zy / scale);

			fixed3 nWNormal = normalize(IN.worldNormal*fixed3(_NX, _NY, _NZ));
			fixed3 projnormal = saturate(pow(nWNormal*1.5, 4));

			half4 result = lerp(c, c1, projnormal.z);
			result = lerp(result, c2, projnormal.x);

			o.Albedo = result.rgb * _Color.rgb;
			o.Alpha = result.a * _Color.a;
		}
		ENDCG
	}
	
	Fallback "VertexLit"
}