---@type number
max_level = 1
---@type number
image_id = 529

---@type string
name = "Sabotage"
---@type number
pa_cost = 3

---@type ChainMode
chain_mode = ChainMode.MiddleChain

--todo bouc emissaire (parce que j'ai modif conspiration en ca)
-- (idée de carte : bouc émissaire -> pas jouable, chainable middle, si chainée alors elles intercepte le targeting event et se met dedans)
---@type string
description = "Force la carté \"méchante\" jouée par votre adversaire a cibler cette carte-ci"

local function card_filter(aCard)
    -- carte choisis aleatoirement depuis ton deck
    return EffectOwner.Hand.Contains(aCard)
            and This ~= aCard
end

local function card_filter2(aCard)
    -- carte choisis aleatoirement depuis ton deck

    return EffectOwner.OtherPlayer.Discard.Contains(aCard)

end

---@type Target[]
targets = {
}

should_hijack = false

local function is_last_play_evil()
    print("isevil")
    ---@type CardEffectPlayEvent
    local lastEvt = --[[---@type CardEffectPlayEvent]] EventStack[EventStack.Count - 2]
    if lastEvt.GetType() ~= T_CardEffectPlayEvent then
        print("noteffectplay")
        return false
    end
    if EventStack[EventStack.Count - 1].GetType() ~=T_ChainingEvent then
        print("notchaining")
        return false
    end
    local effectName = lastEvt.Card.EffectId
    if effectName == "pistolet"
            or effectName == "echange"
            or effectName == "espion" then
        return true
    end
    print("not evil")
    return false
end

--- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    local evc = EventStack.Count >= 3
    print("precond : EventCount : " .. tostring(evc))
    return evc and is_last_play_evil()
end

function do_effect()
    local thisCard = This
    ---@param evt TargetingEvent
    SubscribeTo(T_TargetingEvent, function(evt, hnd)
        print("hijacking")
        evt.ResolvedTarget = thisCard
        UnsubscribeTo(hnd)
    end, true, false)
    
end 