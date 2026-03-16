Shader "Noise/VoronoiUnlit" {
    Properties {
        _CellSize ("Cell Size", Range(0, 2)) = 2
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _TimeScale ("Scrolling Speed", Range(0, 2)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "Random.cginc"

            float _CellSize;
            float _TimeScale;
            float3 _BorderColor;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float3 voronoiNoise(float3 value){
                float3 baseCell = floor(value);

                //first pass to find the closest cell
                float minDistToCell = 10;
                float3 toClosestCell;
                float3 closestCell;

                [unroll]
                for(int x1=-1; x1<=1; x1++){
                        [unroll]
                        for(int y1=-1; y1<=1; y1++){
                            [unroll]
                            for(int z1=-1; z1<=1; z1++){
                                float3 cell = baseCell + float3(x1, y1, z1);
                                float3 cellPosition = cell + rand3dTo3d(cell);
                                float3 toCell = cellPosition - value;
                                float distToCell = length(toCell);
                                if(distToCell < minDistToCell){
                                    minDistToCell = distToCell;
                                    closestCell = cell;
                                    toClosestCell = toCell;
                            }
                        }
                    }
                }

                //second pass to find the distance to the closest edge
                float minEdgeDistance = 10;
                [unroll]
                for(int x2=-1; x2<=1; x2++){
                    [unroll]
                    for(int y2=-1; y2<=1; y2++){
                        [unroll]
                        for(int z2=-1; z2<=1; z2++){
                            float3 cell = baseCell + float3(x2, y2, z2);
                            float3 cellPosition = cell + rand3dTo3d(cell);
                            float3 toCell = cellPosition - value;

                            float3 diffToClosestCell = abs(closestCell - cell);
                            bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
                            if(!isClosestCell){
                                float3 toCenter = (toClosestCell + toCell) * 0.5;
                                float3 cellDifference = normalize(toCell - toClosestCell);
                                float edgeDistance = dot(toCenter, cellDifference);
                                minEdgeDistance = min(minEdgeDistance, edgeDistance);
                            }
                        }
                    }
                }

                float random = rand3dTo1d(closestCell);
                return float3(minDistToCell, random, minEdgeDistance);
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 value = mul(unity_WorldToObject, float4(i.worldPos,2)).xyz / _CellSize;
                value.z += _Time.y * _TimeScale;

                float3 noise = voronoiNoise(value);

                float3 cellColor = float3(1.0, 0.0, 0.0);
                float valueChange = fwidth(value.z) * 0.5;
                float isBorder = 1 - smoothstep(0.05 - valueChange, 0.05 + valueChange, noise.z);
                float3 color = lerp(cellColor, _BorderColor, isBorder);

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}