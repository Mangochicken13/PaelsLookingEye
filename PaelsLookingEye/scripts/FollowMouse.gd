extends TextureRect
class_name PaelsLookingEye

var parent: Control
var parent_rect: Rect2
var screen_rect: Rect2
var rect_ratio: Vector2

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	parent = get_parent()
	print(parent)
	if parent:
		parent_rect = parent.get_rect()
		print(parent_rect)
		
		screen_rect = get_viewport_rect()
		parent.item_rect_changed.connect(_on_item_rect_changed)
		
		rect_ratio = Vector2(parent_rect.size.x / screen_rect.size.x, parent_rect.size.y / screen_rect.size.y)
		print(rect_ratio)
		

func _on_item_rect_changed() -> void:
	print(screen_rect)
	screen_rect = get_viewport_rect()
	print(screen_rect)

func _process(_delta: float) -> void:
	#print(parent_rect)
	var mouse_pos := parent.get_global_mouse_position()
	#print(mouse_pos)
	var relative_mouse_pos := (parent.global_position + parent.size / 2) - Vector2(mouse_pos)
	var transformed_relative_offset = Vector2(relative_mouse_pos.x * rect_ratio.x, relative_mouse_pos.y * rect_ratio.y)
	#position = (parent.size / 2)
	position = (-transformed_relative_offset * 0.2)
	#print(position)
