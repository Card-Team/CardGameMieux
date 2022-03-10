---@type number
max_level = 2
---@type number
image_id = 530

---@type string
name = "Conspiration"
---@type number
pa_cost = 3

---@type string
description = "Annule l'amélioration d'une carte qui vient d'etre joué "

---@type Target[]
targets = {
}

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
	return EventStack[EventStack.Count -1] 
end

function do_effect()
	print("EventStack count : " .. EventStack.Count)
	for i = 3, EventStack.Count - 1 do
		print("Doing " .. EventStack.Count - i)
		local cur = EventStack.Count - i
		local ev = EventStack[cur]
		if ev.GetType() == T_CardEffectPlayEvent then
			local ev = --[[---@type CardEffectPlayEvent]] print("Event " .. i .. " is " .. ev.ToString())
			ev.Cancelled = true
		
		end
	end
end

