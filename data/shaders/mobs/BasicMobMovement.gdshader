// Wobble effect without CPU involvement
shader_type canvas_item;

uniform float wobble_speed = 5.0;
uniform float wobble_amount = 0.1;

void fragment() {
    vec2 uv = UV;
    uv.x += sin(TIME * wobble_speed + uv.y * 10.0) * wobble_amount;
    COLOR = texture(TEXTURE, uv);
}