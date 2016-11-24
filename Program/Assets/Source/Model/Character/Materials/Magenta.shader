Shader "Custom/Magenta" {
	Properties{
		_Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Offset0("Offset 0", vector) = (0, 0, 0, 0)
		_Alpha0("Alpha 0", Range(0.0, 1.0)) = 0.078125
		_Offset1("Offset 1", vector) = (0, 0, 0, 0)
		_Alpha1("Alpha 0", Range(0.0, 1.0)) = 0.078125
		_Offset2("Offset 2", vector) = (0, 0, 0, 0)
		_Alpha2("Alpha 0", Range(0.0, 1.0)) = 0.078125
		_Offset3("Offset 3", vector) = (0, 0, 0, 0)
		_Alpha3("Alpha 0", Range(0.0, 1.0)) = 0.078125
	}

		CGINCLUDE
#include "UnityCG.cginc"

	float4 _Color;
	float4 _Offset0;
	float4 _Offset1;
	float4 _Offset2;
	float4 _Offset3;
	half _Alpha0;
	half _Alpha1;
	half _Alpha2;
	half _Alpha3;

	struct appdata {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 color : COLOR;
	};

	// 在shader中要渲染自身，以及4个残影，所以要定义5个不同的vert函数
	v2f vert_normal(appdata v) { // 渲染自身的vert函数
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		return o;
	}
	// 渲染4个残影的vert函数
	v2f vert_offset_1(appdata v) {
		v2f o;
		float4 pos = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_VP, pos + _Offset0);
		o.color = v.color;
		return o;
	}


	v2f vert_offset_2(appdata v) {
		v2f o;
		float4 pos = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_VP, pos + _Offset1);
		o.color = v.color;
		return o;
	}

	v2f vert_offset_3(appdata v) {
		v2f o;
		float4 pos = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_VP, pos + _Offset2);
		o.color = v.color;
		return o;
	}

	v2f vert_offset_4(appdata v) {
		v2f o;
		float4 pos = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_VP, pos + _Offset3);
		o.color = v.color;
		return o;
	}

	float4 frag_normal(v2f i) : SV_Target{
		return i.color;
	}

	float4 frag_color1(v2f i) : COLOR{
		float4 c;
		c.rgb = _Color;
		c.w = _Alpha0;
		return c;
	}

	float4 frag_color2(v2f i) : COLOR{
		float4 c;
		c.rgb = _Color;
		c.w = _Alpha1;
		return c;
	}

	float4 frag_color3(v2f i) : COLOR{
		float4 c;
		c.rgb = _Color;
		c.w = _Alpha2;
		return c;
	}

	float4 frag_color4(v2f i) : COLOR{
		float4 c;
		c.rgb = _Color;
		c.w = _Alpha3;
		return c;
	}
		ENDCG

		SubShader { // 这里用4个pass来渲染残影，第5个pass渲染自身
		Pass{ // 从最远的开始渲染
			
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
#pragma vertex vert_offset_4
#pragma fragment frag_color4
			ENDCG
		}

			Pass{
	
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
#pragma vertex vert_offset_3
#pragma fragment frag_color3
			ENDCG
		}

			Pass{

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
#pragma vertex vert_offset_2
#pragma fragment frag_color2
			ENDCG
		}

			Pass{
			
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
#pragma vertex vert_offset_1
#pragma fragment frag_color1
			ENDCG
		}

			Pass{ // 渲染自身，这时要开启 ZWrite
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
#pragma vertex vert_normal
#pragma fragment frag_normal
			ENDCG
		}
	}
	FallBack "Diffuse"
}