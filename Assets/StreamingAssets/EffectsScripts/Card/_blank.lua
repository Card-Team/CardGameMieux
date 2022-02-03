---@type number
max_level = 1
---@type number
image_id = 519

---@type string
name = "Vide"
---@type number
pa_cost = 1

---@type string
description = "vide"

---@type Target[]
targets = {}

-- carte vide par défaut
-- elle est chargée automatiquement quand une carte est virtuelle
function precondition()
	return true
end

function do_effect()

end

