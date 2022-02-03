---@type number
max_level = 1
---@type number
image_id = 523

---@type string
name = "Annulation"
---@type number
pa_cost = 3

---@type string
description = "Annulle la derniere carte jouée"

---@type Target[]
targets = {
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EventStack.Count > 2
end

function do_effect()
	print("EventStack count : " .. EventStack.Count)
	for i = 3, EventStack.Count - 1 do
		print("Doing " .. EventStack.Count - i)
		local cur = EventStack.Count - i
		local ev = EventStack[cur]
		if ev.GetType() == CardEffectPlayEvent then
			print("Event " .. i .. " is " .. ev.ToString())
			ev.Cancelled = true
		end
	end
end